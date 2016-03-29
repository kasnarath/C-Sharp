using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ChatHost
{
    public partial class Form1 : Form
    {
        List<LogEntry> GlobalLog = new List<LogEntry>();
        BindingList<Client> Clients = new BindingList<Client>();

        public delegate void RefreshBindingSourceDelegate();
        public delegate void GlobalLogAddDelegate(string entry);
        delegate void ServerThreadDelegate(bool bringOnline);
        delegate void ServerThreadFeedBackDelegate(string onlineButtonText, string formCaption);
        delegate void ServerThreadAddClient(Client C);

        Thread ServerThread = null;
        bool KillServerThread = false;
        BindingSource bs = new BindingSource();
        public RefreshBindingSourceDelegate RefreshLog = null;
        public GlobalLogAddDelegate GlobalLogAdd = null;

        public Form1()
        {
            InitializeComponent();
            toolStripComboBoxPort.SelectedItem = "1337";
            dataGridViewClients.DataSource = Clients;
            SetupServerThread();
            bs.DataSource = GlobalLog;
            dataGridViewLog.DataSource = bs;
            RefreshLog = new RefreshBindingSourceDelegate(RefreshBindingSource);
            GlobalLogAdd = new GlobalLogAddDelegate(AddToGlobalLog);
        }

        private void SetupServerThread()
        {
            ServerThread = new Thread(ChangeSeverState);
            ServerThread.Name = "Listener thread";
            ServerThread.IsBackground = true;
        }

        private void AddToGlobalLog(string entry)
        {
            GlobalLog.Add(new LogEntry(entry));
        }

        private void toolStripButtonServerState_Click(object sender, EventArgs e)
        {
            if (!ServerThread.IsAlive)
            {
                KillServerThread = false;
                SetupServerThread();
                ServerThread.Start(int.Parse(toolStripComboBoxPort.SelectedItem.ToString()));
                toolStripButtonServerState.Text = "Online";
            }
            else
            {
                KillServerThread = true;
                if (ServerThread.Join(1000))
                {
                    toolStripButtonServerState.Text = "Offline";
                }
                else
                {
                    MessageBox.Show("Thread refuses to die!");
                }
            }
        }

        private void ServerThreadFeedBack(string onlineButtonText, string formCaption)
        {
            if (onlineButtonText != null)
            {
                toolStripButtonServerState.Text = onlineButtonText;
            }
            if (formCaption != null)
            {
                this.Text = formCaption;
            }
        }

        private void RefreshBindingSource()
        {
            bs.ResetBindings(true);
            dataGridViewLog.Columns[0].Width = 100;
            dataGridViewLog.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void AddClient(Client C)
        {
            Clients.Add(C);
        }

        private void ChangeSeverState(object Port)
        {
            ServerThreadFeedBackDelegate FB = new ServerThreadFeedBackDelegate(ServerThreadFeedBack);
            ServerThreadAddClient AC = new ServerThreadAddClient(AddClient);

            TcpListener Listener = TcpListener.Create((int)Port); // 1863 is the MSN messenger port            
            Listener.Start();

            this.Invoke(FB, null, string.Format("Host, Listening at {0}...", Listener.Server.LocalEndPoint.ToString()));
            
            Encoding Enc = Encoding.Unicode;
            while (!KillServerThread)
            {
                if (Listener.Pending())
                {
                    TcpClient C = Listener.AcceptTcpClient();
                    this.Invoke(AC, new Client(C.Client.LocalEndPoint.ToString(), C,this));
                }
                else
                {   // no pending connections; Go to sleep
                    Thread.Sleep(100); 
                }    
            }
        }

        private void LoadLog(List<LogEntry> log)
        {
            if (bs.DataSource != log)
            {
                bs.DataSource = log;
                RefreshBindingSource();
            }
        }

        private void dataGridViewClients_SelectionChanged(object sender, EventArgs e)
        {
            if(dataGridViewClients.SelectedRows != null)
            {
                LoadLog(Clients[dataGridViewClients.CurrentRow.Index].ClientLog);
            }
            else
            {
                LoadLog(GlobalLog);
            }
        }

        private void toolStripButtonGlobalLogLoad_Click(object sender, EventArgs e)
        {
            dataGridViewClients.ClearSelection();
            LoadLog(GlobalLog);
        }
    }
}