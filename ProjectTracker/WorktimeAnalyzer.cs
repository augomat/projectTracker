using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTracker.Util;

namespace ProjectTracker
{
    public class WorktimeAnalyzer
    {
        private IWorktimeRecordStorage Storage;
        private IProjectHandler ProjectHandler;
        private IProjectCorrectionHandler ProjectCorrectionHandler;

        private TimeSpan maxWorktime { get { return TimeSpan.Parse(Properties.Settings.Default.maxWorktime); } }

        public WorktimeAnalyzer(IWorktimeRecordStorage storage, IProjectHandler projectHandler, IProjectCorrectionHandler projectCorrectionHandler)
        {
            Storage = storage;
            ProjectCorrectionHandler = projectCorrectionHandler;
            ProjectHandler = projectHandler;
        }

        public WorktimeStatistics AnalyzeWorkday(DateTime day)
        {
            DateTime from, to;
            ProjectUtilities.getWorkDayByDate(day, out from, out to);

            if (DateTime.Now >= from && DateTime.Now <= to) //needed to have current project in analysis
                ProjectCorrectionHandler.correctCurrentProject(ProjectHandler.currentProject, 1);

            var items = Storage.getAllWorktimeRecords(from, to);
            return generateStatistics(items);
        }

        public WorktimeStatistics Analyze(DateTime from, DateTime to)
        {
            ProjectCorrectionHandler.correctCurrentProject(ProjectHandler.currentProject, 1);

            var items = Storage.getAllWorktimeRecords(from, to);
            return generateStatistics(items);
        }

        private WorktimeStatistics generateStatistics(List<WorktimeRecord> worktimeRecords)
        {
            //TODO write current desktop to storage
            var currentStats = new WorktimeStatistics();
            foreach (var wtr in worktimeRecords)
            {
                var currentInterval = TimeSpan.FromSeconds((wtr.End - wtr.Start).TotalSeconds);

                if (wtr.ProjectName == ProjectChangeHandler.PROJECT_WORKTIMEBREAK)
                {
                    currentStats.totalWorkbreaktime += currentInterval;
                    currentStats.totalWorktime += currentInterval;
                }
                else if (wtr.ProjectName == ProjectChangeHandler.PROJECT_PAUSE)
                {
                    currentStats.totalPausetime += currentInterval;
                }
                else if (wtr.ProjectName == ProjectChangeHandler.PROJECT_MEETING)
                {
                    currentStats.totalUndefinedTime += currentInterval;
                }
                else //normal project
                {
                    if (!currentStats.projectTimes.ContainsKey(wtr.ProjectName))
                        currentStats.projectTimes[wtr.ProjectName] = new TimeSpan(0, 0, 0);

                    currentStats.projectTimes[wtr.ProjectName] += currentInterval;
                    currentStats.totalProjectTime += currentInterval;
                    currentStats.totalWorktime += currentInterval;

                }
                currentStats.totalTime += currentInterval;

            }
            foreach (var project in currentStats.projectTimes)
                currentStats.relativeProjectTimes[project.Key] = (float)(project.Value.TotalSeconds / currentStats.totalProjectTime.TotalSeconds) * 100;

            //currentStats.totalTime.Milliseconds = 0;
            return currentStats;
        }

        public WorktimeStatistics considerOvertime(WorktimeStatistics originalWts)
        {
            var newWts = new WorktimeStatistics();
            var overtimes = new Dictionary<string, TimeSpan>();

            calculateOvertime(originalWts, out newWts, out overtimes);

            //TODO make db-entry

            return newWts;
        }



        private void calculateOvertime(WorktimeStatistics originalWts, out WorktimeStatistics newWts, out Dictionary<string, TimeSpan> overtimes)
        {
            newWts = originalWts;
            overtimes = new Dictionary<string, TimeSpan>();

            if (originalWts.totalWorktime > maxWorktime) //we worked overtime
            {
                TimeSpan diff = originalWts.totalWorktime - maxWorktime;

                var projectIndex = 0;
                while (diff > new TimeSpan(0, 0, 0))
                {
                    var project = originalWts.projectTimes.Keys.ToList().ElementAt(projectIndex);
                    var time = originalWts.projectTimes.Values.ToList().ElementAt(projectIndex);

                    //TODO if project not in official list, then ignore (and hope the other projects make up for enough overtime)

                    var timeToSubtract = new TimeSpan(0, 0, 0);
                    if (time <= diff) //whole project time is consumed
                    {
                        overtimes[project] = time;
                        newWts.projectTimes[project] = new TimeSpan(0, 0, 0);
                        timeToSubtract = time;
                        diff -= time;
                    }
                    else //project time is only partially consumed
                    {
                        overtimes[project] = diff;
                        newWts.projectTimes[project] -= diff;
                        timeToSubtract = diff;
                        diff = new TimeSpan(0, 0, 0);
                    }

                    newWts.totalTime -= timeToSubtract;
                    newWts.totalProjectTime -= timeToSubtract;
                    newWts.totalWorktime -= timeToSubtract;

                    projectIndex++;
                }

                foreach (var project in newWts.projectTimes)
                    newWts.relativeProjectTimes[project.Key] = (float)(project.Value.TotalSeconds / newWts.totalProjectTime.TotalSeconds) * 100;
            }
            else
            {
                newWts = originalWts;
            }
        }

        //This method is just for testing as I had troubles with invoking the method with out-params
        private Tuple<WorktimeStatistics, Dictionary<string, TimeSpan>> calculateOvertimeForTesting(WorktimeStatistics originalWts)
        {
            WorktimeStatistics newWts = null;
            Dictionary<string, TimeSpan> overtimes = null;

            calculateOvertime(originalWts, out newWts, out overtimes);

            return new Tuple<WorktimeStatistics, Dictionary<string, TimeSpan>>(newWts, overtimes);
        }
    }

    public class WorktimeStatistics
    {
        public Dictionary<string, TimeSpan> projectTimes = new Dictionary<string, TimeSpan>();
        public Dictionary<string, float> relativeProjectTimes = new Dictionary<string, float>();
        public TimeSpan totalTime = new TimeSpan(0, 0, 0);
        public TimeSpan totalProjectTime = new TimeSpan(0, 0, 0);
        public TimeSpan totalWorktime = new TimeSpan(0, 0, 0);
        public TimeSpan totalPausetime = new TimeSpan(0, 0, 0);
        public TimeSpan totalWorkbreaktime = new TimeSpan(0, 0, 0);
        public TimeSpan totalUndefinedTime = new TimeSpan(0, 0, 0);
    }
}
