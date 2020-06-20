using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTracker.Util;

namespace ProjectTracker
{
    public class WorktimeRecordStorageInMemory : IWorktimeRecordStorage
    {
        List<WorktimeRecord> wtrs = new List<WorktimeRecord>();

        public IList<WorktimeRecord> worktimeRecords { get {return wtrs; } }

        public void handleProjectChangeEvent(ProjectChangeEvent projectChangeEvent)
        {
            if (projectChangeEvent == null)
                return;

            foreach (var wtr in projectChangeEvent.WorktimeRecords)
            {
                addWorktimeRecord(wtr);
            }
        }

        public void addWorktimeRecord(WorktimeRecord worktimeRecord)
        {
            if (worktimeRecord == null)
                return;

            wtrs.Add(worktimeRecord);
            wtrs.Last().storageID = wtrs.Count - 1;
        }

        public List<WorktimeRecord> getAllWorktimeRecords(DateTime from, DateTime to)
        {
            return wtrs.Where(wtr => (wtr.Start >= from && wtr.End <= to)).ToList();
        }

        public void ChangeStartTime(int id, DateTime newStartDate)
        {
            if (id >= wtrs.Count)
                throw new Exception("This WorktimeRecord does not exist: Index out of bounds");

            var current = wtrs.Where(wtr => wtr.storageID == id).FirstOrDefault();

            if (current.Start.Date != newStartDate.Date)
                throw new Exception("Only times (not dates, i.e. overnighters) can be changed");

            if (newStartDate > current.End)
                throw new Exception("Begin time cannot be greater than end time");

            if (newStartDate.TimeOfDay > current.Start.TimeOfDay) //shortening
            {
                var newWtr = new WorktimeRecord(current.Start, newStartDate, "undefined", current.Comment);
                newWtr.storageID = wtrs.Count;
                var index = wtrs.FindIndex(wtr => wtr.storageID == id);
                wtrs.Insert(index, newWtr);
                current.Start = newStartDate;
            }
            else //lengthening
            {
                var minTimeOfDay = wtrs.Where(wtr => wtr.Start.Date == newStartDate.Date && wtr.Start.Hour >= 4).Min(wtr => wtr.Start);
                var firstOfDay = wtrs.Where(wtr => wtr.Start == minTimeOfDay).FirstOrDefault();

                if (current !=  firstOfDay) //in the middle
                    throw new Exception("Lengthening in the middle not implemented");
                if (newStartDate.Hour < 4)
                    throw new Exception("Day must not start before 4am");

                current.Start = newStartDate;
            }
        }

        public void ChangeEndTime(int id, DateTime newEndDate)
        {
            if (id >= wtrs.Count)
                throw new Exception("This WorktimeRecord does not exist: Index out of bounds");

            var current = wtrs.Where(wtr => wtr.storageID == id).FirstOrDefault();

            if (current.End.Date != newEndDate.Date)
                throw new Exception("Only times (not dates, i.e. overnighters) can be changed");

            if (newEndDate < current.Start)
                throw new Exception("Begin time cannot be greater than end time");

            if (newEndDate.TimeOfDay < current.End.TimeOfDay) //shortening
            {
                var newWtr = new WorktimeRecord(newEndDate, current.End, "undefined", current.Comment);
                newWtr.storageID = wtrs.Count;
                var index = wtrs.FindIndex(wtr => wtr.storageID == id);
                wtrs.Insert(index+1, newWtr);
                current.End = newEndDate;
            }
            else //lengthening
            {
                throw new Exception("Lengthening not implemented");
            }
        }

        public void ChangeProjectName(int id, string newProjectName)
        {
            if (id >= wtrs.Count)
                throw new Exception("This WorktimeRecord does not exist: Index out of bounds");

            var current = wtrs.Where(wtr => wtr.storageID == id).FirstOrDefault();

            current.ProjectName = newProjectName;
        }

        public void ChangeProjectComment(int id, string newProjectComment)
        {
            if (id >= wtrs.Count)
                throw new Exception("This WorktimeRecord does not exist: Index out of bounds");

            var current = wtrs.Where(wtr => wtr.storageID == id).FirstOrDefault();

            current.Comment = newProjectComment;
        }

        public void updateOvertimes(Dictionary<string, TimeSpan> overtimes)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, TimeSpan> getOvertimes()
        {
            throw new NotImplementedException();
        }

        public List<String> getRecentProjects()
        {
            return new List<String>();
        }

        public List<string> getRecentComments(string projectName)
        {
            return new List<String>();
        }
    }
}
