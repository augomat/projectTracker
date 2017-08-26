using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    public interface IWorktimeRecordStorage
    {
        void addWorktimeRecord(WorktimeRecord worktimeRecord);
        void handleProjectChangeEvent(ProjectChangeEvent projectChangeEvent);
        List<WorktimeRecord> getAllWorktimeRecords(DateTime day);
        List<WorktimeRecord> getAllWorktimeRecords(DateTime from, DateTime to);
        void ChangeStartTime(int index, DateTime newStartDate);
        void ChangeEndTime(int index, DateTime newStartDate);
        void ChangeProjectName(int index, string newProjectName);
        void ChangeProjectComment(int index, string newProjectComment);
        void updateOvertimes(Dictionary<string, TimeSpan> overtimes);
        Dictionary<string, TimeSpan> getOvertimes();
    }
}
