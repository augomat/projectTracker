﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class WorktimeRecordStorageInMemory : IWorktimeRecordStorage
    {
        List<WorktimeRecord> wtrs = new List<WorktimeRecord>();

        public IList<WorktimeRecord> worktimeRecords { get {return wtrs; } }

        public void handleProjectChangeEvent(ProjectChangeEvent projectChangeEvent)
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

        public void ChangeStartTime(int index, DateTime newStartDate)
        {
            if (index >= wtrs.Count)
                throw new Exception("This WorktimeRecord does not exist: Index out of bounds");

            var current = wtrs[index];

            if (current.Start.Date != newStartDate.Date)
                throw new Exception("Only times (not dates) can be changed");

            if (newStartDate > current.End)
                throw new Exception("Begin time cannot be greater than end time");

            if (newStartDate.TimeOfDay > current.Start.TimeOfDay) //shortening
            {
                if (current != wtrs.First()) //in the middle
                {
                    wtrs.Insert(index, new WorktimeRecord(current.Start, newStartDate, "undefined", "Next project shortened"));
                }
                current.Start = newStartDate;
            }
            else //lengthening
            {
                if (current != wtrs.First()) //in the middle
                    throw new Exception("Lengthening in the middle not implemented");
                current.Start = newStartDate;
            }
        }

        public void ChangeEndTime(int index, DateTime newStartDate)
        {
            throw new NotImplementedException();
        }

        public void ChangeProjectName(int index, string newProjectName)
        {
            if (index >= wtrs.Count)
                throw new Exception("This WorktimeRecord does not exist: Index out of bounds");

            var current = wtrs[index];

            current.ProjectName = newProjectName;
        }

        public void ChangeProjectComment(int index, string newProjectComment)
        {
            if (index >= wtrs.Count)
                throw new Exception("This WorktimeRecord does not exist: Index out of bounds");

            var current = wtrs[index];

            current.Comment = newProjectComment;
        }
    }
}
