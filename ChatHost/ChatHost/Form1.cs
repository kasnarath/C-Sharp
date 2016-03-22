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
        delegate void ServerThreadDelegate(bool bringOnline);
        delegate void ServerThreadFeedBackDelegate(string onlineButtonText, string formCaption);
        delegate void ServerThreadAddClient(Client C);

        public Form1()
        {
            InitializeComponent();
            dataGridViewClients.DataSource = Clients;
        }

        private void toolStripButtonServerState_Click(object sender, EventArgs e)
        {
            Thread ServerThread = new Thread(ChangeSeverState);
            ServerThread.Name = "Server Thread";
            ServerThread.IsBackground = true;
            ServerThread.Start();
        }

        private void ServerThreadFeedBack(string onlineButtonText, string formCaption)
        {
            if (onlineButtonText != "")
            {
                toolStripButtonServerState.Text = onlineButtonText;
            }
            if (formCaption != "")
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
            bool OnlineState = false;
            ServerThreadFeedBackDelegate FB = new ServerThreadFeedBackDelegate(ServerThreadFeedBack);
            ServerThreadAddClient AC = new ServerThreadAddClient(AddClient);

            //Socket Host = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TcpListener Listener = TcpListener.Create(1863); // The MSN messenger port
            while (OnlineState)
            {
                
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
            if(dataGridViewClients.SelectedRows[0] != null)
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
            LoadLog(GlobalLog);
        }
    }
}