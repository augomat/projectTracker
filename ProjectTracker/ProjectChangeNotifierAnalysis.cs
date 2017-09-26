using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    public class ProjectChangeNotifierAnalysis : ProjectChangeNotifier
    {
        public ProjectChangeNotifierAnalysis(ProjectChangeHandler handler) : base(handler) { }

        public override void start()
        {
            //nothing to do here as this notifier does not actually listen to any events    
        }

        public void logCurrentProject()
        {
            OnRaiseProjectChangeEvent(getAnalyseCurrentProjectEvent());
        }

        public ProjectChangeEvent getAnalyseCurrentProjectEvent()
        {
            return new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        Handler.currentProject,
                        "For analysis of projectstatistics",
                        new WorktimeRecord(
                            Handler.currentProjectSince,
                            DateTime.Now,
                            Handler.currentProject,
                            "For analysis of projectstatistics")
                        );
        }
    }
}
