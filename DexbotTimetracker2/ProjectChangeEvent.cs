using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeEvent : EventArgs
    {
        public enum Types { Init, Change, GoodMorning };

        public Types Type { get; }
        public string MessageHeader { get; }
        public string MessageText { get; }
        public WorktimeRecord WorktimeRecord { get; }

        public ProjectChangeEvent(Types type, string messageHeader, string messageText, WorktimeRecord wtr)
        {
            Type = type;
            MessageHeader = messageHeader;
            MessageText = messageText;
            WorktimeRecord = wtr;
        }

        public override string ToString()
        {
            return String.Format("Type: {0}, Header: {1}, Text: {2}, WorktimeRecord: {3}",
                Type.ToString(), MessageHeader, MessageText, (WorktimeRecord != null) ? WorktimeRecord.ToString() : "");
        }
    }
}
