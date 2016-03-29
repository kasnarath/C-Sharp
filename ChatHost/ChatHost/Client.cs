using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatHost
{
    class Client
    {
        public string ClientId { get; set; } = string.Empty;
        private TcpClient TCPC { get; set; } = null;
        private Thread T = null;

        public List<LogEntry> ClientLog
        {
            get;
            private set;
        } = null;

        public Client(string clientId, TcpClient tcpC)
        {
            ClientId = clientId;
            TCPC = tcpC;
            ClientLog = new List<LogEntry>();

            AddToClientLog(string.Format("Client created: {0}: {1}", ClientId, TCPC.ToString()));
            AddToClientLog("Starting client thread...");

            T = new Thread(ClientThread);
            T.IsBackground = true;
            T.Start(TCPC);
        }

        private void AddToClientLog(string entry)
        {
            ClientLog.Add(new LogEntry(entry));
        }

        private void IncommingMessage(string message)
        {
            AddToClientLog(string.Format("=> {0}", message));
        }

        public void ClientThread(object TCPClient)
        {
            AddToClientLog("Client thread started.");
            Encoding Enc = Encoding.Unicode;
            Socket S = ((TcpClient)TCPClient).Client;
            
            byte[] Receivebuffer = new byte[200];
            byte[] TransmitBuffer = Enc.GetBytes("?UserID");

            AddToClientLog("Requesting UserID");
            S.Send(TransmitBuffer);
            while (S.Connected)
            {
                if (S.Receive(Receivebuffer) > 0)
                {
                    IncommingMessage(Enc.GetString(Receivebuffer));
                }
            }
        }
    }
}
