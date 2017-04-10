using System;
using System.Diagnostics.Contracts;

namespace Dorado.Core.Logger
{
    /// <summary>
    /// 日志记录的辅助工具
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="error"></param>
        /// <param name = "args"></param>
        public static void Error(this Logger<LogItem> logger, string title, string error, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Error, string.Format(error, args), title));
        }

        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="error"></param>
        public static void Error(this Logger<LogItem> logger, string title, Exception error)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Error, error.ToString(), title));
        }

        public static void Error(this Logger<LogItem> logger, Exception error, string title, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Error, error.ToString(), string.Format(title, args)));
        }

        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="error"></param>
        /// <param name = "args"></param>
        public static void Error(this Logger<LogItem> logger, string error, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Error, string.Format(error, args), string.Empty));
        }

        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="error"></param>
        public static void Error(this Logger<LogItem> logger, Exception error)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Error, error.ToString(), string.Empty));
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="info"></param>
        /// <param name = "args"></param>
        public static void Info(this Logger<LogItem> logger, string title, string info, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Info, string.Format(info, args), title));
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="info"></param>
        /// <param name = "args"></param>
        public static void Info(this Logger<LogItem> logger, string info, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Info, string.Format(info, args), string.Empty));
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="info"></param>
        /// <param name = "args"></param>
        public static void Info(this Logger<LogItem> logger, string title, Exception info)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Info, info.ToString(), title));
        }

        /// <summary>
        /// 记录跟踪
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="trace"></param>
        /// <param name = "args"></param>
        public static void Trace(this Logger<LogItem> logger, string title, string trace, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Trace, string.Format(trace, args), title));
        }

        /// <summary>
        /// 记录跟踪
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="trace"></param>
        /// <param name = "args"></param>
        public static void Trace(this Logger<LogItem> logger, string trace, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Trace, string.Format(trace, args), string.Empty));
        }

        /// <summary>
        /// 记录跟踪
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="trace"></param>
        /// <param name = "args"></param>
        public static void Trace(this Logger<LogItem> logger, string title, Exception trace)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Trace, trace.ToString(), title));
        }

        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="warn"></param>
        /// <param name = "args"></param>
        public static void Warn(this Logger<LogItem> logger, string title, string warn, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Warn, string.Format(warn, args), title));
        }

        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="warn"></param>
        /// <param name = "args"></param>
        public static void Warn(this Logger<LogItem> logger, string warn, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Warn, string.Format(warn, args), string.Empty));
        }

        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="warn"></param>
        /// <param name = "args"></param>
        public static void Warn(this Logger<LogItem> logger, string title, Exception warn)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Warn, warn.ToString(), title));
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="debug"></param>
        /// <param name = "args"></param>
        public static void Debug(this Logger<LogItem> logger, string title, string debug, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Debug, string.Format(debug, args), title));
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="debug"></param>
        /// <param name = "args"></param>
        public static void Debug(this Logger<LogItem> logger, string debug, params object[] args)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Debug, string.Format(debug, args), string.Empty));
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="title"></param>
        /// <param name="debug"></param>
        /// <param name = "args"></param>
        public static void Debug(this Logger<LogItem> logger, string title, Exception debug)
        {
            Contract.Requires(logger != null);

            LoggerWrapper.Logger.Log(new LogItem(LogType.Debug, debug.ToString(), title));
        }
    }
}