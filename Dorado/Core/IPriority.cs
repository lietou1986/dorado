using System;

namespace Dorado.Core
{
    /// <summary>
    /// 支持对象的优先级
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPriority<out T>
        where T : IComparable<T>
    {
        /// <summary>
        /// 获取优先级
        /// </summary>
        /// <returns></returns>
        T GetPriority();
    }
}