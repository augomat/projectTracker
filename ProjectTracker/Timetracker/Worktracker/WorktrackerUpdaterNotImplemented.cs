#if !WORKTRACKER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ProjectTracker.Util;

namespace ProjectTracker
{
    public class WorktrackerUpdater
    {
        
        public WorktrackerUpdater() { }

        public bool WorktrackerConnect()
        {
            throw new Exception("Worktracker not supported in current build");
        }


        public void WorktrackerDisonnect()
        {
            throw new Exception("Worktracker not supported in current build");
        }
        
        public void updateProjectEntries(DateTime day, WorktimeStatistics wtstats)
        {
            throw new Exception("Worktracker not supported in current build");
        }
 
        public void updateFullDay(DateTime day, WorktimeStatistics wtstats, DateTime end)
        {
            throw new Exception("Worktracker not supported in current build");
        }

        public Tuple<DateTime, DateTime, TimeSpan> calcAdaptedStartEndTimes(DateTime day, WorktimeStatistics wtstats, DateTime origEnd)
        {
            throw new Exception("Worktracker not supported in current build");
        }

        public void updateBreak(DateTime day, TimeSpan breakTime)
        {
            throw new Exception("Worktracker not supported in current build");
        }

        public void finishDayNow(DateTime day, TimeSpan breakTime)
        {
            throw new Exception("Worktracker not supported in current build");
        }

        public void finishDay(DateTime day, DateTime end, TimeSpan breakTime)
        {
            throw new Exception("Worktracker not supported in current build");
        }
    }
}
#endif
