using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeEvent : EventArgs
    {
        public enum Types { Init, Start, Finish, Change, GoodMorning };

        public Types Type { get; }
        public string Message { get; }
        public Boolean Processed { get; set; }
        public WorktimeRecord WorktimeRecord { get; }

        public ProjectChangeEvent(Types type, string message, WorktimeRecord wtr, Boolean processed = false)
        {
            Type = type;
            Message = message;
            WorktimeRecord = wtr;
            Processed = processed; 
        }

        public ProjectChangeEvent(ProjectChangeEvent pce)
            : this(pce.Type, pce.Message, new WorktimeRecord(pce.WorktimeRecord), pce.Processed) { }

        public override string ToString()
        {
            return String.Format("Type: {0} ({1}), Message: {2}, WorktimeRecord: {3}",
                Type.ToString(),
                (Processed) ? "processed" : "not processed", 
                Message, 
                (WorktimeRecord != null) ? WorktimeRecord.ToString() : "");
        }
    }
}
