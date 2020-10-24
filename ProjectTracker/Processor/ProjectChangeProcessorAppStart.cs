using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeProcessorAppStart : AProjectChangeProcessor
    {
        public ProjectChangeProcessorAppStart(ProjectChangeHandler handler) : base(handler)
        {
        }

        public override bool process(ProjectChangeEvent projectChangeEvent)
        {
            //If we just started we will rewrite the event to be an Init-Event
            if (string.IsNullOrEmpty(Handler.currentProject) && projectChangeEvent.Type == ProjectChangeEvent.Types.Change)
            {
                projectChangeEvent.Type = ProjectChangeEvent.Types.Init;
                projectChangeEvent.WorktimeRecord = new WorktimeRecord(
                                   DateTime.Now,
                                   DateTime.Now,
                                   projectChangeEvent.NewProject,
                                   "Project initialized");

                return false;
            }
            return false;
        }
    }
}
