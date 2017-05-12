/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/7 10:42:38
 * 版本号：v1.0
 * 本类主要用途描述：同步状态枚举
 *  -------------------------------------------------------------------------*/

#region using

using System.ComponentModel;

#endregion using

namespace Dorado.VWS.Model.Enum
{
    /// <summary>
    ///     同步状态枚举
    /// </summary>
    public enum EnumSyncStatus
    {
        /// <summary>
        ///     同步中
        /// </summary>
        [Description("同步中")]
        Running = 1,

        /// <summary>
        ///     同步挂起
        /// </summary>
        [Description("同步挂起")]
        Suspend = 2,

        /// <summary>
        ///     同步成功
        /// </summary>
        [Description("同步成功")]
        Succeed = 3,

        /// <summary>
        ///     同步失败
        /// </summary>
        [Description("同步失败")]
        Failed = 4,

        /// <summary>
        ///     同步被回滚
        /// </summary>
        [Description("同步被回滚")]
        Rollback = 5,

        /// <summary>
        ///     同步回滚失败
        /// </summary>
        [Description("同步回滚失败")]
        RollbackFailed = 6
    }
}