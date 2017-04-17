using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Util
{
    class ProjectUtilities
    {
        // Returns the workday which actually starts at 4am (per my definition)
        public static void getWorkDayByDate(DateTime day, out DateTime from, out DateTime to)
        {
            from = day.Date + new TimeSpan(4, 0, 0);
            to = day.Date.AddDays(1) + new TimeSpan(4, 0, 0);
        }

        // Returns the workday which actually starts at 4am (per my definition)
        public static void getWorkDayByDateTime(DateTime dayAndTime, out DateTime from, out DateTime to)
        {
            if (DateTime.Now.Hour >= 4)
                from = DateTime.Now.Date + new TimeSpan(4, 0, 0);
            else //if were are between 12am and 4am, we still count it as the day before
                from = DateTime.Now.Date.AddDays(-1) + new TimeSpan(4, 0, 0);
            to = from.AddDays(1);
        }
    }
}
