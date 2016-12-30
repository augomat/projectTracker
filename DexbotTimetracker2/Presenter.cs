using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectTracker
{
    class Presenter
    {
        //this should actually be an interface but that's not worth the work...
        private Form1 Form;
        public IWorktimebreakHandler WorktimebreakHandler { get; set; }

        public Presenter(Form1 form)
        {
            Form = form;

            Form.countAsWorktime.Leave += countAsWorktime_Leave;
            Form.carryOverHours.Leave += carryOverHours_Leave;
        }

        public void showNotification(string title, string text)
        {
            Form.TrayIcon.BalloonTipTitle = (title != "") ? title : "[no title]";
            Form.TrayIcon.BalloonTipText = (text != "") ? text : "[no text]";
            Form.TrayIcon.ShowBalloonTip(10);
        }

        public TimeSpan getAvailableWorktimebreak()
        {
            return WorktimebreakHandler.freeWorkbreaktime;
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
    }
}
