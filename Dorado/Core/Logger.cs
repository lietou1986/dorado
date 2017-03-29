using Dorado.Core.Logger;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Dorado.Core
{
    public static class LoggerWrapper
    {
        private static Logger<LogItem> _logger = new Logger<LogItem>(AppSettings.LogPath);

        public static Logger<LogItem> Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }
    }

    /// <summary>
    /// 日志记录器
    /// </summary>
    /// <typeparam name="TLogItem"></typeparam>
    public class Logger<TLogItem>
        where TLogItem : ILogItem
    {
        public ILogWriter<TLogItem> LogWriter
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logWriter"></param>
        public Logger(ILogWriter<TLogItem> logWriter)
        {
            Contract.Requires(logWriter != null);

            LogWriter = logWriter;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logDirectory">日志记录目录</param>
        /// <param name="maxFileSize">日志文件的最大尺寸</param>
        public Logger(string logDirectory, int maxFileSize = FileLogWriter<TLogItem>.DefaultMaxFileSize)
            : this(new FileLogWriter<TLogItem>(logDirectory, maxFileSize))
        {
        }

        /// <summary>
        /// 记录一条日志
        /// </summary>
        /// <param name="item"></param>
        public void Log(TLogItem item)
        {
            Contract.Requires(item != null);
            Log(new[] { item });
        }

        /// <summary>
        /// 批量记录日志
        /// </summary>
        /// <param name="items"></param>
        public void Log(IEnumerable<TLogItem> items)
        {
            Contract.Requires(items != null);
            LogWriter.Write(items);
        }
    }
}