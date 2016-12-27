using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class WorktimeRecord
    {
        public DateTime Start { get; }
        public DateTime End { get; }
        public string ProjectName { get; }
        public string Comment { get; }

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
    }
}
