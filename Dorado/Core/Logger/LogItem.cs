using Dorado.Extensions;
using System;
using System.Text;

namespace Dorado.Core.Logger
{
    /// <summary>
    /// 日志项
    /// </summary>
    public class LogItem : ILogItem
    {
        public LogItem(LogType type, string message, string title)
        {
            Type = type;
            Message = message ?? string.Empty;
            Time = CommonExtension.QuickGetTime();
            Title = title;
        }

        #region ILogItem Members

        /// <summary>
        /// 日志类型
        /// </summary>
        public LogType Type { get; private set; }

        /// <summary>
        /// 日志标记
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// 日志记录时间
        /// </summary>
        public DateTime Time { get; private set; }

        #endregion ILogItem Members

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0}]-[{1}]|{2} {3}", Time.ToString("yyyy-MM-dd HH:mm:ss"), Type, Title, Message);
            return sb.ToString();
        }
    }
}