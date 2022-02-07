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
        ProjectChangeEvent getSplitCurrentProjectEvent(string projectShortname, float percentage);
        void splitCurrentProject(string projectShortname, float percentage);
        void addNewCurrentProject(string projectShortname, string projectComment);
        void changeCurrentProject(string projectShortname, string projectComment);
        void splitCurrentProject(List<WorktimeRecord> projects);

    }
}
