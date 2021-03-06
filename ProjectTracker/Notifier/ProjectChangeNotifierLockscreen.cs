﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace ProjectTracker
{
    class ProjectChangeNotifierLockscreen : AProjectChangeNotifier //publisher
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
                        ProjectChangeEvent.Types.Lock,
                        Handler.currentProject, //this is a hack and of course not necessarily true but was a convenient hack for the after unlock-prompt.
                        Handler.currentProjectComment,
                        new WorktimeRecord(
                            new DateTime(Handler.currentProjectSince.Ticks),
                            DateTime.Now,
                            Handler.currentProject,
                            Handler.currentProjectComment)
                        )
                    );
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                //I returned to my desk
                OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                    ProjectChangeEvent.Types.Unlock,
                    lastProject,
                    "",
                    new WorktimeRecord(
                        Handler.currentProjectSince,
                        DateTime.Now,
                        "[unknown-lock2]",
                        "unlocked")
                    )
                );
            }
        }
    }
}
