using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class WorktimeRecordStorageInMemory : IWorktimeRecordStorage
    {
        List<WorktimeRecord> wtrs = new List<WorktimeRecord>();

        public void addProjectChangeEvent(ProjectChangeEvent projectChangeEvent)
        {
            if (projectChangeEvent == null)
                return;

            foreach (var wtr in projectChangeEvent.WorktimeRecords)
            {
                wtrs.Add(wtr);
            }
        }

        public void addWorktimeRecord(WorktimeRecord worktimeRecord)
        {
            if (worktimeRecord == null)
                return;

            wtrs.Add(worktimeRecord);
        }

        public List<WorktimeRecord> getAllWorktimeRecords(DateTime day)
        {
            return wtrs.Where(wtr => (wtr.Start.Date == day.Date)).ToList();
        }
    }
}
