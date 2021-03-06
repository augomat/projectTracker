﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace ProjectTracker
{
    public class WorktimeRecord : IStorageClass
    {
        [BsonId]
        public int storageID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string ProjectName { get; set; }
        public string Comment { get; set; }

        public WorktimeRecord() { } //needed for LiteDB
        public WorktimeRecord(DateTime start, DateTime end, string projectName, string comment)
        {
            Start = start;
            End = end;
            ProjectName = projectName;
            Comment = comment;
        }

        public WorktimeRecord(WorktimeRecord wtr)
            : this(wtr.Start, wtr.End, wtr.ProjectName, wtr.Comment) { }

        public override string ToString()
        {
            return String.Format("Start: {0}, End: {1}, Project: {2}, Comment: {3}",
                Start.ToString(), End.ToString(), ProjectName, Comment);
        }

        public override bool Equals(object obj)
        {
            var cmp = (WorktimeRecord)obj;

            return (cmp.Start == this.Start
                && cmp.End == this.End
                && cmp.ProjectName == this.ProjectName
                && cmp.Comment == this.Comment);
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode() ^ ProjectName.GetHashCode() ^ Comment.GetHashCode();
        }
    }
}
