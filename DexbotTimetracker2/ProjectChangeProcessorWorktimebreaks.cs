﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeProcessorWorktimebreaks : ProjectChangeProcessor, IWorktimebreakHandler
    {
        private int CountAsWorktimebreakMins { get { return Properties.Settings.Default.countAsWorktimebreakMins; } } 
        private int CarryOverWorktimeCountHours { get { return Properties.Settings.Default.carryOverWorktimeCountHours; } } 

        private int freeWorktimebreakSecs = 0;
        public TimeSpan freeWorkbreaktime { get { return getCurrentFreeWorktimebreak(); } }

        public ProjectChangeProcessorWorktimebreaks(ProjectChangeHandler handler)  : base(handler) { }

        public override bool process(ProjectChangeEvent projectChangeEvent)
        {
            if (projectChangeEvent.WorktimeRecord == null)
                return false;

            if (projectChangeEvent.Type == ProjectChangeEvent.Types.GoodMorning)
            {
                //Reset free workbreaktime every new day
                freeWorktimebreakSecs = 0;
                return false;
            }
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Start)
            {
                return comeBackFromOffScreen(projectChangeEvent);
            }
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Change)
            {
                updateFreeWorktimeBreakOnProjectChange();
                return false;
            }
            return false;
        }

        private bool comeBackFromOffScreen(ProjectChangeEvent projectChangeEvent)
        {
            var wtr = projectChangeEvent.WorktimeRecord;
            bool declaredAsWorktimebreak = (wtr.ProjectName == "0") ? true : false; //RTODO auslagern

            //Update free Worktimebreak
            var normalBreakSecs = updateFreeWorktimeBreakOnUnlock();

            if (declaredAsWorktimebreak)
            {
                if (normalBreakSecs > 0)
                {
                    //write worktimebreak
                    var worktimebreakEnd = DateTime.Now - TimeSpan.FromSeconds(normalBreakSecs);
                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                               ProjectChangeEvent.Types.Start,
                               projectChangeEvent.NewProject,
                               "Worktimebreak & normal break",
                               new List<WorktimeRecord>()
                               {
                                    new WorktimeRecord(
                                        Handler.currentProjectSince,
                                        worktimebreakEnd,
                                        "0",
                                        "worktimebreak: " + projectChangeEvent.Message),
                                    new WorktimeRecord(
                                        worktimebreakEnd,
                                        DateTime.Now,
                                        "-1",
                                        "break: " + projectChangeEvent.Message)
                               })
                           );
                    return true;
                }
                else
                {
                    //write worktimebreak
                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                              ProjectChangeEvent.Types.Start,
                              projectChangeEvent.NewProject,
                              "Worktimebreak",
                              new WorktimeRecord(
                                  Handler.currentProjectSince,
                                  DateTime.Now,
                                  "0",
                                  "worktimebreak: " + projectChangeEvent.Message),
                              availableWorktimebreak: freeWorkbreaktime
                              )
                    );
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private int updateFreeWorktimeBreakOnUnlock()
        {
            //when coming back, subtract from available worktimebreak and return secs that do not fall in worktimebreak
            var secsPassed = (int)(DateTime.Now - Handler.currentProjectSince).TotalSeconds; //really bad, implies that lastSwitchPassedSecs is reset after calling updateFreeWorktime
            freeWorktimebreakSecs -= (int)secsPassed;
            var notInWorkbreakSecs = Math.Min(0, freeWorktimebreakSecs) * -1;
            freeWorktimebreakSecs = Math.Max(0, freeWorktimebreakSecs);
            return notInWorkbreakSecs;
        }

        private void updateFreeWorktimeBreakOnProjectChange()
        {
            //when working on screen, add gained seconds to total available worktimebreak 
            var secsPassed = (int)(DateTime.Now - Handler.currentProjectSince).TotalSeconds; //really bad, implies that lastSwitchPassedSecs is reset after calling updateFreeWorktime
            var currentWorktimebreakSecs = WorktimeSecsToWorktimebreakSecs(secsPassed);

            double factor = CountAsWorktimebreakMins / 60.0;
            var maxWorktimebreakSecs = (int)System.Math.Ceiling(CarryOverWorktimeCountHours * 60.0 * 60.0 * factor); //todo round

            freeWorktimebreakSecs += currentWorktimebreakSecs;
            freeWorktimebreakSecs = Math.Min(maxWorktimebreakSecs, freeWorktimebreakSecs);
        }

        private TimeSpan getCurrentFreeWorktimebreak() //TODO merge with updateFreeWorktimebreakOnProjectChange
        {
            var secsPassed = (int)(DateTime.Now - Handler.currentProjectSince).TotalSeconds; //really bad, implies that lastSwitchPassedSecs is reset after calling updateFreeWorktime
            var currentWorktimebreakSecs = WorktimeSecsToWorktimebreakSecs(secsPassed);

            double factor = CountAsWorktimebreakMins / 60.0;
            var maxWorktimebreakSecs = (int)System.Math.Ceiling(CarryOverWorktimeCountHours * 60.0 * 60.0 * factor); //todo round

            var currentFreeWorktimebreakSecs = freeWorktimebreakSecs + currentWorktimebreakSecs;
            currentFreeWorktimebreakSecs = Math.Min(maxWorktimebreakSecs, currentFreeWorktimebreakSecs);
            return TimeSpan.FromSeconds(currentFreeWorktimebreakSecs);
        }

        private int WorktimeSecsToWorktimebreakSecs(long worktime)
        {
            double factor = CountAsWorktimebreakMins / 60.0;
            return (int)System.Math.Ceiling(worktime * factor); //TODO round
        }
    }
}
