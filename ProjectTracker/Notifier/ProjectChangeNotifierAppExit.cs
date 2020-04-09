using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectTracker
{
    class ProjectChangeNotifierAppExit : AProjectChangeNotifier
    {
        public ProjectChangeNotifierAppExit(ProjectChangeHandler handler) : base(handler)
        {
        }

        public override void start()
        {
            Application.ApplicationExit += Application_ApplicationExit;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Exit,
                        "[unknown-exit]",
                        "",
                        new WorktimeRecord(
                            Handler.currentProjectSince,
                            DateTime.Now,
                            Handler.currentProject,
                            Handler.currentProjectComment)
                        )
                    );
        }
    }
}
