using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Diagnostics;
using System.Reflection;

namespace LeveledUp
{
    public partial class MainWindow : Window
    {
        private readonly WatcherSettings _settings;
        private readonly LevelUpWatcher _watcher;
        private NotifyIcon _notifyIcon;
        private readonly MessageServer _server;

        public MainWindow()
        {
            KillOtherInstances();

            InitializeComponent();
            DwmDropShadow.DropShadowToWindow(this);

            //load settings and set UI fields
            _settings = WatcherSettings.Load();
            FolderBox.Text = _settings.FolderToWatch;
            FileTypeFilterBox.Text = _settings.WatchedFileTypes;
            CommandBox.Text = _settings.Command;

            StartTrayIcon();

            _watcher = new LevelUpWatcher();
            _watcher.OnFileChange += _watcher_OnFileChange;

            //make window draggable
            MouseDown += WindowMouseDown;

            _server = new MessageServer();
            _server.OnConnectionOpen += _server_OnConnectionOpen;
            _server.OnConnectionClosed += _server_OnConnectionClosed;

            ExtensionIcon.PreviewMouseDown += ExtensionIcon_PreviewMouseDown;
        }

        private void KillOtherInstances()
        {
            foreach (var process in Process.GetProcessesByName("LeveledUp"))
            {
                if(process.Id!=Process.GetCurrentProcess().Id)
                    process.Kill();
            }
        }

        void ExtensionIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string path = Path.Combine(Path.GetDirectoryName(
                                            Assembly.GetExecutingAssembly()
                                                    .Location
                                            ), "chrome_extension.crx");
            string[] files = new[] { path };
            string dataFormat = System.Windows.DataFormats.FileDrop;
            var dataObject = new System.Windows.DataObject(dataFormat, files);
            DragDrop.DoDragDrop(this.ExtensionIcon, dataObject, System.Windows.DragDropEffects.Copy);
        }


        void _server_OnConnectionOpen(object sender, EventArgs e)
        {
            WriteMessage("Client connected.");
        }

        void _server_OnConnectionClosed(object sender, EventArgs e)
        {
            WriteMessage("Connection closed.");
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    DragMove();
                }
                catch (Exception)
                {}
            }
        }

        private void StartTrayIcon()
        {
            _notifyIcon = new NotifyIcon { Icon = Properties.Resources.mushroom, Visible = true };
            _notifyIcon.DoubleClick +=
                (sender, args) =>
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }

        void _watcher_OnFileChange(object sender, FileSystemEventArgs e)
        {

            WriteMessage("Change detected");

            if (!string.IsNullOrWhiteSpace(_settings.Command))
            {
                WriteMessage("Running Command " + _settings.Command);

                var p = new Process
                            {
                                StartInfo =
                                    {
                                        FileName = "CMD.exe",
                                        CreateNoWindow = true,
                                        Arguments = "/C " + _settings.Command,
                                        WorkingDirectory = _settings.FolderToWatch,
                                        WindowStyle = ProcessWindowStyle.Hidden
                                    }
                            };

                p.Start();
                p.WaitForExit();
            }
            WriteMessage("Notifying clients...");

            _server.SendNotificationMessage("LeveledUp:"+ Path.GetExtension(e.FullPath));
        }

        private void WriteMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogBox.AppendText(Environment.NewLine + message);
                LogBox.ScrollToEnd();
            });
        }

        private void OpenFolderButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                FolderBox.Text = dialog.SelectedPath;
        }

        private void StartStopButtonClick(object sender, RoutedEventArgs e)
        {
            if (!_server.IsRunning)
            {

                if (string.IsNullOrWhiteSpace(FolderBox.Text) || string.IsNullOrWhiteSpace(FileTypeFilterBox.Text))
                {
                    WriteMessage("Folder && File Types cannot be empty...");
                    return;
                }

                _settings.FolderToWatch = FolderBox.Text.Trim();
                _settings.WatchedFileTypes = FileTypeFilterBox.Text.Trim();
                _settings.Command = CommandBox.Text.Trim();

                _watcher.Start(_settings.FolderToWatch, _settings.WatchedFileTypes);

                const string serverUrl = "wss://localhost:9797";
                _server.Start(serverUrl);

                WriteMessage(string.Format("Server running on {0}", serverUrl));
                WriteMessage(string.Format("Now start the browser extension by clicking the mushroom icon on the page you want to auto-refresh."));
                StartStopButton.Content = "Stop";
                _settings.Save();
            }
            else
            {
                _watcher.Stop();
                _server.Stop();
                StartStopButton.Content = "Start";
                WriteMessage("Server stopped...");
            }
        }
        
        protected override void OnClosed(EventArgs e)
        {
            _notifyIcon.Dispose();
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
