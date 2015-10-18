using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using WebSocketSharp.Server;

namespace uomap_client
{
    public partial class MainForm : Form
    {
        private WebSocketServer wss;

        private const int Port = 27555;
        private const string ProfileDirectory = "Profiles";

        public static List<GameWindow> windows = new List<GameWindow>();
        
        public MainForm()
        {
            InitializeComponent();

            LoadProfiles();

            var updateTimer = new Timer {Interval = 500};
            updateTimer.Tick += UpdateClients;
            updateTimer.Start();

            wss = new WebSocketServer ("ws://127.0.0.1:27555");
            wss.AddWebSocketService<WSHandler> ("/");
            wss.ReuseAddress = true;
            wss.Start();
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

        public void UpdateClients(object sender, EventArgs e)
        {
            var closedWindows = new List<GameWindow>();
            var statuses = new List<String>();

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

                if(!characterListBox.Items.Contains(window))
                {
                    characterListBox.Items.Add(window);
                }

                var index = characterListBox.Items.IndexOf(window);

                if(window.Invalidated)
                {
                    characterListBox.RefreshItem(index);
                }

                if (window.IsActive)
                    characterListBox.SetSelected (index, true);
                else
                    characterListBox.SetSelected (index, false);

                if (window.Moved)
                    statuses.Add(window.ToJson());
            }
            
            foreach(var window in closedWindows)
            {
                windows.Remove(window);
                characterListBox.Items.Remove(window);
            }

            if(windows.Count <= 0)
            {
                Text = "uomap";
            }                                             

            if (statuses.Count > 0)
            {
                wss.WebSocketServices ["/"].Sessions.Broadcast ("[" + string.Join (",", statuses.ToArray ()) + "]");
            }
        }

        private void openMapButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://uomap.net/dev/map.html");
        }
    }

    public class WSHandler : WebSocketBehavior
    {
        public WSHandler()
        {
            IgnoreExtensions = true;
        }


        protected override void OnOpen()
        {
            var statuses = new List<String>();

            foreach (var window in MainForm.windows)
            {
                if (!window.IsInitialized || window.ClientClosed)
                    continue;

                statuses.Add(window.ToJson());
            }

            Send("[" + string.Join(",", statuses.ToArray()) + "]");
        }
    }
}
