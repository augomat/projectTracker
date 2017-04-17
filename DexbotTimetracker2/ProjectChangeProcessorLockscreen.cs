using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeProcessorLockscreen : ProjectChangeProcessor
    {
        public ProjectChangeProcessorLockscreen(ProjectChangeHandler handler) : base(handler)
        { }

        public override bool process(ProjectChangeEvent projectChangeEvent)
        {
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Lock)
            {
                //Hack: Just replace event data instead of refiring, because the old event is per definition invalid
                projectChangeEvent.Type = ProjectChangeEvent.Types.Start;
                return false;
            }
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Unlock)
            {
                List<WorktimeRecord> breakTimes = new Prompt().ShowDialog(projectChangeEvent.WorktimeRecord.Start, projectChangeEvent.WorktimeRecord.End);

                foreach (var brk in breakTimes)
                {
                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        brk.ProjectName,
                        brk.Comment,
                        brk));
                }
                return true;
            }
            return false;
        }
    }
}
