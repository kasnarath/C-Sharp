using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;

namespace ChatHost
{
    class Client
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientIP { get; set; } = string.Empty;
        private TcpClient TCPC { get; set; } = null;
        private Thread T = null;
        private static Form1 HostForm = null;
        public List<LogEntry> ClientLog
        {
            get;
            private set;
        } = null;

        public Client(string clientId, TcpClient tcpC, Form1 hostForm)
        {
            HostForm = hostForm;
            ClientId = clientId.TrimEnd('\0');
            ClientIP = tcpC.Client.LocalEndPoint.ToString();// as IPEndPoint;
            TCPC = tcpC;
            ClientLog = new List<LogEntry>();

            AddToClientLog(string.Format("Client created: {0}", ClientId));
            AddToClientLog("Starting client thread...");

            T = new Thread(ClientThread);
            T.IsBackground = true;
            T.Start(TCPC);
        }

        private void AddToClientLog(string entry)
        {
            ClientLog.Add(new LogEntry(entry));
            HostForm.Invoke(HostForm.RefreshLog);
            HostForm.Invoke(HostForm.GlobalLogAdd, string.Format("{0}: {1}",this.ClientId.TrimEnd('\0'), entry));
        }

        private void IncommingMessage(string message)
        {
            AddToClientLog(string.Format("=> {0}", message));
            ParseMessage(message);
        }

        private void ParseMessage(string message)
        {
            if(message.Substring(0,1) == "=") // Request reply
            {
                int pos = message.IndexOf(':');
                switch (message.Substring(1, pos - 1).ToLower())
                {
                    case "userid": // User ID reply
                        ClientId = message.Substring(pos + 1).TrimEnd('\0');
                        break;
                    default:
                        AddToClientLog(string.Format("Answer: {0} not understood", message));
                        break;
                }  
            }
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
