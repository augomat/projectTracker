using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;

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
        public IList<WorktimeRecord> WorktimeRecords { private get; set; }

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
            Form.ButtonUpdate.Click += updateButton_Click;
            Form.Activated += (o, i) => { refreshGrid(); };
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
            return ProjectCorrectionHandler.getCorrectedTimes(percentage);
        }

        //------------------------------------------------
        //------------------------------------------------

        private void AnalyzeWorktimes_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime BeginDate;
                if (DateTime.Now.Hour >= 4 ) 
                    BeginDate = DateTime.Now.Date + new TimeSpan(4, 0, 0);
                else //if were are between 12am and 4am, we still count it as the day before
                    BeginDate = DateTime.Now.Date.AddDays(-1) + new TimeSpan(4, 0, 0);
                var EndDate = BeginDate.AddDays(1);

                var projectStatistics = WorktimeAnalyzer.Analyze(BeginDate, EndDate);

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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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

            ProjectCorrectionHandler.correctProject(Form.correctProjectCombobox.Text, Form.getTrackerbarPercentage());
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            refreshGrid();
        }

        public void refreshGrid()
        {
            Form.dataGridView1.Rows.Clear();
            foreach (var wtr in WorktimeRecords)
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
            Form.dataGridView1.Refresh();
        }
    }
}
