using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProjectTracker
{
    public class ProjectChangeNotifierCorrection : AProjectChangeNotifier, IProjectCorrectionHandler
    {
        public ProjectChangeNotifierCorrection(ProjectChangeHandler handler) : base(handler) { }

        public override void start()
        {
            //nothing to do here as this notifier does not actually listen to any events    
        }

        public Tuple<DateTime, DateTime> getCurrentProjectCorrectedTimes(float percentage)
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

        public void splitCurrentProject(string projectShortname, float percentage)
        {
            OnRaiseProjectChangeEvent(getSplitCurrentProjectEvent(projectShortname, percentage));
        }

        public ProjectChangeEvent getSplitCurrentProjectEvent(string projectShortname, float percentage)
        {
            var correctedTimes = getCurrentProjectCorrectedTimes(percentage);

            return new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        projectShortname,
                        "",
                        new WorktimeRecord(
                            Handler.currentProjectSince,
                            correctedTimes.Item1,
                            Handler.currentProject,
                            Handler.currentProjectComment)
                        );
        }

        public void changeCurrentProject(string projectShortname, string projectComment)
        {
            OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        projectShortname,
                        projectComment,
                        new List<WorktimeRecord>()));
        }

        public void addNewCurrentProject(string projectShortname, string projectComment)
        {
            OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        projectShortname,
                        projectComment,
                        new WorktimeRecord(
                            Handler.currentProjectSince,
                            DateTime.Now,
                            Handler.currentProject,
                            Handler.currentProjectComment)
                        ));
        }

        public void splitCurrentProject(List<WorktimeRecord> projects)
        {
            if (projects.Count == 1)
            {
                OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        projects[0].ProjectName,
                        projects[0].Comment,
                        new List<WorktimeRecord>()));
                return;
            }

            for (int i = 0; i < projects.Count - 2; i++)
            {
                var project = projects[i];
                if ((project.End - project.Start).TotalSeconds < 60)
                    continue;
       
                OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                    ProjectChangeEvent.Types.Change,
                    projects[i+1].ProjectName,
                    projects[i+1].Comment,
                    projects[i]));
            }

            //The last wtr is the current project, the penultimate the last project to be stored
            var lastProject = projects[projects.Count - 2];
            var currentProject = projects.Last();
            OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        currentProject.ProjectName,
                        currentProject.Comment,
                        lastProject));
        }
    }
}
