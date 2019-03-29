using System;
using SAMI.Logging;

namespace SAMI.Application.LogViewer
{
    internal struct LogEntry
    {
        public DateTime Date
        {
            get;
            private set;
        }

        public String Message
        {
            get;
            private set;
        }

        public LogCategory Category
        {
            get;
            private set;
        }

        public String MachineName
        {
            get;
            private set;
        }

        public LogEntry(String message, DateTime date, LogCategory category, String machineName)
            : this()
        {
            Message = message;
            Date = date;
            Category = category;
            MachineName = machineName;
        }
    }
}
