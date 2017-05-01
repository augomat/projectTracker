using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;
using ProjectTracker.Util;

namespace ProjectTracker
{
    class Presenter
    {
        //this should actually be an interface but that's not worth the work...
        private Form1 Form;
        public IWorktimebreakHandler WorktimebreakHandler { private get;  set; }
        public IProjectCorrectionHandler ProjectCorrectionHandler { private get; set; } //TODO still needed?
        public IProjectHandler ProjectHandler { private get; set; }
        public WorktimeAnalyzer WorktimeAnalyzer { private get; set; }
        public IWorktimeRecordStorage storage { private get; set;  }

        private WorktimeStatistics ProjectStatistics;
        private WorktrackerUpdater wtUpdater = new WorktrackerUpdater();

        public string currentProject { get { return ProjectHandler.currentProject; } } //TODO errorhandling
        public DateTime currentProjectSince { get { return ProjectHandler.currentProjectSince; } } //TODO errorhandling


        public Presenter(Form1 form)
        {
            Form = form;

            Form.correctProjectCombobox.Items.AddRange(ProjectChangeHandler.getAvailableProjects().Cast<string>().ToArray());

            Form.countAsWorktime.Leave += countAsWorktime_Leave;
            Form.carryOverHours.Leave += carryOverHours_Leave;
            Form.CorrectProject.Click += CorrectProject_Click;
            Form.AnalyzeWorktimes.Click += AnalyzeWorktimes_Click;
            Form.SetInWorkT.Click += SetInWT_Click;
            Form.ButtonUpdate.Click += updateButton_Click;
            Form.dataGridView1.CellValueChanged += grid_CellValueChanged;
            Form.dataGridView1.CellValidating += dataGridView1_CellValidating;
            Form.dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            Form.dateTimePicker1.ValueChanged += updateButton_Click; // hack...
            Form.Activated += (o, i) => { refreshGrid(); };

            DateTime from, to;
            ProjectUtilities.getWorkDayByDateTime(DateTime.Now, out from, out to);
            Form.dateTimePicker1.Value = from;
        }

        public void showNotification(string title, string text)
        {
            Form.TrayIcon.BalloonTipTitle = (title != "") ? title : "[no title]";
            Form.TrayIcon.BalloonTipText = (text != "") ? text : "[no text]";
            Form.TrayIcon.ShowBalloonTip(10);
        }

        public TimeSpan getAvailableWorktimebreak()
        {
            //TODO errorhandling
            return WorktimebreakHandler.freeWorkbreaktime;
        }

        public Tuple<DateTime, DateTime> getProjectCorrections(float percentage)
        {
            //TODO errorhandling
            return ProjectCorrectionHandler.getCurrentProjectCorrectedTimes(percentage);
        }

        //------------------------------------------------
        //------------------------------------------------

        private void AnalyzeWorktimes_Click(object sender, EventArgs e)
        {
            try
            {
                var projectStatistics = WorktimeAnalyzer.AnalyzeWorkday(Form.dateTimePicker1.Value);

                Form.ProjectTimesSummary.Series.Clear();
                Series series = new Series
                {
                    Name = "projects",
                    IsVisibleInLegend = false,
                    ChartType = SeriesChartType.Column
                };
                Form.ProjectTimesSummary.Series.Add(series);

                foreach (var project in projectStatistics.projectTimes)
                {
                    series.Points.Add(project.Value.TotalMinutes);
                    var p = series.Points.Last();
                    p.AxisLabel = project.Key;
                    p.Label = Math.Round(projectStatistics.relativeProjectTimes[project.Key]).ToString() + "%";
                }
                Form.ProjectTimesSummary.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                Form.ProjectTimesSummary.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
                Form.ProjectTimesSummary.ChartAreas[0].BackColor = System.Drawing.SystemColors.Control;
                Form.ProjectTimesSummary.ChartAreas[0].RecalculateAxesScale();
                Form.ProjectTimesSummary.Invalidate();
                Form.ProjectTimesSummary.Visible = true;

                Form.totalTime.Text = projectStatistics.totalTime.ToString(@"hh\:mm\:ss");
                Form.UndefinedTime.Text = projectStatistics.totalUndefinedTime.ToString(@"hh\:mm\:ss");
                Form.PauseTime.Text = projectStatistics.totalPausetime.ToString(@"hh\:mm\:ss");
                Form.Worktime.Text = projectStatistics.totalWorktime.ToString(@"hh\:mm\:ss");
                Form.ProjectTime.Text = projectStatistics.totalProjectTime.ToString(@"hh\:mm\:ss");
                Form.Workbreaktime.Text = projectStatistics.totalWorkbreaktime.ToString(@"hh\:mm\:ss");

                refreshGrid();
                ProjectStatistics = projectStatistics;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SetInWT_Click(object sender, EventArgs e)
        {
            if (ProjectStatistics == null)
            {
                MessageBox.Show("Period must be analyzed first.");
                return;
            }

            try
            {
                wtUpdater.updateProjectEntries(Form.dateTimePicker1.Value, ProjectStatistics);
                MessageBox.Show("Project entries were successfully set",
                    "Worktracker",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Worktracker-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void countAsWorktime_Leave(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.countAsWorktimebreakMins = Int32.Parse(Form.countAsWorktime.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        private void carryOverHours_Leave(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.carryOverWorktimeCountHours = Int32.Parse(Form.carryOverHours.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CorrectProject_Click(object sender, EventArgs e)
        {
            if (Form.correctProjectCombobox.Text == "")
            {
                MessageBox.Show("Correct Project must not be empty!");
                return;
            }

            ProjectCorrectionHandler.correctCurrentProject(Form.correctProjectCombobox.Text, Form.getTrackerbarPercentage());
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            refreshGrid();
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var grid = Form.dataGridView1;
            string headerText = grid.Columns[e.ColumnIndex].HeaderText;

            // Abort validation if cell is not in the Project column or in last/new row.
            if (!headerText.Equals("Project")) return;

            // Confirm that the cell is not empty.
            if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
            {
                grid.Rows[e.RowIndex].ErrorText = "Project name must not be empty";
                e.Cancel = true;
            }
        }

        void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.   
            Form.dataGridView1.Rows[e.RowIndex].ErrorText = String.Empty;
        }

        private void grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var grid = Form.dataGridView1;

            try
            {
                if (grid.Columns[e.ColumnIndex].Name == "StartTime") //TODO consts oder so
                    storage.ChangeStartTime(e.RowIndex, DateTime.Parse(grid.Rows[e.RowIndex].Cells["StartTime"].Value.ToString()));
                if (grid.Columns[e.ColumnIndex].Name == "EndTime")
                    storage.ChangeEndTime(e.RowIndex, DateTime.Parse(grid.Rows[e.RowIndex].Cells["EndTime"].Value.ToString()));
                if (grid.Columns[e.ColumnIndex].Name == "Project")
                    storage.ChangeProjectName(e.RowIndex, grid.Rows[e.RowIndex].Cells["Project"].Value.ToString());
                if (grid.Columns[e.ColumnIndex].Name == "Comment")
                    storage.ChangeProjectComment(e.RowIndex, grid.Rows[e.RowIndex].Cells["Comment"].Value.ToString());
            } catch (Exception ex)
            {
                //We basically have 2 different forms of validation with this (see CellValidating), but it is apparently not possible 
                //to trigger any e.Cancel in CellValueChanged and there appears to be no really pretty way to uncouple the validation logic
                //in the data storage from the actually storing the value - I decided that, until there is no better implementation with
                //a data binding, that i don't care

                MessageBox.Show(ex.Message, "Validation Error");
            }

            refreshGrid();
        }


        public void refreshGrid()
        {
            try
            {
                int firstDisplayed = Form.dataGridView1.FirstDisplayedScrollingRowIndex;
                int displayed = Form.dataGridView1.DisplayedRowCount(true);
                int lastVisible = (firstDisplayed + displayed) - 1;
                int lastIndex = Form.dataGridView1.RowCount - 1;
                bool shouldAutoscroll = lastVisible == lastIndex;

                Form.dataGridView1.SuspendLayout();
                Form.dataGridView1.Rows.Clear();

                DateTime from, to;
                ProjectUtilities.getWorkDayByDate(Form.dateTimePicker1.Value, out from, out to);
                foreach (var wtr in storage.getAllWorktimeRecords(from, to))
                {
                    //TODO overnighters
                    Form.dataGridView1.Rows.Add(
                        wtr.Start.Date.ToShortDateString(),
                        wtr.Start.ToLongTimeString(),
                        wtr.End.ToLongTimeString(),
                        Math.Round((wtr.End - wtr.Start).TotalMinutes, 1),
                        wtr.ProjectName,
                        wtr.Comment);
                }

                if (shouldAutoscroll)
                    Form.dataGridView1.FirstDisplayedScrollingRowIndex = Form.dataGridView1.RowCount - displayed;
                else
                    Form.dataGridView1.FirstDisplayedScrollingRowIndex = firstDisplayed;

                Form.dataGridView1.ResumeLayout();
            }
            catch (Exception)
            {
                //swallow TODO
            }
        }

        public void setDate(DateTime date)
        {
            Form.Invoke(new MethodInvoker(delegate () {
                Form.dateTimePicker1.Value = date;
            }));
        }
    }
}
