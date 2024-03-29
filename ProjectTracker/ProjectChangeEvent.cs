﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    public class ProjectChangeEvent : EventArgs
    {
        public enum Types { Test, Init, Exit, Start, Finish, Change, GoodMorning, Lock, Unlock };

        public Types Type { get; set; }
        public string NewProject { get; set; }
        public string NewComment { get; set; }
        public TimeSpan AvailableWorktimebreak { get; set; }
        public Boolean Processed { get; set; }
        public List<WorktimeRecord> WorktimeRecords { get; set; } = new List<WorktimeRecord>(); //RTODO this list should be ordered according to .end
        public WorktimeRecord WorktimeRecord { 
            get { return (WorktimeRecords.Count > 0) ? WorktimeRecords.ElementAt(0) : null; } 
            set { WorktimeRecords[0] = value; } 
        }
        
        public ProjectChangeEvent(Types type, string newProject, string newComment, List<WorktimeRecord> wtrs, Boolean processed = false, TimeSpan availableWorktimebreak = new TimeSpan())
        {
            Type = type;
            NewProject = newProject;
            NewComment = newComment;
            AvailableWorktimebreak = availableWorktimebreak;
            WorktimeRecords = wtrs;
            Processed = processed; 
        }

        public ProjectChangeEvent(Types type, string newProject, string newComment, WorktimeRecord wtr, Boolean processed = false, TimeSpan availableWorktimebreak = new TimeSpan())
            : this(type, newProject, newComment, new List<WorktimeRecord>(), processed, availableWorktimebreak)
        {
            WorktimeRecords.Add(wtr);
        }

        public ProjectChangeEvent(ProjectChangeEvent pce)
            : this(pce.Type, pce.NewProject, pce.NewComment, new List<WorktimeRecord>(), pce.Processed, pce.AvailableWorktimebreak)
        {
            foreach (var wtr in pce.WorktimeRecords)
            {
                WorktimeRecords.Add(new WorktimeRecord(wtr));
            }
        }

        public override string ToString()
        {
            var outString = String.Format("Type: {0} ({1}), New Pj: {2}, New Comment: {3}: FreeWTB: {4}",
                Type.ToString(),
                (Processed) ? "processed" : "not processed",
                NewProject,
                NewComment,
                AvailableWorktimebreak.TotalSeconds.ToString());
            if (WorktimeRecords.Count() == 0)
            {
                outString += ", WorktimeRecord: [none]";
            }
            else if (WorktimeRecords.Count == 1)
            {
                outString += ", WorktimeRecord - " + WorktimeRecord.ToString();
            }
            else
            {
                foreach (var wtr in WorktimeRecords)
                {
                    outString += String.Format("\n    WorktimeRecord: {0}", wtr.ToString() );
                }
            }
            return outString;
        }
    }
}
