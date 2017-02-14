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
        void addProjectChangeEvent(ProjectChangeEvent projectChangeEvent);
        List<WorktimeRecord> getAllWorktimeRecords(DateTime day);
        List<WorktimeRecord> getAllWorktimeRecords(DateTime from, DateTime to);
    }
}
