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
                //TODOOOOOOOOOO delete
                //projectChangeEvent.WorktimeRecord.Start = new DateTime(2017, 4, 17, 17, 0, 0);
                //projectChangeEvent.WorktimeRecord.End = new DateTime(2017, 4, 17, 20, 0, 0);

                var outlookAppointments = Util.OutlookAppointmentRetriever.retrieveAppointments(
                    projectChangeEvent.WorktimeRecord.Start, projectChangeEvent.WorktimeRecord.End);

                List<WorktimeRecord> breakTimes = new Prompt().ShowDialog(
                    projectChangeEvent.WorktimeRecord.Start, projectChangeEvent.WorktimeRecord.End,
                    outlookAppointments);

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
