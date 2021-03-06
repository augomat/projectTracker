﻿using System;
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

        private TimeSpan maxWorktime { get { return TimeSpan.Parse(Properties.Settings.Default.maxWorktime); } }

        public WorktimeAnalyzer(IWorktimeRecordStorage storage, IProjectHandler projectHandler)
        {
            Storage = storage;
            ProjectHandler = projectHandler;
        }

        public WorktimeStatistics AnalyzeWorkday(DateTime day)
        {
            DateTime from, to;
            ProjectUtilities.getWorkDayByDate(day, out from, out to);

            var items = Storage.getAllWorktimeRecords(from, to);
                
            if (DateTime.Now >= from && DateTime.Now <= to) //needed to have current project in analysis
                items.Add(new WorktimeRecord(
                            ProjectHandler.currentProjectSince,
                            DateTime.Now,
                            ProjectHandler.currentProject,
                            ProjectHandler.currentProjectComment)
                        );

            return generateStatistics(items);
        }

        private WorktimeStatistics generateStatistics(List<WorktimeRecord> worktimeRecords)
        {
            //TODO write current desktop to storage
            var currentStats = new WorktimeStatistics();
            foreach (var wtr in worktimeRecords)
            {
                if (String.IsNullOrEmpty(wtr.ProjectName) || wtr.ProjectName.Contains("[unknown"))
                    continue; //that happens e.g. for Init project rows

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
                else if (wtr.ProjectName == ProjectChangeHandler.PROJECT_PRIVAT)
                {
                    currentStats.totalPrivateTime += currentInterval;
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
                if (!currentStats.projectComments.ContainsKey(wtr.ProjectName))
                    currentStats.projectComments.Add(wtr.ProjectName, new Dictionary<string, TimeSpan>());

                var comment = !String.IsNullOrEmpty(wtr.Comment) ? wtr.Comment : "General";

                if (!currentStats.projectComments[wtr.ProjectName].ContainsKey(comment))
                    currentStats.projectComments[wtr.ProjectName].Add(comment, new TimeSpan());

                var newSum = currentStats.projectComments[wtr.ProjectName][comment] + currentInterval;
                currentStats.projectComments[wtr.ProjectName][comment] = newSum;

            }
            foreach (var project in currentStats.projectTimes)
                currentStats.relativeProjectTimes[project.Key] = (float)(project.Value.TotalSeconds / currentStats.totalProjectTime.TotalSeconds) * 100;

            //currentStats.totalTime.Milliseconds = 0;
            return currentStats;
        }

        public WorktimeStatistics considerOvertimeUndertime(WorktimeStatistics originalWts)
        {
            var newWts = new WorktimeStatistics();
            var newOvertimes = new Dictionary<string, TimeSpan>();

            calculateOvertimeUndertime(originalWts, Storage.getOvertimes() , out newWts, out newOvertimes);

            Storage.updateOvertimes(newOvertimes);

            return newWts;
        }



        public void calculateOvertimeUndertime(WorktimeStatistics originalWts, Dictionary<string, TimeSpan> originalOvertimes, out WorktimeStatistics newWts, out Dictionary<string, TimeSpan> newOvertimes)
        {
            newWts = new WorktimeStatistics(originalWts);
            newOvertimes = originalOvertimes.ToDictionary(e => e.Key, e => e.Value); //clone

            if (originalWts.totalWorktime > maxWorktime) //we worked overtime
            {
                TimeSpan diff = originalWts.totalWorktime - maxWorktime;

                var projectIndex = 0;
                while (diff > new TimeSpan(0, 0, 0))
                {
                    var project = originalWts.projectTimes.Keys.ToList().ElementAt(projectIndex);
                    var time = originalWts.projectTimes.Values.ToList().ElementAt(projectIndex);

                    var timeToSubtract = new TimeSpan(0, 0, 0);
                    if (time <= diff) //whole project time is consumed
                    {
                        if (!newOvertimes.ContainsKey(project))
                            newOvertimes[project] = new TimeSpan(0, 0, 0);
                        newOvertimes[project] += time;
                        newWts.projectTimes[project] = new TimeSpan(0, 0, 0);
                        timeToSubtract = time;
                        diff -= time;
                    }
                    else //project time is only partially consumed
                    {
                        if (!newOvertimes.ContainsKey(project))
                            newOvertimes[project] = new TimeSpan(0, 0, 0);
                        newOvertimes[project] += diff;
                        newWts.projectTimes[project] -= diff;
                        timeToSubtract = diff;
                        diff = new TimeSpan(0, 0, 0);
                    }

                    newWts.totalTime -= timeToSubtract;
                    newWts.totalProjectTime -= timeToSubtract;
                    newWts.totalWorktime -= timeToSubtract;

                    projectIndex++;
                }                
            }
            else //we worked less time than maxTime
            {
                TimeSpan diff = maxWorktime - originalWts.totalWorktime;

                var ovtIndex = 0;
                while (diff > new TimeSpan(0,0,0) && sumTimespans(originalOvertimes.Values.ToList()) > new TimeSpan(0,0,0))
                {
                    var project = originalOvertimes.Keys.ToList().ElementAt(ovtIndex);
                    var time = originalOvertimes.Values.ToList().ElementAt(ovtIndex);

                    var timeToAdd = new TimeSpan(0, 0, 0);
                    if (diff > time) //a whole overtime is consumed
                    {
                        timeToAdd = time;
                        newOvertimes[project] = new TimeSpan(0, 0, 0);
                        if (!newWts.projectTimes.ContainsKey(project))
                            newWts.projectTimes[project] = new TimeSpan(0, 0, 0);
                        newWts.projectTimes[project] += time;
                        diff -= time;
                        originalOvertimes[project] -= time;
                    }
                    else //an overtime is partially consumed
                    {
                        timeToAdd = diff;
                        newOvertimes[project] -= diff;
                        if (!newWts.projectTimes.ContainsKey(project))
                            newWts.projectTimes[project] = new TimeSpan(0, 0, 0);
                        newWts.projectTimes[project] += diff;
                        diff = new TimeSpan(0,0,0);
                    }

                    newWts.totalTime += timeToAdd;
                    newWts.totalProjectTime += timeToAdd;
                    newWts.totalWorktime += timeToAdd;

                    ovtIndex++;
                }
            }

            foreach (var project in newWts.projectTimes)
                newWts.relativeProjectTimes[project.Key] = (float)(project.Value.TotalSeconds / newWts.totalProjectTime.TotalSeconds) * 100;
    
        }

        public void takeAllProjectimeAsOvertime(WorktimeStatistics wts)
        {
            var newOvertimes = calcAllProjectimeAsOvertime(wts, Storage.getOvertimes());
            Storage.updateOvertimes(newOvertimes);
        }

        public Dictionary<string, TimeSpan> calcAllProjectimeAsOvertime(WorktimeStatistics wts, Dictionary<string, TimeSpan> originalOvertimes)
        {
            var newOvertimes = originalOvertimes.ToDictionary(e => e.Key, e => e.Value); //clone

            foreach (var pj in wts.projectTimes)
            {
                if (!newOvertimes.ContainsKey(pj.Key))
                    newOvertimes[pj.Key] = new TimeSpan(0, 0, 0);
                newOvertimes[pj.Key] += pj.Value; 
            }
            return newOvertimes;
        }

        static public TimeSpan sumTimespans(List<TimeSpan> spans)
        {
            var sum = new TimeSpan(0, 0, 0);
            foreach (var span in spans)
                sum += span;
            return sum;
        }

        //This method is just for testing as I had troubles with invoking the method with out-params
        private Tuple<WorktimeStatistics, Dictionary<string, TimeSpan>> calculateOvertimeUndertimeForTesting(WorktimeStatistics originalWts, Dictionary<string, TimeSpan> originalOvertimes)
        {
            WorktimeStatistics newWts = null;
            Dictionary<string, TimeSpan> overtimes = null;

            calculateOvertimeUndertime(originalWts, originalOvertimes, out newWts, out overtimes);

            return new Tuple<WorktimeStatistics, Dictionary<string, TimeSpan>>(newWts, overtimes);
        }
    }

    public class WorktimeStatistics
    {
        public Dictionary<string, TimeSpan> projectTimes = new Dictionary<string, TimeSpan>();
        public Dictionary<string, float> relativeProjectTimes = new Dictionary<string, float>();
        public Dictionary<string, Dictionary<string, TimeSpan>> projectComments = new Dictionary<string, Dictionary<string, TimeSpan>>(); //Project<Comment, Sum>
        public TimeSpan totalTime = new TimeSpan(0, 0, 0);
        public TimeSpan totalProjectTime = new TimeSpan(0, 0, 0);
        public TimeSpan totalWorktime = new TimeSpan(0, 0, 0);
        public TimeSpan totalPausetime = new TimeSpan(0, 0, 0);
        public TimeSpan totalWorkbreaktime = new TimeSpan(0, 0, 0);
        public TimeSpan totalPrivateTime = new TimeSpan(0, 0, 0);

        public WorktimeStatistics() { }

        public WorktimeStatistics(WorktimeStatistics wts)
        {
            projectTimes = wts.projectTimes.ToDictionary(e => e.Key, e => e.Value);
            relativeProjectTimes = wts.relativeProjectTimes.ToDictionary(e => e.Key, e => e.Value);
            projectComments = wts.projectComments.ToDictionary(e => e.Key, e => e.Value.ToDictionary(e1 => e1.Key, e1 => e1.Value));
            totalTime = wts.totalTime;
            totalProjectTime = wts.totalProjectTime;
            totalWorktime = wts.totalWorktime;
            totalPausetime = wts.totalPausetime;
            totalWorkbreaktime = wts.totalWorkbreaktime;
            totalPrivateTime = wts.totalPrivateTime;
        }
    }
}
