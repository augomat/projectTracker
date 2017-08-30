using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    public static class Extensions
    {
        public static string FormatForOvertime(this TimeSpan ts)
        {
            return Math.Floor(ts.TotalHours).ToString() + ':' + ts.ToString(@"mm\:ss");
        }
    }
}
