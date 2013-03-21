using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Owin.Hosting;

namespace LeveledUp
{
    public partial class MainWindow : Window
    {

        readonly LevelUpWatcher _watcher = new LevelUpWatcher();
        private bool serverRunning = false;
        private IDisposable _server;

        public MainWindow()
        {
            InitializeComponent();
            _watcher.OnFileChange += _watcher_OnFileChange;
        }

        void _watcher_OnFileChange(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                LogBox.AppendText("Change detected" + Environment.NewLine);
                LogBox.AppendText("Notifying clients..." + Environment.NewLine);
            }));
            LevelUpHub.SendMessage("LeveledUp");
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                FolderBox.Text = dialog.SelectedPath;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _watcher.Stop();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _watcher.Start(FolderBox.Text, FileTypeFilterBox.Text);
            if(_server==null)
                StartNotificationServer();
        }

        private void StartNotificationServer()
        {
            string url = "http://localhost:9797";
            _server = WebApplication.Start<LevelUpStartup>(url);
            LogBox.AppendText(string.Format("Server running on {0}", url) + Environment.NewLine);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if(_server!=null)
                _server.Dispose();
        }
    }
}
