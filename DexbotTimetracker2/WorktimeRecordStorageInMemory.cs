using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class WorktimeRecordStorageInMemory : IWorktimeRecordStorage
    {
        IList<WorktimeRecord> wtrs;
        public IList<WorktimeRecord> worktimeRecords { get { return wtrs; } }

        public WorktimeRecordStorageInMemory(IList<WorktimeRecord> worktimeRecordList)
        {
            //This is actually really ugly, why does my db-class need to retrieve the stupid list from outside????
            //But it appears to be that this is the easiest way to bring an IBindingList into the storage
            wtrs = worktimeRecordList;
        }

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

        public List<WorktimeRecord> getAllWorktimeRecords(DateTime from, DateTime to)
        {
            return wtrs.Where(wtr => (wtr.Start >= from && wtr.End <= to)).ToList();
        }
    }
}
