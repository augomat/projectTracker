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
            }
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Unlock)
            {
                Tuple<string, string> promptValues = Prompt.ShowDialog("Computer unlocked", "What did you do in the mean time?");
                var promptString = promptValues.Item1;
                var promptDesktop = promptValues.Item2;

                //Hack: Just replace event data instead of refiring, because the old event is per definition invalid
                projectChangeEvent.Type = ProjectChangeEvent.Types.Finish;
                projectChangeEvent.Message = promptString;
                //projectChangeEvent.NewProject = promptDesktop;
                projectChangeEvent.WorktimeRecord.Comment = "unlocked: " + promptString;
                projectChangeEvent.WorktimeRecord.ProjectName = promptDesktop;
            }
            return false;
        }
    }
}
