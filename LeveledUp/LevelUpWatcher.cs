using System;
using System.Collections.Generic;
using System.IO;

namespace LeveledUp
{
    public class LevelUpWatcher
    {
        private readonly List<FileSystemWatcher> _watchers;
        public event EventHandler<FileSystemEventArgs> OnFileChange;
        private FileChange _lastFileChange;//used to prevent handling same event twice

        public LevelUpWatcher()
        {
            _watchers = new List<FileSystemWatcher>();
        }

        public void Start(string directory, string filters)
        {
            foreach (var filter in filters.Split('|'))
            {
                var watcher = new FileSystemWatcher
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
                _watchers.Add(watcher);
            }
        }

        public void Stop()
        {
            foreach (var watcher in _watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _watchers.Clear();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (_lastFileChange != null && e.FullPath == _lastFileChange.FileName && _lastFileChange.Time.AddSeconds(1) > DateTime.Now)
                return; //duplicate event, ignore it
            _lastFileChange = new FileChange { Time = DateTime.Now, FileName = e.FullPath };

            if (OnFileChange != null)
            {
                OnFileChange(this, e);
            }
        }
    }
}
