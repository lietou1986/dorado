namespace Dorado.Core.Logger
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 普通信息
        /// </summary>
        Info = 1,

        /// <summary>
        /// 警告
        /// </summary>
        Warn,

        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 跟踪
        /// </summary>
        Trace,

        /// <summary>
        /// 调试
        /// </summary>
        Debug
    }
}