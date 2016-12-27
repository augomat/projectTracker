using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectTracker
{
    class ProjectChangeSubscriberBalloonInformant : IProjectChangeSubscriber
    {
        private NotifyIcon TrayIcon;

        public ProjectChangeSubscriberBalloonInformant(NotifyIcon trayIcon)
        {
            TrayIcon = trayIcon;
        }

        public void handleProjectChangeEvent(object sender, ProjectChangeEvent projectChangeEvent)
        {
            if (!projectChangeEvent.Processed)
                return;

            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Init)
            {
                showBalloon("Project changed", "Desktop initialized");
                return;
            }
            if (projectChangeEvent.WorktimeRecord == null)
            {
                Console.WriteLine("Possibly wrong projectChangeEvent detected and ignored"); //mmmh... //RTODO log
                return;
            }

            var wtr = projectChangeEvent.WorktimeRecord;
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Change)
            {
                var project = wtr.ProjectName;
                var timePassed = (long)System.Math.Abs((wtr.End - wtr.Start).TotalSeconds);

                showBalloon("Project changed", "Time on last project [" + project + "]: " + (timePassed / 60).ToString() + " mins (" + timePassed.ToString() + " secs)");
            }
            else //if (projectChangeEvent.Type == ProjectChangeEvent.Types.Start || projectChangeEvent.Type == ProjectChangeEvent.Types.Finish)
            {
                showBalloon("Something with Project happened", projectChangeEvent.Message); // :) RTODO
            }
        }

        private void showBalloon(string title, string text)
        {
            TrayIcon.BalloonTipTitle = title;
            TrayIcon.BalloonTipText = text;
            TrayIcon.ShowBalloonTip(10);
        }
    }
}
