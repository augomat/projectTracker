using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace ProjectTracker
{
    class ProjectChangeNotifierLockscreen : ProjectChangeNotifier //publisher
    {
        //string projectBeforeLock = "";

        public ProjectChangeNotifierLockscreen(ProjectChangeHandler handler) : base(handler) { }

        public override void start()
        {
            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock
                || e.Reason == SessionSwitchReason.RemoteDisconnect)
            {
                //I left my desk
                //long diffSecs = convertTicksToSec(DateTime.Now.Ticks) - convertTicksToSec(Handler.currentProjectSince.Ticks);
                OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Finish,
                        "Project finished",
                        "Computer locked", //"Time on Desktop [" + Handler.currentProject + "]: " + (diffSecs / 60).ToString() + " mins (" + diffSecs.ToString() + " secs)",
                        new WorktimeRecord(
                            new DateTime(Handler.currentProjectSince.Ticks),
                            DateTime.Now,
                            Handler.currentProject,
                            "locked")
                        )
                    );

                //projectBeforeLock = Handler.currentProject;
                //currentDesktop = "-1"; //break, no meeting - TODO make this enum
                //lastSwitchPassedSecs = convertTicksToSec(DateTime.Now.Ticks); */
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                //I returned to my desk

                //var lastSwitched = new DateTime(convertSecToTicks(lastSwitchPassedSecs));

                //check whether todays 4am is within the locked interval and if so, do not count it as a break
                if (isNewDay(Handler.currentProjectSince))
                {
                    //recordStartOfDay(); //TODO do not swallow return value

                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                       ProjectChangeEvent.Types.Start,
                       "Project started",
                       "Good Morning",
                       new WorktimeRecord(
                           Handler.currentProjectSince,
                           DateTime.Now,
                           Handler.currentProject,
                           "New day begun")
                       )
                   );
                }
                else
                {
                    Tuple<string, string> promptValues = Prompt.ShowDialog("Computer unlocked", "What did you do in the mean time?");
                    var promptString = promptValues.Item1;
                    var promptDesktop = promptValues.Item2;

                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                       ProjectChangeEvent.Types.Start,
                       "Project started",
                       "Computer unlocked", //"Time on Desktop [" + Handler.currentProject + "]: " + (diffSecs / 60).ToString() + " mins (" + diffSecs.ToString() + " secs)",
                       new WorktimeRecord(
                           Handler.currentProjectSince,
                           DateTime.Now,
                           promptDesktop,
                           "unlocked: "+ promptString)
                       )
                   );

                    //currentDesktop = promptDesktop;

                    //recordBackFromLockscreen(promptString, (promptDesktop == "0") ? true : false); //TODO do not swallow return value
                }
            }
        }

        private static bool isNewDay(DateTime lastSwitched)
        {
            var TodayAt4am = DateTime.Now.Date + new TimeSpan(4, 0, 0);
            return lastSwitched < TodayAt4am && TodayAt4am < DateTime.Now;
        }
    }
}
