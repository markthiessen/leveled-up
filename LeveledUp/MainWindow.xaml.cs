using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Fleck;
using System.Drawing;
using System.Diagnostics;

namespace LeveledUp
{
    public partial class MainWindow : Window
    {

        readonly LevelUpWatcher _watcher = new LevelUpWatcher();
        private string _command;
        private string _folder;

        public MainWindow()
        {
            InitializeComponent();
            DwmDropShadow.DropShadowToWindow(this);
            SetupTray();

            _watcher.OnFileChange += _watcher_OnFileChange;
            this.MouseDown += Window_MouseDown;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private NotifyIcon notifyIcon;
        private void SetupTray()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Properties.Resources.mushroom;
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick +=
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

        private FileChange _lastFileChange;
        void _watcher_OnFileChange(object sender, FileSystemEventArgs e)
        {
            if (_lastFileChange != null && e.FullPath == _lastFileChange.FileName && _lastFileChange.Time.AddSeconds(1) > DateTime.Now)
                return; //duplicate event

            _lastFileChange = new FileChange { Time = DateTime.Now, FileName = e.FullPath };
            WriteMessage("Change detected");

            if (!string.IsNullOrWhiteSpace(_command))
            {
                WriteMessage("Running Command " + _command);

                var p = new Process
                            {
                                StartInfo =
                                    {
                                        FileName = "CMD.exe",
                                        CreateNoWindow = true,
                                        Arguments = "/C " + _command,
                                        WorkingDirectory = _folder,
                                        WindowStyle = ProcessWindowStyle.Hidden
                                    }
                            };

                p.Start();
                p.WaitForExit();
            }
            WriteMessage("Notifying clients...");

            SendNotificationMessage("LeveledUp");
        }

        private void WriteMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogBox.AppendText(message + Environment.NewLine);
                LogBox.ScrollToEnd();
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

                _folder = FolderBox.Text.Trim();
                _command = CommandBox.Text.Trim();

                _watcher.Start(_folder, FileTypeFilterBox.Text.Trim());
                if (server == null)
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
            if (server != null)
                server.Dispose();
        }

        protected override void OnClosed(EventArgs e)
        {
            notifyIcon.Dispose();
        }

        private void MinimizeWindowCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }
        private void CloseWindowCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);

        }
    }

    public class FileChange
    {
        public DateTime Time { get; set; }
        public string FileName { get; set; }
    }
}
