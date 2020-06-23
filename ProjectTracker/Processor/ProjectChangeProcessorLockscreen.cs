using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeProcessorLockscreen : AProjectChangeProcessor
    {
        public ProjectChangeProcessorLockscreen(ProjectChangeHandler handler) : base(handler)
        { }

        public override bool process(ProjectChangeEvent projectChangeEvent)
        {
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Lock)
            {
                //Hack: Just replace event data instead of refiring, because the old event is per definition invalid
                projectChangeEvent.Type = ProjectChangeEvent.Types.Finish;
                return false;
            }
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Unlock)
            {
                var outlookAppointments = Util.OutlookAppointmentRetriever.retrieveAppointments(
                    projectChangeEvent.WorktimeRecord.Start, projectChangeEvent.WorktimeRecord.End);

                List<WorktimeRecord> breakTimes = new DialogDefineProjects().ShowDialogMeantime(
                    projectChangeEvent.WorktimeRecord.Start, 
                    projectChangeEvent.WorktimeRecord.End,
                    Handler,
                    outlookAppointments);

                if (breakTimes != null)
                {
                    var currentProject = breakTimes.Last();
                    breakTimes.Remove(currentProject);

                    foreach (var brk in breakTimes)
                    {
                        OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                            ProjectChangeEvent.Types.Start,
                            currentProject.ProjectName,
                            currentProject.Comment,
                            brk));
                    }
                }
                return true;
            }
            return false;
        }
    }
}
