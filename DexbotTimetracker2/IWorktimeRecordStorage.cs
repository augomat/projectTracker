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
        List<WorktimeRecord> getAllWorktimeRecords(DateTime day);
    }
}
