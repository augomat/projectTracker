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
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Start && isNewDay(Handler.currentProjectSince))
            {
                /*OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                               ProjectChangeEvent.Types.GoodMorning,
                               Handler.currentProject,
                               "Good morning",
                               new WorktimeRecord(
                                   DateTime.Now,
                                   DateTime.Now,
                                   Handler.currentProject,
                                   "New day begun")
                               )
                           );
                return true;*/

                //Tryout: Just replace event instead of refiring, because the old event is per definition invalid
                projectChangeEvent = new ProjectChangeEvent(
                               ProjectChangeEvent.Types.GoodMorning,
                               Handler.currentProject,
                               "Good morning",
                               new WorktimeRecord(
                                   DateTime.Now,
                                   DateTime.Now,
                                   Handler.currentProject,
                                   "New day begun")
                               );
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
