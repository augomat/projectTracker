using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    interface IWorktimeRecordStorage
    {
        void addWorktimeRecord(WorktimeRecord worktimeRecord);
        void handleProjectChangeEvent(ProjectChangeEvent projectChangeEvent);
        List<WorktimeRecord> getAllWorktimeRecords(DateTime day);
        List<WorktimeRecord> getAllWorktimeRecords(DateTime from, DateTime to);
        void ChangeStartTime(int id, DateTime newStartDate);
        void ChangeEndTime(int id, DateTime newEndDate);
        void ChangeProjectName(int id, string newProjectName);
        void ChangeProjectComment(int id, string newProjectComment);
    }
}
