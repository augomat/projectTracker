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
                /*OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        "Desktop Change detected",
                        "Time on Desktop [" + currentDesktop + "]: " + (diffSecs / 60).ToString() + " mins (" + diffSecs.ToString() + " secs)",
                        new WorktimeRecord(
                            new DateTime(convertSecToTicks(lastSwitchPassedSecs)),
                            DateTime.Now,
                            currentDesktop,
                            "")
                        )
                    );

                recordOnScreenSwitch("locked"); //TODO do not swallow return value
                desktopBeforeLock = currentDesktop;
                currentDesktop = "-1"; //break, no meeting - TODO make this enum
                lastSwitchPassedSecs = convertTicksToSec(DateTime.Now.Ticks); */
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                //I returned to my desk
                /* var lastSwitched = new DateTime(convertSecToTicks(lastSwitchPassedSecs));

                //check whether todays 4am is within the locked interval and if so, do not count it as a break
                if (isNewDay(lastSwitched))
                {
                    recordStartOfDay(); //TODO do not swallow return value
                }
                else
                {
                    Tuple<string, string> promptValues = Prompt.ShowDialog("Computer unlocked", "What did you do in the mean time?");
                    var promptString = promptValues.Item1;
                    var promptDesktop = promptValues.Item2;

                    currentDesktop = promptDesktop;

                    recordBackFromLockscreen(promptString, (promptDesktop == "0") ? true : false); //TODO do not swallow return value
                }

                currentDesktop = desktopBeforeLock;
                lastSwitchPassedSecs = convertTicksToSec(DateTime.Now.Ticks);
                */
            }
        }
    }
}
