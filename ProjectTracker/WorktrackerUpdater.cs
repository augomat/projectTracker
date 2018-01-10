using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ProjectTracker.Util;
using WorkTracker;

namespace ProjectTracker
{
    public class WorktrackerUpdater
    {
        private WorktrackerSettings WtSettings = new WorktrackerSettings();
        private ITaskMaster worktracker = null;
        private Employee currentUser = null;
        private Dictionary<string, Project> projects = new Dictionary<string, Project>(); //key: UniqueName

        public WorktrackerUpdater() { }

        public bool WorktrackerConnect()
        {
            try
            {
                /*
                var task = Task.Factory.StartNew(() =>
                {
                    try { return WorkTrackerConnection.GetRemoteService(); }
                    catch { return null; }
                });
                task.Wait(new TimeSpan(0, 0, 2));
                if (!task.IsCompleted)
                    throw new Exception("Could not establish connection to WT in time");

                worktracker = task.Result;
                */
                worktracker = WorkTrackerConnection.GetRemoteService();
                if (worktracker == null)
                    return false;

                if (currentUser == null)
                    currentUser = worktracker.GetEmployeeForAuthenticatedUser();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            } 
        }

        private bool WorktrackerConnectInternal()
        {
            return (worktracker == null) ? WorktrackerConnect() : true;
        }

        public void WorktrackerDisonnect()
        {
            worktracker = null;
        }
        
        public void updateProjectEntries(DateTime day, WorktimeStatistics wtstats)
        {
            if (!WorktrackerConnectInternal())
                return;

            updateWtProjects();

            var wtprojects = joinToWtProjects(wtstats);
            var quantizedProjects = quantizeProjectsTo5(wtprojects);

            addProjectEntriesToWorktracker(day, quantizedProjects);
        }
 
        //adjusts start/endtime such that a possible overtime can be billed
        public void updateFullDay(DateTime day, WorktimeStatistics wtstats, DateTime end)
        {
            if (!WorktrackerConnectInternal())
                return;

            var workEntries = worktracker.QueryWorkEntries(currentUser, day.Date, new TimeSpan(1, 0, 0, 0));

            if (workEntries.Count == 0)
                throw new Exception("No Workentry found");

            var workEntry = workEntries[0];

            var trueEnd = (workEntry.StopTime != default(DateTime) && workEntry.IsComplete) ? workEntry.StopTime : end;

            Tuple<DateTime, DateTime, TimeSpan> newTimes = calcAdaptedStartEndTimes(day, wtstats, trueEnd);
            workEntry.BreakDuration = newTimes.Item3;
            workEntry.StartTime = newTimes.Item1;
            workEntry.StopTime = newTimes.Item2;
            workEntry.IsComplete = true;

            worktracker.UpdateWorkEntry(workEntry);
        }

        public Tuple<DateTime, DateTime, TimeSpan> calcAdaptedStartEndTimes(DateTime day, WorktimeStatistics wtstats, DateTime origEnd)
        {
            var newPause = wtstats.totalPausetime;
            var tmpEnd = ((origEnd.Date == day.Date && origEnd.Hour > 22) || origEnd.Date == day.Add(new TimeSpan(24, 0, 0)).Date) 
                ? (day.Date + new TimeSpan(22, 0, 0)) 
                : origEnd;
            var tmpStart = (tmpEnd - wtstats.totalTime);

            //if start is after 12 o'clock, pretend we started to work at 10
            if (tmpStart.TimeOfDay > new TimeSpan(12,0,0))
            {
                var t = tmpStart.TimeOfDay - new TimeSpan(10, 0, 0);
                tmpStart -= t;
                tmpEnd -= t;
            }

            //if we begin before 6 o'clock, pretend that we started at least at 6
            if (tmpStart.TimeOfDay < new TimeSpan(6, 0, 0))
            {
                var t = new TimeSpan(6, 0, 0) - tmpStart.TimeOfDay; 
                tmpStart += t;
                tmpEnd += t;
            }
            var newStart = tmpEnd - wtstats.totalTime;
            var newEnd = tmpEnd;

            //if we are finishing the current day, finish now because otherwise value will be overridden by alarm checkout
            //However, it must not start before 6 o'clock in any case
            if (day.Date == DateTime.Now.Date && tmpEnd > DateTime.Now)
            {
                var t = tmpEnd - DateTime.Now;
                if ((tmpStart - t).TimeOfDay < new TimeSpan(6, 0, 0))
                {
                    var t2 = new TimeSpan(6, 0, 0) - (tmpStart - t).TimeOfDay;
                    t -= t2;
                }
                tmpStart -= t;
                tmpEnd -= t;
            }

            //if pause time is too short, shift start and make pause 40mins
            if (newPause < new TimeSpan(0,40,0))
            {
                newStart -= new TimeSpan(0, 40, 0) - wtstats.totalPausetime;
                newPause = new TimeSpan(0, 40, 0);
            }
            return new Tuple<DateTime, DateTime, TimeSpan>(newStart, newEnd, newPause);
        }

        public void updateBreak(DateTime day, TimeSpan breakTime)
        {
            if (!WorktrackerConnectInternal())
                return;

            var workEntries = worktracker.QueryWorkEntries(currentUser, day.Date, new TimeSpan(1, 0, 0, 0));

            if (workEntries.Count == 0)
                throw new Exception("No Workentry found");

            var workEntry = workEntries[0];

            workEntry.BreakDuration = breakTime;

            worktracker.UpdateWorkEntry(workEntry);
        }

        public void finishDayNow(DateTime day, TimeSpan breakTime)
        {
            if (day.Date != DateTime.Now.Date)
                throw new Exception("Only finishing the current day can be done automatically");

            finishDay(day, DateTime.Now, breakTime);
        }

        public void finishDay(DateTime day, DateTime end, TimeSpan breakTime)
        {
            if (!WorktrackerConnectInternal())
                return;

            var workEntries = worktracker.QueryWorkEntries(currentUser, day.Date, new TimeSpan(1, 0, 0, 0));

            if (workEntries.Count == 0)
                throw new Exception("No Workentry found");

            var workEntry = workEntries[0];

            if (workEntry.IsComplete)
                throw new Exception("Workentry is already complete and cannot be finished");
            
            workEntry.BreakDuration = breakTime;
            workEntry.StopTime = DateTime.Now;
            workEntry.IsComplete = true;

            worktracker.UpdateWorkEntry(workEntry);
        }

        private Dictionary<Project, float> joinToWtProjects(WorktimeStatistics wtstats)
        {
            var wtprojects = new Dictionary<Project, float>();
            foreach (var pjs in wtstats.relativeProjectTimes)
            {
                var wtprojectname = getWTProjectname(pjs.Key);
                var wtproject = projects.FirstOrDefault(x => x.Value.UniqueName == wtprojectname).Value; //todo auslagern
                if (wtproject == null)
                    throw new Exception($"WtProject name {wtprojectname} not found in Worktracker unique project Names");

                if (!wtprojects.ContainsKey(wtproject))
                    wtprojects[wtproject] = 0;
                wtprojects[wtproject] += pjs.Value;
            }
            return wtprojects;
        }

        private Dictionary<Project, int> quantizeProjectsTo5(Dictionary<Project, float> wtprojects)
        {
            const float quantizer = 5;
            var wtprojectsq = new Dictionary<Project, int>();

            //first round: quantize
            foreach (var wtproject in wtprojects)
            {
                if ((wtproject.Value % quantizer) > quantizer / 2)
                    wtprojectsq[wtproject.Key] = (int)(Math.Ceiling(wtproject.Value / quantizer) * quantizer);
                else
                    wtprojectsq[wtproject.Key] = (int)(Math.Floor(wtproject.Value / quantizer) * quantizer);
            }

            //check if we reached exactly 100%
            var countPercentage = 0;
            foreach (var pq in wtprojectsq)
            {
                countPercentage += pq.Value;
            }
            if (countPercentage == 100)
                return wtprojectsq;

            //second round: make adaptions so we fit 100% by adapting the most sensible projects (i.e. the ones with the biggest deviations)
            var errorList = new List<Tuple<Project, float>>();
            foreach (var pq in wtprojectsq)
                errorList.Add(Tuple.Create(pq.Key, Math.Abs(wtprojects[pq.Key] - pq.Value)));
            errorList = errorList.OrderByDescending(x => x.Item2).ToList();

            foreach (var pql in errorList)
            {
                if (countPercentage > 100)
                    wtprojectsq[pql.Item1] -= 5;
                else
                    wtprojectsq[pql.Item1] += 5;

                if (wtprojectsq.Values.Sum() == 100)
                    return wtprojectsq;
            }
            throw new Exception("Adaption algorithm was not able to find a sensible percentage-distribution");
        }

        private void addProjectEntriesToWorktracker(DateTime day, Dictionary<Project, int> pes)
        {
            var workEntries = worktracker.QueryWorkEntries(currentUser, day.Date, new TimeSpan(1, 0, 0, 0));

            if (workEntries.Count == 0)
                throw new Exception("No Workentry found");

            var workEntry = workEntries[0];

            if (!workEntry.IsComplete)
                throw new Exception("Workentry is not yet complete");

            if (worktracker.QueryProjectEntries(workEntry).Count > 0)
                throw new Exception("No Projects must already be booked on the workentry");

            foreach (var pe in pes)
            {
                if (pe.Value > 0)
                    worktracker.InsertProjectEntry(new ProjectEntry(pe.Key.Key, workEntry.Key, pe.Value, ""));
            }

            //TODO tracken auch von der pause & überschüssigen zeit
        }

        private void updateWtProjects()
        {
            projects.Clear();
            projects = worktracker.QueryAllProjects().ToDictionary(p => p.Key);
        }

        private string getWTProjectname(string project)
        {
            try
            {
                return WtSettings.projectToWorktrackerProject[project];
            }
            catch
            {
                throw new Exception($"Project name {project} not found in projectToWorktrackerProject in app.config");
            }
        }
    }

    public class WorktrackerSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        public SerializableDictionary<string, string> projectToWorktrackerProject
        {
            get { return (SerializableDictionary<string, string>)this["projectToWorktrackerProject"]; }
            set { }
        }
    }
}
