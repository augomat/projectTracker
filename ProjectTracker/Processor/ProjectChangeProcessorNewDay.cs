﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTracker.Util;

namespace ProjectTracker
{
    class ProjectChangeProcessorNewDay : AProjectChangeProcessor
    {
        private WorktimeAnalyzer WorktimeAnalyzer;
        private WorktrackerUpdater WorktrackerUpdater;

        private bool flagAutoFinishPreviousDay { get { return Properties.Settings.Default.flagAutoFinishWT; } }
        private bool flagConsiderOvertime { get { return Properties.Settings.Default.flagConsiderOvertime;  } }

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
                    AutoUpdateDay(Handler.currentProjectSince.Date);

                //Hack: Just replace event data instead of refiring, because the old event is per definition invalid
                projectChangeEvent.Type = ProjectChangeEvent.Types.GoodMorning;
                projectChangeEvent.WorktimeRecord = new WorktimeRecord(
                                   DateTime.Now,
                                   DateTime.Now,
                                   projectChangeEvent.NewProject,
                                   "New day begun");
                return false;
            }
            return false;
        }

        private void AutoUpdateDay(DateTime day)
        {
            try
            {
                var projectStatistics = WorktimeAnalyzer.AnalyzeWorkday(day);

                //in case the user forgot to log out
                try { WorktrackerUpdater.finishDay(day, Handler.currentProjectSince, projectStatistics.totalPausetime); } catch { }

                if (flagConsiderOvertime)
                {
                    var projectStatisticsAdapted = WorktimeAnalyzer.considerOvertimeUndertime(projectStatistics);
                    WorktrackerUpdater.updateFullDay(day, projectStatisticsAdapted, Handler.currentProjectSince); //unfortunately if something fails here, the overtime-db was updated anyways
                    WorktrackerUpdater.updateProjectEntries(day, projectStatisticsAdapted);
                }
                else
                {
                    WorktrackerUpdater.updateProjectEntries(day, projectStatistics);
                    WorktrackerUpdater.updateBreak(day, projectStatistics.totalPausetime);
                }  
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
