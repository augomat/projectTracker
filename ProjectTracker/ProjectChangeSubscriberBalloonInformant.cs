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
        public delegate void ShowNotification(string title, string text);
        private ShowNotification showBalloon;

        public ProjectChangeSubscriberBalloonInformant(ShowNotification notificationDelegate)
        {
            showBalloon = notificationDelegate;
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
                Console.WriteLine("Possibly wrong projectChangeEvent detected and ignored - Worktimerecord was empty"); //mmmh... //RTODO log
                return;
            }

            var wtr = projectChangeEvent.WorktimeRecord;
            var timePassed = (long)System.Math.Abs((wtr.End - wtr.Start).TotalSeconds);
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Change)
            {
                var project = wtr.ProjectName;
                showBalloon("Project changed to [" + projectChangeEvent.NewProject + "]", "Time on last project [" + project + "]: " + (timePassed / 60).ToString() + " mins (" + timePassed.ToString() + " secs)");
            }
            else if (projectChangeEvent.Type == ProjectChangeEvent.Types.Start)
            {
                if (projectChangeEvent.WorktimeRecords.Count == 1)
                {
                    if (wtr.ProjectName == ProjectChangeHandler.PROJECT_WORKTIMEBREAK)
                        showBalloon("Welcome back", String.Format("{0}: {1}min, Worktimebreak left: {2}mins", "Worktimebreak", (timePassed / 60).ToString(), ((long)projectChangeEvent.AvailableWorktimebreak.TotalSeconds / 60).ToString()));
                    else
                        //if not processed via the wtb-handler, we do not have any wtb-left information
                        showBalloon("Welcome back", String.Format("{0}: {1}min", "Break", (timePassed / 60).ToString(), ((long)projectChangeEvent.AvailableWorktimebreak.TotalSeconds / 60).ToString()));
                }
                else //there is a wtb und a pause entry
                {
                    var timePassedTotal = (long)System.Math.Abs((projectChangeEvent.WorktimeRecords.Last().End - projectChangeEvent.WorktimeRecords.First().Start).TotalSeconds);
                    showBalloon("Welcome back", String.Format("Total break: {0}min, no Worktimebreak left", ((long)timePassedTotal / 60).ToString()));
                }
            }
            else if (projectChangeEvent.Type == ProjectChangeEvent.Types.Finish)
            {
                //ignore this message for now as this only happens when you logout
            }
            else if (projectChangeEvent.Type == ProjectChangeEvent.Types.GoodMorning)
            {
                showBalloon("Good Morning Sir", projectChangeEvent.Message);
            }
            else if (projectChangeEvent.Type == ProjectChangeEvent.Types.Exit)
            {
                //nothing to do here either as we do not need to inform the user that we are exiting the application :)
            }
            else if (projectChangeEvent.Type == ProjectChangeEvent.Types.Test)
            {
                showBalloon("Testmessage captured", projectChangeEvent.Message);
            }
        }
    }
}
