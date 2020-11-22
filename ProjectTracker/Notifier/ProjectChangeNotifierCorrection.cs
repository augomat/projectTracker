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

        public void correctCurrentProject(string projectShortname, float percentage)
        {
            OnRaiseProjectChangeEvent(getCorrectCurrentProjectEvent(projectShortname, percentage));
        }

        public ProjectChangeEvent getCorrectCurrentProjectEvent(string projectShortname, float percentage)
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
            // This is the implicit contract with DialogDefineProjects that if it full-time (which I cannot know here), 
            // it will only pass 1 param...hack...
            if (projects.Count == 1)
            {
                Handler.changeCurrentProjectRetrospectively(projects[0].ProjectName, projects[0].Comment);
                return;
            }

            for (int i = 0; i < projects.Count - 2; i++)
            {
                var project = projects[i];
                if ((project.End - project.Start).TotalSeconds < 60)
                    continue;
       
                OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                    ProjectChangeEvent.Types.Change,
                    projects[i].ProjectName,
                    projects[i].Comment,
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
