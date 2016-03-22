using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ChatHost
{
    class Client
    {
        public string ClientId { get; set; } = string.Empty;
        public IPAddress ClientIP { get; set; } = IPAddress.None;
        public List<LogEntry> ClientLog
        {
            get;
            private set;
        } = null;

        public Client(string clientId, IPAddress clientIP)
        {
            ClientId = clientId;
            ClientIP = clientIP;
            ClientLog = new List<LogEntry>();
            ClientLog.Add(new LogEntry(DateTime.Now, string.Format("Client created: {0} @ {1}", ClientId, ClientIP.ToString())));
        }
    }
}
