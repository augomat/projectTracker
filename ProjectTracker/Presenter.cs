using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;
using ProjectTracker.Util;
using System.Drawing;

namespace ProjectTracker
{
    public class Presenter
    {
        //this should actually be an interface but that's not worth the work...
        public Form1 Form;
        public IWorktimebreakHandler WorktimebreakHandler { private get;  set; }
        public IProjectCorrectionHandler ProjectCorrectionHandler { private get; set; } //TODO still needed?
        public IProjectHandler ProjectHandler { private get; set; }
        public WorktimeAnalyzer WorktimeAnalyzer { private get; set; }
        public IWorktimeRecordStorage storage { private get; set;  }
        public WorktrackerUpdater wtUpdater;

        public string currentProject { get { return ProjectHandler.currentProject; } } //TODO errorhandling
        public DateTime currentProjectSince { get { return ProjectHandler.currentProjectSince; } } //TODO errorhandling
        public string currentProjectComment { get { return ProjectHandler.currentProjectComment; } set { ProjectHandler.currentProjectComment = value; } } //TODO errorhandling


        public Presenter(Form1 form)
        {
            Form = form;

            Form.correctProjectCombobox.Items.AddRange(ProjectChangeHandler.getAvailableProjects().Cast<string>().ToArray());

#if !WORKTRACKER
            Form.SetInWorkT.Enabled = false;
            Form.finishWTday.Enabled = false;
            Form.autoFinish.Enabled = false;
#endif
            Form.countAsWorktime.Leave += countAsWorktime_Leave;
            Form.carryOverHours.Leave += carryOverHours_Leave;
            Form.maxWorktime.Leave += maxWorktime_Leave;
            Form.finishWTday.CheckedChanged += FinishWTday_Leave;
            Form.autoFinish.CheckedChanged += AutoFinish_Leave;
            Form.timeularAPIkey.Leave += apiKey_Leave;
            Form.timeularAPIsecret.Leave += apiSecret_Leave;
            Form.dexbotFilepath.Leave += dexbotLog_Leave;

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

        public void onInitCompleted()
        {
            Form.currentOvertime.Text = WorktimeAnalyzer.sumTimespans(storage.getOvertimes().Values.ToList()).FormatForOvertime();

            var t = new Thread(() => {
                var waitingTime = 3;
                while (String.IsNullOrEmpty(ProjectHandler.currentProject))
                {
                    System.Threading.Thread.Sleep(1000*waitingTime);
                    waitingTime *= 10;

                    if (String.IsNullOrEmpty(ProjectHandler.currentProject))
                    {
                        waitForHandleCreated(Form);
                        Form.Invoke(new MethodInvoker(delegate () {
                            showNotification("Initialize project", "ProjectTracker is not yet initialized with a project");
                        }));
                    }
                        
                }
                
            });
            t.Name = "ProjectInitializedChecker";
            t.Start();
        }

        

        public void showNotification(string title, string text)
        {
            Form.TrayIcon.BalloonTipTitle = (title != "") ? title : "[no title]";
            Form.TrayIcon.BalloonTipText = (text != "") ? text : "[no text]";
            Form.TrayIcon.BalloonTipClicked += Form.ShowForm;
            Form.TrayIcon.ShowBalloonTip(10);
        }

        public void showError(string title, string text)
        {
            waitForHandleCreated(Form);
            Form.Invoke(new MethodInvoker(delegate () {
                MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
            
        }

        public void ShowDialogAddComment()
        {
            new Prompt().ShowDialogChangeCurrentComment(ProjectHandler);
        }

        public void ShowDialogNewProject()
        {
            var newCurrentProject = new Prompt().ShowDialogNewProject(ProjectHandler);
            if (newCurrentProject != null)
                ProjectCorrectionHandler.addNewCurrentProject(newCurrentProject.ProjectName, newCurrentProject.Comment);
        }

        public void setDexpotError(string text)
        {
            waitForHandleCreated(Form);
            Form.Invoke(new MethodInvoker(delegate () {
                ToolTip yourToolTip = new ToolTip();

                yourToolTip.SetToolTip(Form.dexbotStatus, text);
            }));
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

                WorktimeStatistics projectStatisticsReal = null; 
                if (Form.flagConsiderOvertime.Checked)
                {
                    Dictionary<string, TimeSpan> newOvertimes = null;
                    WorktimeAnalyzer.calculateOvertimeUndertime(projectStatistics, storage.getOvertimes(), out projectStatisticsReal, out newOvertimes);
                    
                }
                else
                    projectStatisticsReal = projectStatistics;

                Form.currentOvertime.Text = WorktimeAnalyzer.sumTimespans(storage.getOvertimes().Values.ToList()).FormatForOvertime();
                Form.ProjectTimesSummary.Series.Clear();
                Series series = new Series
                {
                    Name = "projects",
                    IsVisibleInLegend = false,
                    ChartType = SeriesChartType.Column
                };
                Form.ProjectTimesSummary.Series.Add(series);

                foreach (var project in projectStatisticsReal.projectTimes)
                {
                    series.Points.Add(project.Value.TotalMinutes);
                    var p = series.Points.Last();
                    p.AxisLabel = project.Key;
                    p.Label = Math.Round(projectStatisticsReal.relativeProjectTimes[project.Key]).ToString() + "%";
                }
                Form.ProjectTimesSummary.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                Form.ProjectTimesSummary.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
                Form.ProjectTimesSummary.ChartAreas[0].BackColor = System.Drawing.SystemColors.Control;
                Form.ProjectTimesSummary.ChartAreas[0].RecalculateAxesScale();
                Form.ProjectTimesSummary.Invalidate();
                Form.ProjectTimesSummary.Visible = true;

                Form.totalTime.Text = projectStatisticsReal.totalTime.ToString(@"hh\:mm\:ss");
                Form.UndefinedTime.Text = projectStatisticsReal.totalUndefinedTime.ToString(@"hh\:mm\:ss");
                Form.PauseTime.Text = projectStatisticsReal.totalPausetime.ToString(@"hh\:mm\:ss");
                Form.Worktime.Text = projectStatisticsReal.totalWorktime.ToString(@"hh\:mm\:ss");
                Form.ProjectTime.Text = projectStatisticsReal.totalProjectTime.ToString(@"hh\:mm\:ss");
                Form.Workbreaktime.Text = projectStatisticsReal.totalWorkbreaktime.ToString(@"hh\:mm\:ss");

                refreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SetInWT_Click(object sender, EventArgs e)
        {
            WorktimeStatistics projectStatistics;
            try
            {
                projectStatistics = WorktimeAnalyzer.AnalyzeWorkday(Form.dateTimePicker1.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            if (isHoliday(Form.dateTimePicker1.Value))
            {
                var dialogResult = MessageBox.Show("You should only do this at the end of the day! When done multiple times, overtimes will be wrong.\nAre you sure you want to log the worktime now?",
                        "ProjectTracker",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Exclamation);
                WorktimeAnalyzer.takeAllProjectimeAsOvertime(projectStatistics);
                if (dialogResult == DialogResult.No)
                    return;

                Form.currentOvertime.Text = WorktimeAnalyzer.sumTimespans(storage.getOvertimes().Values.ToList()).FormatForOvertime();
                MessageBox.Show("As it is a holiday, all project time was considered as overtime",
                        "ProjectTracker",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                return;
            }
            
            try //it's a normal workday (and not a weekend/holiday)
            {
                if (!wtUpdater.WorktrackerConnect())
                    throw new Exception("Could not connect to Worktracker");

                if (Form.finishWTday.Checked)
                    wtUpdater.finishDayNow(Form.dateTimePicker1.Value, projectStatistics.totalPausetime);

                if (Form.flagConsiderOvertime.Checked)
                {
                    var projectStatisticsAdapted = WorktimeAnalyzer.considerOvertimeUndertime(projectStatistics);

                    DateTime from, to;
                    ProjectUtilities.getWorkDayByDate(Form.dateTimePicker1.Value, out from, out to);
                    var wtrs = storage.getAllWorktimeRecords(from, to);
                    var maxDate = wtrs.Max(wtr => wtr.End);
                    var wtrsEnd = wtrs.First(wtr => wtr.End == maxDate).End;

                    wtUpdater.updateFullDay(Form.dateTimePicker1.Value, projectStatisticsAdapted, wtrsEnd); //unfortunately if something fails here, the overtime-db was updated anyways
                    wtUpdater.updateProjectEntries(Form.dateTimePicker1.Value, projectStatisticsAdapted);

                    Form.currentOvertime.Text = WorktimeAnalyzer.sumTimespans(storage.getOvertimes().Values.ToList()).FormatForOvertime();
                    MessageBox.Show("Day with under-/overtime and project entries were successfully set",
                        "Worktracker",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    wtUpdater.updateProjectEntries(Form.dateTimePicker1.Value, projectStatistics);

                    MessageBox.Show("Project entries were successfully set",
                        "Worktracker",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Worktracker-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private bool isHoliday(DateTime date)
        {
            return (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) ? true : false;
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

            if (e.RowIndex == Form.dataGridView1.Rows.Count - 1 && currentProjectVisible())
            {
                if (grid.Columns[e.ColumnIndex].Name == "Comment")            
                    currentProjectComment = grid.Rows[e.RowIndex].Cells["Comment"].Value.ToString();
                return;
            }
                

            try
            {
                
                var date = DateTime.Parse(grid.Rows[e.RowIndex].Cells["Date"].Value.ToString());
                var startTime = DateTime.Parse(date.Date.ToString().Substring(0, 10) + " " + grid.Rows[e.RowIndex].Cells["StartTime"].Value.ToString());
                var startDateTime = date.Date.Add(startTime.TimeOfDay);
                var endTime = DateTime.Parse(date.Date.ToString().Substring(0, 10) + " " + grid.Rows[e.RowIndex].Cells["EndTime"].Value.ToString());
                var endDateTime = date.Date.Add(endTime.TimeOfDay); //TODO compensate date for overnighters
                var index = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["Index"].Value);

                if (grid.Columns[e.ColumnIndex].Name == "StartTime") //TODO consts oder so
                    storage.ChangeStartTime(index, startTime);
                if (grid.Columns[e.ColumnIndex].Name == "EndTime")
                    storage.ChangeEndTime(index, endTime);
                if (grid.Columns[e.ColumnIndex].Name == "Project")
                    storage.ChangeProjectName(index, grid.Rows[e.RowIndex].Cells["Project"].Value.ToString());
                if (grid.Columns[e.ColumnIndex].Name == "Comment")
                    storage.ChangeProjectComment(index, grid.Rows[e.RowIndex].Cells["Comment"].Value?.ToString() ?? "");
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
            if (storage == null)
                return;

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
                var wtrs = storage.getAllWorktimeRecords(from, to);
                foreach (var wtr in wtrs)
                {
                    //TODO overnighters
                    Form.dataGridView1.Rows.Add(
                        wtr.Start.Date.ToShortDateString(),
                        wtr.Start.ToLongTimeString(),
                        wtr.End.ToLongTimeString(),
                        Math.Round((wtr.End - wtr.Start).TotalMinutes, 1),
                        wtr.ProjectName,
                        wtr.Comment,
                        wtr.storageID);
                }

                // If we need to show current project
                if (currentProject != null && DateTime.Now >= from && DateTime.Now <= to)
                {
                    //Add current project
                    Form.dataGridView1.Rows.Add(
                        wtrs.Last().Start.Date.ToShortDateString(),
                        wtrs.Last().End.ToLongTimeString(),
                        "",
                        "",
                        currentProject,
                        currentProjectComment,
                        "");

                    //Make all cells except comment readonly
                    int lastRow = Form.dataGridView1.Rows.Count - 1;
                    foreach (DataGridViewCell cell in Form.dataGridView1.Rows[lastRow].Cells)
                    {
                        if (cell.OwningColumn.HeaderText != "Comment")
                            cell.ReadOnly = true;
                    }

                    Form.dataGridView1.Rows[lastRow].DefaultCellStyle.BackColor = Color.Gold;
                    Form.dataGridView1.Rows[lastRow].DefaultCellStyle.SelectionBackColor = Color.Gold;
                    Form.dataGridView1.Rows[lastRow].DefaultCellStyle.ForeColor = Color.Gray;
                    Form.dataGridView1.Rows[lastRow].DefaultCellStyle.SelectionForeColor = Color.Gray;
                }


                if (shouldAutoscroll)
                    Form.dataGridView1.FirstDisplayedScrollingRowIndex = Form.dataGridView1.RowCount - displayed;
                else if (firstDisplayed != -1)
                    Form.dataGridView1.FirstDisplayedScrollingRowIndex = firstDisplayed; //Form.dataGridView1.FirstDisplayedScrollingRowIndex will return -1 when not initialized or so but throws an exception when set

                Form.dataGridView1.ResumeLayout();
            }
            catch (Exception)
            {
                //swallow TODO
            }
        }

        public bool currentProjectVisible()
        {
            return Form.dataGridView1.Rows.Count > 0 
                && Form.dataGridView1.Rows[Form.dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor == Color.Gold; //what a hack
        }

        public void setDate(DateTime date)
        {
            waitForHandleCreated(Form);
            Form.Invoke(new MethodInvoker(delegate () {
                Form.dateTimePicker1.Value = date;
            }));
        }

        public void setNotifierState(string notifierName, bool enabled)
        {
            Label label = null;
            if (notifierName == "Dexbot")
                label = Form.dexbotStatus;
            if (notifierName == "Timeular")
                label = Form.timeularStatus;

            if (label != null)
            {
                waitForHandleCreated(Form);
                if (enabled)
                {
                    Form.Invoke(new MethodInvoker(delegate () {
                        label.Text = "enabled";
                        label.BackColor = Color.Green;
                        label.ForeColor = Color.White;
                    }));   
                }
                else
                {
                    Form.Invoke(new MethodInvoker(delegate () {
                        label.Text = "disabled";
                        label.BackColor = Color.Red;
                        label.ForeColor = Color.Black;
                    }));
                }
            }
        }

        private void waitForHandleCreated(Form form)
        {
            while(!form.IsDisposed && form.IsHandleCreated == false)
                System.Threading.Thread.Sleep(100);
        }

        //-----------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------


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

        private void maxWorktime_Leave(object sender, EventArgs e)
        {
            try
            {
                TimeSpan.Parse(Form.maxWorktime.Text);
                Properties.Settings.Default.maxWorktime = Form.maxWorktime.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FinishWTday_Leave(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.flagFinishWTDay = Form.finishWTday.Checked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AutoFinish_Leave(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.flagAutoFinishWT = Form.autoFinish.Checked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void apiKey_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.timeularAPIkey = Form.timeularAPIkey.Text;
        }

        private void apiSecret_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.timeularAPIsecret = Form.timeularAPIsecret.Text;
        }

        private void dexbotLog_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.DexbotLogFilePath = Form.dexbotFilepath.Text;
        }
    }
}
