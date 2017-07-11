﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTracker.Util;

namespace ProjectTracker
{
    class WorktimeAnalyzer
    {
        private IWorktimeRecordStorage Storage;
        private IProjectHandler ProjectHandler;
        private IProjectCorrectionHandler ProjectCorrectionHandler;

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