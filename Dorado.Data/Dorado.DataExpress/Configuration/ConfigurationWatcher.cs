using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Dorado.DataExpress.Configuration
{
    public class ConfigurationWatcher : IDisposable
    {
        public delegate void FilesChangedEventHandler(object sender, List<FileSystemEventArgs> events);

        private const int DEFAULT_DELAY = 2000;
        private readonly object _eventLock = new object();
        private List<FileSystemEventArgs> _events = new List<FileSystemEventArgs>();
        private readonly Timer _delayTmr;
        private readonly FileSystemWatcher _watcher;
        private bool _isDisposing;

        public event ConfigurationWatcher.FilesChangedEventHandler OnFilesChanged;

        public List<FileSystemEventArgs> Events
        {
            get
            {
                return this._events;
            }
            set
            {
                this._events = value;
            }
        }

        public NotifyFilters NotifyFilter
        {
            get
            {
                return this._watcher.NotifyFilter;
            }
            set
            {
                this._watcher.NotifyFilter = value;
            }
        }

        public string Path
        {
            get
            {
                return this._watcher.Path;
            }
            set
            {
                this._watcher.Path = value;
            }
        }

        public string Filter
        {
            get
            {
                return this._watcher.Filter;
            }
            set
            {
                this._watcher.Filter = value;
            }
        }

        public int DefaultDelay
        {
            get;
            set;
        }

        public ConfigurationWatcher()
        {
            this.DefaultDelay = 2000;
            this._delayTmr = new Timer(new TimerCallback(this.OnTimer), null, -1, -1);
            this._watcher = new FileSystemWatcher();
            this._watcher.IncludeSubdirectories = false;
            this._watcher.Changed += new FileSystemEventHandler(this.OnFileChanged);
            this._watcher.Created += new FileSystemEventHandler(this.OnFileChanged);
            this._watcher.Deleted += new FileSystemEventHandler(this.OnFileChanged);
            this._watcher.Renamed += new RenamedEventHandler(this.OnFileRenamed);
        }

        public ConfigurationWatcher(string path)
            : this()
        {
            this.Path = path;
        }

        public ConfigurationWatcher(string path, string filter)
            : this()
        {
            this.Path = path;
            this.Filter = filter;
        }

        public void Start()
        {
            this._watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            this._watcher.EnableRaisingEvents = false;
        }

        private void AddEvents(FileSystemEventArgs e)
        {
            object eventLock;
            Monitor.Enter(eventLock = this._eventLock);
            try
            {
                this.Events.Add(e);
            }
            finally
            {
                Monitor.Exit(eventLock);
            }
        }

        public virtual void OnTimer(object arg)
        {
            if (this.OnFilesChanged != null)
            {
                this.OnFilesChanged(this, this.Events);
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            this.AddEvents(e);
            this._delayTmr.Change(this.DefaultDelay, -1);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            this.AddEvents(e);
            this._delayTmr.Change(this.DefaultDelay, -1);
        }

        public void Dispose()
        {
            if (this._isDisposing)
            {
                return;
            }
            this._isDisposing = true;
            this._delayTmr.Dispose();
            this._watcher.Dispose();
        }

        ~ConfigurationWatcher()
        {
            this.Dispose();
        }
    }
}