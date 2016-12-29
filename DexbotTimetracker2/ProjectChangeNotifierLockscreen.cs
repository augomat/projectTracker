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
        private string lastProject = "";

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
                lastProject = Handler.currentProject;

                //I left my desk
                OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Finish,
                        "[unknown]",
                        "Bye bye",
                        new WorktimeRecord(
                            new DateTime(Handler.currentProjectSince.Ticks),
                            DateTime.Now,
                            Handler.currentProject,
                            "locked")
                        )
                    );
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                //I returned to my desk
                Tuple<string, string> promptValues = Prompt.ShowDialog("Computer unlocked", "What did you do in the mean time?");
                var promptString = promptValues.Item1;
                var promptDesktop = promptValues.Item2;

                OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                    ProjectChangeEvent.Types.Start,
                    lastProject,
                    promptString,
                    new WorktimeRecord(
                        Handler.currentProjectSince,
                        DateTime.Now,
                        promptDesktop,
                        "unlocked: "+ promptString)
                    )
                );
            }
        }
    }
}
