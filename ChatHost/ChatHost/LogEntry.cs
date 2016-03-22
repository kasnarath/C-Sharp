using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHost
{
    class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Entry { get; set; }

        public LogEntry(string entry)       : this(DateTime.Now, entry) { }
        public LogEntry(DateTime timestamp) : this(timestamp, "") { }
        public LogEntry()                   : this(DateTime.Now, "") { }
        public LogEntry(DateTime timestamp, string entry)
        {
            Timestamp = timestamp;
            Entry = entry;
        }
    }
}
