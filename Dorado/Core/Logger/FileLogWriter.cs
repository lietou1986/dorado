using Dorado.Core.GlobalTimer;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;

namespace Dorado.Core.Logger
{
    /// <summary>
    /// 用于提供FileLogWriter所使用的名称
    /// </summary>
    public interface IFileLogWriterPathStrategy
    {
        /// <summary>
        /// 获取一个新的日志文件路径
        /// </summary>
        /// <returns></returns>
        string GetNewPath(string ext = ".log");
    }

    /// <summary>
    /// 以当前时间为名称提供日志文件路径的策略
    /// </summary>
    internal class TimeFileLogWriterPathStrategy : IFileLogWriterPathStrategy
    {
        protected readonly string Directory;

        public TimeFileLogWriterPathStrategy(string directory)
        {
            Contract.Requires(directory != null);

            Directory = directory;
        }

        public virtual string GetNewPath(string ext = ".log")
        {
            for (int k = 0; ; k++)
            {
                var path = Path.Combine(Directory, string.Format("{0}{1}{2}",
                    DateTime.Now.ToString(@"yyyy-MM-dd"), k == 0 ? string.Empty : ("_" + k), ext));

                if (!File.Exists(path))
                    return path;
            }
        }
    }

    /// <summary>
    /// 基于文件的日志记录策略
    /// </summary>
    /// <typeparam name="TLogItem">日志类型</typeparam>
    public class FileLogWriter<TLogItem> : ILogWriter<TLogItem>, IDisposable
        where TLogItem : ILogItem
    {
        protected readonly Object ThisLock = new Object();
        private readonly IGlobalTimerTaskHandle _taskHandle;
        protected readonly IFileLogWriterPathStrategy PathStrategy;
        protected int MaxFileSize;
        protected readonly List<TLogItem> LogItems = new List<TLogItem>();
        protected string Path;
        internal const int DefaultMaxFileSize = 100;
        protected int BufferSize = 100;//内存中缓存的日志记录数

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pathStrategy">用于提供所使用的路径</param>
        /// <param name="maxFileSize">每个日志文件的最大尺寸(MB)</param>
        public FileLogWriter(IFileLogWriterPathStrategy pathStrategy, int maxFileSize = DefaultMaxFileSize)
        {
            Contract.Requires(pathStrategy != null && maxFileSize > 0);

            PathStrategy = pathStrategy;
            MaxFileSize = maxFileSize;
            _taskHandle = Global.GlobalTimer.Add(TimeSpan.FromSeconds(5), new TaskFuncAdapter(WriteLogItems), false);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directory">日志路径</param>
        /// <param name="maxFileSize"></param>
        public FileLogWriter(string directory, int maxFileSize = DefaultMaxFileSize)
            : this(new TimeFileLogWriterPathStrategy(directory), maxFileSize)
        {
        }

        /// <summary>
        /// 批量记录日志
        /// </summary>
        /// <param name="items">日志</param>
        public void Write(IEnumerable<TLogItem> items)
        {
            if (items == null)
                return;

            lock (LogItems)
            {
                foreach (TLogItem item in items)
                {
                    LogItems.Add(item);
                }
            }
            if (LogItems.Count > BufferSize)
                WriteLogItems();
        }

        protected virtual void WriteLogItems()
        {
            if (LogItems.Count == 0)
                return;

            try
            {
                string logs;
                lock (LogItems)
                {
                    logs = string.Join("\r\n", LogItems.Select(v => v.ToString())) + "\r\n";
                    LogItems.Clear();
                }

                lock (ThisLock)
                {
                    if (Path == null || _GetFileSize(Path) >= MaxFileSize)
                        Path = PathStrategy.GetNewPath();

                    string directory = System.IO.Path.GetDirectoryName(Path);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                }
                File.AppendAllText(Path, logs, Encoding.UTF8);
            }
            catch
            {
                // ignored
            }
        }

        protected static long _GetFileSize(string path)
        {
            if (!File.Exists(path))
                return 0;

            return new FileInfo(path).Length / 1024 / 1024;
        }

        #region IDisposable Members

        ~FileLogWriter()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _taskHandle.Dispose();
            WriteLogItems();
        }

        #endregion IDisposable Members
    }
}