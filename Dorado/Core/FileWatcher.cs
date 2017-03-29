using Dorado.Core.Logger;
using Dorado.Core.Threading;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dorado.Core
{
    /// <summary>
    /// 目录监控，检测文件有无变化（通用类）
    /// </summary>
    internal sealed class DirectoryWatcher
    {
        private RwLocker filesLock;
        private Dictionary<string, EventHandler> files;
        private List<string> pendingFileReloads;
        private int changeFileDelay;
        private string directory;

        public DirectoryWatcher(string directory, int changeDelay)
        {
            filesLock = new RwLocker();
            files = new Dictionary<string, EventHandler>();
            this.directory = directory;
            pendingFileReloads = new List<string>();
            changeFileDelay = changeDelay;
            InitWatcher(directory);
        }

        /// <summary>
        /// 只在 DirectoryWatcher创建时执行一次
        /// </summary>
        /// <param name="directory"></param>
        private void InitWatcher(string directory)
        {
            FileSystemWatcher scareCrow = new FileSystemWatcher();
            scareCrow.Path = directory;
            scareCrow.IncludeSubdirectories = false;
            scareCrow.NotifyFilter = NotifyFilters.Attributes;

            scareCrow.Changed += scareCrow_Changed;
            scareCrow.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="delegateMethod">委托(string filePath,Empty), 文件地址用小写</param>
        internal void AddFile(string fileName, EventHandler delegateMethod)
        {
            fileName = fileName.ToLower();

            using (filesLock.Write())
            {
                if (!files.ContainsKey(fileName))
                    files.Add(fileName, delegateMethod);
            }
        }

        private EventHandler GetEventHandler(string fileName)
        {
            fileName = fileName.ToLower();
            EventHandler handler;

            using (filesLock.Read())
            {
                files.TryGetValue(fileName, out handler);
            }
            return handler;
        }

        private bool ContainsFile(string fileName)
        {
            fileName = fileName.ToLower();
            bool contains;

            using (filesLock.Read())
            {
                contains = files.ContainsKey(fileName);
            }
            return contains;
        }

        private void scareCrow_Changed(object sender, FileSystemEventArgs e)
        {
            string fileName = e.Name.ToLower();

            using (filesLock.Upgrade())
            {
                if (pendingFileReloads.Contains(fileName) || !ContainsFile(fileName))
                    return;

                pendingFileReloads.Add(fileName);
            }
            CountdownTimer timer = new CountdownTimer();
            timer.BeginCountdown(changeFileDelay, DelayedProcessFileChanged, fileName);
        }

        public void ProcessFileChanged(string fileName)
        {
            EventHandler delegateMethod = GetEventHandler(fileName);
            if (delegateMethod != null)
            {
                try
                {
                    string filePath = directory + "\\" + fileName;
                    delegateMethod(filePath, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("FileWatcher", ex);
                }
            }
        }

        private void DelayedProcessFileChanged(IAsyncResult ar)
        {
            string fileName = (string)ar.AsyncState;

            using (filesLock.Write())
            {
                pendingFileReloads.Remove(fileName);
            }

            //文件与处理器一对一!!
            ProcessFileChanged(fileName);
        }
    }

    public class FileWatcher
    {
        private object dirsLock;
        private Dictionary<string, DirectoryWatcher> directories;

        private static FileWatcher instance = new FileWatcher();

        public static FileWatcher Instance
        {
            get
            {
                return instance;
            }
        }

        protected FileWatcher()
        {
            dirsLock = new object();
            directories = new Dictionary<string, DirectoryWatcher>();
        }

        public void AddFile(string filePath, EventHandler handler, int changeFileDelay = 5000)
        {
            string dir = Path.GetDirectoryName(filePath).ToLower();
            string fileName = Path.GetFileName(filePath).ToLower();
            DirectoryWatcher watcher;
            lock (dirsLock)
            {
                if (!directories.TryGetValue(dir, out watcher))
                {
                    watcher = new DirectoryWatcher(dir, changeFileDelay);
                    directories.Add(dir, watcher);
                }
            }
            watcher.AddFile(fileName, handler);
        }

        public void ProcessFileChanged(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath).ToLower();
            string fileName = Path.GetFileName(filePath).ToLower();
            DirectoryWatcher watcher;
            lock (dirsLock)
            {
                if (!directories.TryGetValue(dir, out watcher))
                    return;
            }
            watcher.ProcessFileChanged(fileName);
        }
    }
}