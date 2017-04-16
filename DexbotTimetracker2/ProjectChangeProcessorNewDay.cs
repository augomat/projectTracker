using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeProcessorNewDay : ProjectChangeProcessor
    {
        public ProjectChangeProcessorNewDay(ProjectChangeHandler handler) : base(handler)
        { }

        public override bool process(ProjectChangeEvent projectChangeEvent)
        {
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Unlock && isNewDay(Handler.currentProjectSince))
            {   
                //Hack: Just replace event data instead of refiring, because the old event is per definition invalid
                projectChangeEvent.Type = ProjectChangeEvent.Types.GoodMorning;
                projectChangeEvent.Message = "Good Morning";
                projectChangeEvent.WorktimeRecord = new WorktimeRecord(
                                   DateTime.Now,
                                   DateTime.Now,
                                   projectChangeEvent.NewProject,
                                   "New day begun");
                return false;
            }
            return false;
        }

        private static bool isNewDay(DateTime lastSwitched)
        {
            var TodayAt4am = DateTime.Now.Date + new TimeSpan(4, 0, 0);
            return lastSwitched < TodayAt4am && TodayAt4am < DateTime.Now;
        }
    }
}
