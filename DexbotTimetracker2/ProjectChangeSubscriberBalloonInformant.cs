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
            var timePassed = (long)System.Math.Abs((wtr.End - wtr.Start).TotalSeconds);
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Change)
            {
                var project = wtr.ProjectName;
                showBalloon("Project changed", "Time on last project [" + project + "]: " + (timePassed / 60).ToString() + " mins (" + timePassed.ToString() + " secs)");
            }
            else if (projectChangeEvent.Type == ProjectChangeEvent.Types.Start)
            { 
                if (projectChangeEvent.WorktimeRecords.Count == 1)
                {
                    var breakName = (wtr.ProjectName == "0") ? "Worktimebreak" : "Break";
                    showBalloon("Welcome back", String.Format("{0}: {1}min, Worktimebreak left: {2}sec", breakName, (timePassed / 60).ToString(), projectChangeEvent.AvailableWorktimebreak.TotalSeconds));
                }
                else
                {
                    //var timePassedWtb = (long)System.Math.Abs((projectChangeEvent.WorktimeRecords.First().End - projectChangeEvent.WorktimeRecords.First().Start).TotalSeconds);
                    //var timePassedBreak = (long)System.Math.Abs((projectChangeEvent.WorktimeRecords.Last().End - projectChangeEvent.WorktimeRecords.Last().Start).TotalSeconds);
                    var timePassedTotal = (long)System.Math.Abs((projectChangeEvent.WorktimeRecords.Last().End - projectChangeEvent.WorktimeRecords.First().Start).TotalSeconds);
                    showBalloon("Welcome back", String.Format("Total break: {0}min, no Worktimebreak left", (timePassedTotal / 60).ToString()));
                }
            }
            else //if (projectChangeEvent.Type == ProjectChangeEvent.Types.Start || projectChangeEvent.Type == ProjectChangeEvent.Types.Finish)
            {
                showBalloon("Something with Project happened", projectChangeEvent.Message); // :) RTODO
            }
        }

        private void showBalloon(string title, string text)
        {
            TrayIcon.BalloonTipTitle = (title != "") ? title : "[no title]";
            TrayIcon.BalloonTipText = (text != "") ? text : "[no text]";
            TrayIcon.ShowBalloonTip(10);
        }
    }
}
