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
                OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                               ProjectChangeEvent.Types.Start,
                               "Good morning",
                               new WorktimeRecord( //RTODO no wtr needed
                                   DateTime.Now,
                                   DateTime.Now,
                                   "",
                                   "New day begun"),
                               true
                               )
                           );
                return true;
            }
            return false;
        }

        private static bool isNewDay(DateTime lastSwitched)
        {
            //return (DateTime.Now.Subtract(new TimeSpan(0, 0, 10)) > lastSwitched) ? true : false;
            var TodayAt4am = DateTime.Now.Date + new TimeSpan(4, 0, 0);
            return lastSwitched < TodayAt4am && TodayAt4am < DateTime.Now;
        }
    }
}
