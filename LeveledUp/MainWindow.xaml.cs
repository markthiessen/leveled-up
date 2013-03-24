using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using Fleck;

namespace LeveledUp
{
    public partial class MainWindow : Window
    {

        readonly LevelUpWatcher _watcher = new LevelUpWatcher();
        private IDisposable _server;

        public MainWindow()
        {
            InitializeComponent();
            SetupTray();

            _watcher.OnFileChange += _watcher_OnFileChange;
        }

        private void SetupTray()
        {
            NotifyIcon ni = new NotifyIcon();
            ni.Icon = new System.Drawing.Icon("mushroom.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }

        void _watcher_OnFileChange(object sender, EventArgs e)
        {
            WriteMessage("Change detected");
            WriteMessage("Notifying clients...");

            SendNotificationMessage("LeveledUp");
        }

        private void WriteMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogBox.AppendText(message + Environment.NewLine);
            });
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                FolderBox.Text = dialog.SelectedPath;
        }

        private bool running = false;
        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!running)
            {
                if (string.IsNullOrWhiteSpace(FolderBox.Text) || string.IsNullOrWhiteSpace(FileTypeFilterBox.Text))
                {
                    WriteMessage("Folder && File Types cannot be empty...");
                    return;
                }

                _watcher.Start(FolderBox.Text.Trim(), FileTypeFilterBox.Text.Trim());
                if (_server == null)
                    StartNotificationServer();
                running = true;
                StartStopButton.Content = "Stop";
            }
            else
            {
                _watcher.Stop();
                running = false;
                StartStopButton.Content = "Start";
            }
        }

        private WebSocketServer server;
        private List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        private void StartNotificationServer()
        {
            server = new WebSocketServer("ws://localhost:9797");
            server.Start(socket =>
                {
                    socket.OnOpen = () =>
                        {
                            WriteMessage("Open!");
                            allSockets.Add(socket);
                        };
                    socket.OnClose = () =>
                        {
                            WriteMessage("Close!");
                            allSockets.Remove(socket);
                        };

                    socket.OnMessage = message => socket.Send(message);
                });
            const string url = "http://localhost:9797";

            
            WriteMessage(string.Format("Server running on {0}", url));
        }


        private void SendNotificationMessage(string message)
        {
            if (server != null)
                foreach (var socket in allSockets)
                {
                    socket.Send(message);
                }
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_server != null)
                _server.Dispose();
        }
    }
}
