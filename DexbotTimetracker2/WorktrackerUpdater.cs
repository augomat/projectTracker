﻿using System;
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
        private ITaskMaster worktracker;
        private Employee currentUser;
        private Dictionary<string, Project> projects = new Dictionary<string, Project>(); //key: UniqueName

        public WorktrackerUpdater()
        {
            worktracker = WorkTrackerConnection.GetRemoteService();
            currentUser = worktracker.GetEmployeeForAuthenticatedUser();
        }
        
        public void updateProjectEntries(DateTime day, WorktimeStatistics wtstats)
        {
            updateWtProjects();

            var wtprojects = joinToWtProjects(wtstats);
            var quantizedProjects = quantizeProjectsTo5(wtprojects);

            addProjectEntriesToWorktracker(day, quantizedProjects);
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
                errorList.Add(Tuple.Create(pq.Key, wtprojects[pq.Key] - pq.Value));
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
            var workEntries = worktracker.QueryWorkEntries(currentUser, day, new TimeSpan(1, 0, 0, 0));
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
