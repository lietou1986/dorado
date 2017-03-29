using Dorado.Core;

namespace Dorado
{
    /// <summary>
    /// 全局对象
    /// </summary>
    internal static class Global
    {
        /// <summary>
        /// 全局定时器
        /// </summary>
        public static readonly GlobalTimer<ITask> GlobalTimer = GlobalTimer<ITask>.Default;
    }
}