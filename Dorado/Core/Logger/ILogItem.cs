using System;

namespace Dorado.Core.Logger
{
    /// <summary>
    /// 日志项
    /// </summary>
    public interface ILogItem
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        LogType Type { get; }

        /// <summary>
        /// 标记
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 信息
        /// </summary>
        string Message { get; }

        /// <summary>
        /// 记录时间
        /// </summary>
        DateTime Time { get; }
    }
}