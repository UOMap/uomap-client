using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace uomap_client
{
    public partial class MainForm : Form
    {
        private TcpListener listener;       

        private const int Port = 27555;
        private const string ProfileDirectory = "Profiles";

        List<GameWindow> windows = new List<GameWindow>();
        
        public MainForm()
        {
            InitializeComponent();

            LoadProfiles();

            var updateTimer = new Timer {Interval = 1000};
            updateTimer.Tick += UpdateClients;
            updateTimer.Start();

            listener = new TcpListener(IPAddress.Loopback, Port);
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptCallback, listener);
        }

        public void LoadProfiles()
        {
            if (!Directory.Exists(ProfileDirectory))
            {
                Directory.CreateDirectory(ProfileDirectory);
            }

            var files = Directory.GetFiles(ProfileDirectory);
            foreach (var file in files)
            {
                
            }
        }

        public void UpdateClients(object sender, System.EventArgs e)
        {
            List<GameWindow> closedWindows = new List<GameWindow>();

            Game.FindWindows(windows);

            foreach(var window in windows)
            {
                if(!window.IsInitialized)
                {
                    Game.Initialize(window);
                }

                Game.UpdateClient(window);

                if(window.ClientClosed)
                {
                    closedWindows.Add(window);
                    continue;
                }                

                /*if(window.IsActiveWindow)
                {*/
                    this.Text = window.Character.ToString();
               // }

                if(!characterListBox.Items.Contains(window.Character))
                {
                    characterListBox.Items.Add(window.Character);
                }

                if(window.Character.Invalidated)
                {
                    var index = characterListBox.Items.IndexOf(window.Character);
                    characterListBox.RefreshItem(index);
                }
            }
            
            foreach(var window in closedWindows)
            {
                windows.Remove(window);
                characterListBox.Items.Remove(window.Character);
            }

            if(windows.Count <= 0)
            {
                this.Text = "uomap";
            }                                             
        }

        void AcceptCallback(IAsyncResult ar)
        {
            var o = new StateObject();

            // Get the socket that handles the client request.
            var server = (TcpListener)ar.AsyncState;
            var client = server.EndAcceptTcpClient(ar);

            server.BeginAcceptTcpClient(AcceptCallback, server);

            o.Client = client;

            var networkStream = client.GetStream();

            o.Stream = networkStream;
            networkStream.BeginRead(o.Buffer, 0, o.Buffer.Length, ReadCallback, o);
        }
        
        public class StateObject
        {
            public TcpClient Client = null;
            public NetworkStream Stream = null;
            public const int BufferSize = 1024;
            public byte[] Buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();
        }

        public void ReadCallback(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            state.Stream.EndRead(ar);
            state.sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, state.Buffer.Length));

            if (state.Stream.CanWrite)
            {
                var output = JsonFormatter.BuildJson(windows);

                var writeBuffer = Encoding.ASCII.GetBytes(output);
                state.Stream.BeginWrite(writeBuffer, 0, writeBuffer.Length, WriteCallback, state);

                state.Stream.Flush();
                state.Stream.Close();                
            }
        }

        public void WriteCallback(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            state.Stream.EndWrite(ar);
            state.Stream.Flush();
            state.Stream.Close();
        }

        
    }
}
