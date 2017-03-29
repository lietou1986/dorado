using System.Collections.Generic;

namespace Dorado.Core.Logger
{
    /// <summary>
    /// 日志记录策略
    /// </summary>
    /// <typeparam name="TLogItem"></typeparam>
    public interface ILogWriter<in TLogItem>
        where TLogItem : ILogItem
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="items"></param>
        void Write(IEnumerable<TLogItem> items);
    }
}