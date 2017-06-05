using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeProcessorNewDay : ProjectChangeProcessor
    {
        private WorktimeAnalyzer WorktimeAnalyzer;
        private WorktrackerUpdater WorktrackerUpdater;

        private bool flagAutoFinishPreviousDay { get { return Properties.Settings.Default.flagAutoFinishWT; } }

        public ProjectChangeProcessorNewDay(ProjectChangeHandler handler, 
            WorktimeAnalyzer worktimeAnalyzer,
            WorktrackerUpdater worktrackerUpdater) : base(handler)
        {
            WorktimeAnalyzer = worktimeAnalyzer;
            WorktrackerUpdater = worktrackerUpdater;
        }

        public override bool process(ProjectChangeEvent projectChangeEvent)
        {
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Unlock && isNewDay(Handler.currentProjectSince))
            {
                if (flagAutoFinishPreviousDay)
                    AutoUpdateDay(DateTime.Now.Date.AddDays(-1));

                //Hack: Just replace event data instead of refiring, because the old event is per definition invalid
                projectChangeEvent.Type = ProjectChangeEvent.Types.GoodMorning;
                projectChangeEvent.Message = "Good Morning";
                projectChangeEvent.WorktimeRecord = new WorktimeRecord(
                                   DateTime.Now,
                                   DateTime.Now,
                                   projectChangeEvent.NewProject,
                                   "New day begun");
                return false;
            }
            return false;
        }

        private void AutoUpdateDay(DateTime dayToFinish)
        {
            try
            {
                var projectStatistics = WorktimeAnalyzer.AnalyzeWorkday(dayToFinish);
                WorktrackerUpdater.updateProjectEntries(dayToFinish, projectStatistics);
            }
            catch (Exception)
            {
                //TODO swallow - there is currently no sane way we can propagate this error out for showing e.g
                //a notification bubble as this is certainly not a projectChangeEvent
            }
            
        }

        private static bool isNewDay(DateTime lastSwitched)
        {
            var TodayAt4am = DateTime.Now.Date + new TimeSpan(4, 0, 0);
            return lastSwitched < TodayAt4am && TodayAt4am < DateTime.Now;
        }
    }
}
