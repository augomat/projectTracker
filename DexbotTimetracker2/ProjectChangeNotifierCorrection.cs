using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProjectTracker
{
    class ProjectChangeNotifierCorrection : ProjectChangeNotifier, IProjectCorrectionHandler
    {
        public ProjectChangeNotifierCorrection(ProjectChangeHandler handler) : base(handler) { }

        public override void start()
        {
            //nothing to do here as this notifier does not actually listen to any events    
        }

        public Tuple<DateTime, DateTime> getCorrectedTimes(float percentage)
        {
            if (percentage < 0 || percentage > 1)
                throw new Exception("Percentages must be between 0 and 1");
            if (Handler.currentProjectSince.Year == 1)
                return new Tuple<DateTime, DateTime>(new DateTime(), new DateTime()); //implement nullable?

              var totalSeconds = (DateTime.Now - Handler.currentProjectSince).TotalSeconds;
            var secsToProjectChange = totalSeconds * percentage;

            var datetimeProjectChange = Handler.currentProjectSince + TimeSpan.FromSeconds((int)secsToProjectChange);

            return new Tuple<DateTime, DateTime>(datetimeProjectChange, DateTime.Now);
        }

        public void correctProject(string projectShortname, float percentage)
        {
            OnRaiseProjectChangeEvent(getCorrectProjectEvent(projectShortname, percentage));
        }

        public ProjectChangeEvent getCorrectProjectEvent(string projectShortname, float percentage)
        {
            var correctedTimes = getCorrectedTimes(percentage);

            return new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        projectShortname,
                        "Project corrected",
                        new WorktimeRecord(
                            Handler.currentProjectSince,
                            correctedTimes.Item1,
                            Handler.currentProject,
                            "Project corrected")
                        );
        }
    }
}
