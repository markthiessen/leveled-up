using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LeveledUp
{
    public class LevelUpWatcher
    {
        private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        public event EventHandler OnFileChange;


        public void Start(string directory, string filters)
        {
            foreach (var filter in filters.Split('|'))
            {
                FileSystemWatcher watcher = new FileSystemWatcher
                    {
                        EnableRaisingEvents = false,
                        Path = directory,
                        IncludeSubdirectories = true,
                        NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                       | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                        Filter = filter
                    };

                watcher.Changed += OnChanged;

                // Begin watching.
                watcher.EnableRaisingEvents = true;
                watchers.Add(watcher);
            }
        }

        public void Stop()
        {
            foreach (var watcher in watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            watchers.Clear();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (OnFileChange != null)
            {
                OnFileChange(this, new EventArgs());
            }
        }
    }
}
