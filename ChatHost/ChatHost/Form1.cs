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
        Thread ServerThread = null;
        bool KillServerThread = false;
        delegate void ServerThreadDelegate(bool bringOnline);
        delegate void ServerThreadFeedBackDelegate(string onlineButtonText, string formCaption);
        delegate void ServerThreadAddClient(Client C);

        public Form1()
        {
            InitializeComponent();
            dataGridViewClients.DataSource = Clients;
            ServerThread = new Thread(ChangeSeverState);
            ServerThread.Name = "Listener thread";
            ServerThread.IsBackground = true;
        }

        private void toolStripButtonServerState_Click(object sender, EventArgs e)
        {
            if (!ServerThread.IsAlive)
            {
                KillServerThread = false;
                ServerThread.Start();
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

        private void AddClient(Client C)
        {
            Clients.Add(C);
        }

        private void ChangeSeverState(object state)
        {
            ServerThreadFeedBackDelegate FB = new ServerThreadFeedBackDelegate(ServerThreadFeedBack);
            ServerThreadAddClient AC = new ServerThreadAddClient(AddClient);

            TcpListener Listener = TcpListener.Create(1337); // 1863 is the MSN messenger port            
            Listener.Start();
            
            Encoding Enc = Encoding.Unicode;
            while (!KillServerThread)
            {
                if (Listener.Pending())
                {
                    TcpClient C = Listener.AcceptTcpClient();
                    this.Invoke(AC, new Client("UserPlaceholder", C));
                }
                else
                {   // no pending connections; Go to sleep
                    Thread.Sleep(100); 
                }    
            }
        }

        private void LoadLog(List<LogEntry> log)
        {
            if (dataGridViewLog.DataSource != log)
            {
                dataGridViewLog.Columns.Clear();
                dataGridViewLog.DataSource = log;
                dataGridViewLog.Columns[0].Width = 100;
                dataGridViewLog.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;  
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