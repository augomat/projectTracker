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
        public Boolean Processed { get; }
        public WorktimeRecord WorktimeRecord { get; }

        public ProjectChangeEvent(Types type, string message, WorktimeRecord wtr, Boolean processed = false)
        {
            Type = type;
            Message = message;
            WorktimeRecord = wtr;
            Processed = processed; 
        }

        public override string ToString()
        {
            return String.Format("Type: {0}, Message: {1}, WorktimeRecord: {2}",
                Type.ToString(), Message, (WorktimeRecord != null) ? WorktimeRecord.ToString() : "");
        }
    }
}
