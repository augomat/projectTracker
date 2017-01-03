using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ProjectTracker
{
    class Presenter
    {
        //this should actually be an interface but that's not worth the work...
        private Form1 Form;
        public IWorktimebreakHandler WorktimebreakHandler { private get;  set; }
        public IProjectCorrectionHandler ProjectCorrectionHandler { private get; set; }
        public IProjectHandler ProjectHandler { private get; set; }

        public string currentProject { get { return ProjectHandler.currentProject; } } //TODO errorhandling
        public DateTime currentProjectSince { get { return ProjectHandler.currentProjectSince; } } //TODO errorhandling

        public Presenter(Form1 form)
        {
            Form = form;

            Form.correctProjectCombobox.Items.AddRange(ProjectChangeHandler.getAvailableProjects().Cast<string>().ToArray());

            Form.countAsWorktime.Leave += countAsWorktime_Leave;
            Form.carryOverHours.Leave += carryOverHours_Leave;
            Form.CorrectProject.Click += CorrectProject_Click;
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
    }
}
