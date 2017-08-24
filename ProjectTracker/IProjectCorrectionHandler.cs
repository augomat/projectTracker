using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    public interface IProjectCorrectionHandler
    {
        Tuple<DateTime, DateTime> getCurrentProjectCorrectedTimes(float percentage);
        ProjectChangeEvent getCorrectCurrentProjectEvent(string projectShortname, float percentage);
        void correctCurrentProject(string projectShortname, float percentage);
    }
}
