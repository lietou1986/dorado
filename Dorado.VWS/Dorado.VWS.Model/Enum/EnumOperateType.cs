/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/7 10:06:23
 * 版本号：v1.0
 * 本类主要用途描述：操作类型枚举
 *  -------------------------------------------------------------------------*/

#region using

using System.ComponentModel;

#endregion using

namespace Dorado.VWS.Model.Enum
{
    /// <summary>
    ///     操作类型枚举
    /// </summary>
    public enum EnumOperateType
    {
        /// <summary>
        ///     同步操作
        /// </summary>
        [Description("同步操作")]
        Sync = 1,

        /// <summary>
        ///     回滚操作
        /// </summary>
        [Description("回滚操作")]
        RollBack = 2,

        /// <summary>
        ///     服务控制操作
        /// </summary>
        [Description("服务控制操作")]
        ServerControl = 3,

        /// <summary>
        ///     服务器管理操作
        /// </summary>
        [Description("服务器管理操作")]
        ServerManage = 4,

        /// <summary>
        ///     权限管理操作
        /// </summary>
        [Description("权限管理操作")]
        PermissionManage = 5,

        /// <summary>
        ///     计划任务操作
        /// </summary>
        [Description("计划任务操作")]
        ScheduleTask = 6,

        /// <summary>
        ///     图片操作
        /// </summary>
        [Description("图片同步操作")]
        ImageSync = 7,

        /// <summary>
        ///     系统权限管理操作
        /// </summary>
        [Description("权限管理操作")]
        SystemPermissionManage = 8,
    }
}