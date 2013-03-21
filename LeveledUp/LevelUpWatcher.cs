using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LeveledUp
{
    public class LevelUpWatcher
    {
        private FileSystemWatcher watcher = new FileSystemWatcher();
        public event EventHandler OnFileChange;


        public void Start(string directory, string filter)
        {
            watcher.EnableRaisingEvents = false;

            watcher.Path = directory;
            watcher.IncludeSubdirectories = true;
            
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            
            watcher.Filter = filter;

            watcher.Changed += OnChanged;
            
            // Begin watching.
            watcher.EnableRaisingEvents = true;

        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (OnFileChange !=null)
            {
                OnFileChange(this, new EventArgs());
            }
        }
    }
}
