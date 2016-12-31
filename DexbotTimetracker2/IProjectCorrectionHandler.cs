using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    interface IProjectCorrectionHandler
    {
        Tuple<DateTime, DateTime> getCorrectedTimes(float percentage);
        void correctProject(string projectShortname, float percentage);
    }
}
