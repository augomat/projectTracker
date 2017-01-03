using System;
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
            var isBackFromLockscreen = projectChangeEvent.Type == ProjectChangeEvent.Types.Start;
            var isChangeFromBreak = projectChangeEvent.Type == ProjectChangeEvent.Types.Change &&
                                        (projectChangeEvent.WorktimeRecord.ProjectName == ProjectChangeHandler.PROJECT_PAUSE
                                        || projectChangeEvent.WorktimeRecord.ProjectName == ProjectChangeHandler.PROJECT_WORKTIMEBREAK);
             
            if (isBackFromLockscreen || isChangeFromBreak)
            {
                return comeBackFromOffScreen(projectChangeEvent);
            }
            if ((projectChangeEvent.Type == ProjectChangeEvent.Types.Change && !isChangeFromBreak)
                || projectChangeEvent.Type == ProjectChangeEvent.Types.Finish)
            {
                updateFreeWorktimeBreakOnProjectChange(projectChangeEvent);
                return false;
            }
            return false;
        }

        private bool comeBackFromOffScreen(ProjectChangeEvent projectChangeEvent)
        {
            var wtr = projectChangeEvent.WorktimeRecord;
            bool declaredAsWorktimebreak = (wtr.ProjectName == ProjectChangeHandler.PROJECT_WORKTIMEBREAK) ? true : false;

            //Update free Worktimebreak
            var normalBreakSecs = updateFreeWorktimeBreakOnUnlock(projectChangeEvent);

            if (declaredAsWorktimebreak)
            {
                if (normalBreakSecs > 0)
                {
                    //write worktimebreak
                    var worktimebreakEnd = projectChangeEvent.WorktimeRecord.End - TimeSpan.FromSeconds(normalBreakSecs);
                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                               ProjectChangeEvent.Types.Start,
                               projectChangeEvent.NewProject,
                               "Worktimebreak & normal break",
                               new List<WorktimeRecord>()
                               {
                                    new WorktimeRecord(
                                        Handler.currentProjectSince,
                                        worktimebreakEnd,
                                        ProjectChangeHandler.PROJECT_WORKTIMEBREAK,
                                        projectChangeEvent.Message),
                                    new WorktimeRecord(
                                        worktimebreakEnd,
                                        projectChangeEvent.WorktimeRecord.End,
                                        ProjectChangeHandler.PROJECT_PAUSE,
                                        projectChangeEvent.Message)
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
                                  projectChangeEvent.WorktimeRecord.End,
                                  ProjectChangeHandler.PROJECT_WORKTIMEBREAK,
                                  "worktimebreak: " + projectChangeEvent.Message),
                              availableWorktimebreak: freeWorkbreaktime
                              )
                    );
                    return true;
                }
            }
            else
                return false;
        }

        private int updateFreeWorktimeBreakOnUnlock(ProjectChangeEvent pce)
        {
            //when coming back, subtract from available worktimebreak and return secs that do not fall in worktimebreak
            var secsPassed = (int)(pce.WorktimeRecord.End - Handler.currentProjectSince).TotalSeconds; //really bad, implies that lastSwitchPassedSecs is reset after calling updateFreeWorktime
            freeWorktimebreakSecs -= (int)secsPassed;
            var notInWorkbreakSecs = Math.Min(0, freeWorktimebreakSecs) * -1;
            freeWorktimebreakSecs = Math.Max(0, freeWorktimebreakSecs);
            return notInWorkbreakSecs;
        }

        private void updateFreeWorktimeBreakOnProjectChange(ProjectChangeEvent pce)
        {
            if (Handler.currentProjectSince == default(DateTime))
                return;

            //when working on screen, add gained seconds to total available worktimebreak 
            var secsPassed = (int)(pce.WorktimeRecord.End - Handler.currentProjectSince).TotalSeconds; //really bad, implies that lastSwitchPassedSecs is reset after calling updateFreeWorktime
            var currentWorktimebreakSecs = WorktimeSecsToWorktimebreakSecs(secsPassed);

            double factor = CountAsWorktimebreakMins / 60.0;
            var maxWorktimebreakSecs = (int)System.Math.Ceiling(CarryOverWorktimeCountHours * 60.0 * 60.0 * factor); //todo round

            freeWorktimebreakSecs += currentWorktimebreakSecs;
            freeWorktimebreakSecs = Math.Min(maxWorktimebreakSecs, freeWorktimebreakSecs);
        }

        private TimeSpan getCurrentFreeWorktimebreak() //TODO merge with updateFreeWorktimebreakOnProjectChange
        {
            if (Handler.currentProjectSince == default(DateTime))
                return TimeSpan.FromSeconds(0);

            if (Handler.currentProject != ProjectChangeHandler.PROJECT_PAUSE
                && Handler.currentProject != ProjectChangeHandler.PROJECT_WORKTIMEBREAK)
            {
                var secsPassed = (int)(DateTime.Now - Handler.currentProjectSince).TotalSeconds; //really bad, implies that lastSwitchPassedSecs is reset after calling updateFreeWorktime
                var currentWorktimebreakSecs = WorktimeSecsToWorktimebreakSecs(secsPassed);

                double factor = CountAsWorktimebreakMins / 60.0;
                var maxWorktimebreakSecs = (int)System.Math.Ceiling(CarryOverWorktimeCountHours * 60.0 * 60.0 * factor); //todo round

                var currentFreeWorktimebreakSecs = freeWorktimebreakSecs + currentWorktimebreakSecs;
                currentFreeWorktimebreakSecs = Math.Min(maxWorktimebreakSecs, currentFreeWorktimebreakSecs);
                return TimeSpan.FromSeconds(currentFreeWorktimebreakSecs);
            }
            else
            {
                var secsPassed = (int)(DateTime.Now - Handler.currentProjectSince).TotalSeconds; //really bad, implies that lastSwitchPassedSecs is reset after calling updateFreeWorktime
                var currentFreeWorktimebreakSecs = freeWorktimebreakSecs - secsPassed;
                currentFreeWorktimebreakSecs = Math.Max(0, currentFreeWorktimebreakSecs);
                return TimeSpan.FromSeconds(currentFreeWorktimebreakSecs);
            }
        }

        private int WorktimeSecsToWorktimebreakSecs(long worktime)
        {
            double factor = CountAsWorktimebreakMins / 60.0;
            return (int)System.Math.Ceiling(worktime * factor); //TODO round
        }
    }
}
