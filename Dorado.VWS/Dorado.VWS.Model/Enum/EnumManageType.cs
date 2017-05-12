/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/11/16 16:25:26
 * 版本号：v1.0
 * 本类主要用途描述：系统管理权限类型定义（将部分运维权限下放到用户层）
 *  -------------------------------------------------------------------------*/

using System.ComponentModel;

namespace Dorado.VWS.Model.Enum
{
    /// <summary>
    /// 系统运维权限定义
    /// </summary>
    public enum EnumManageType
    {
        /// <summary>
        ///     日常运维
        /// </summary>
        [Description("日常运维")]
        DailyManage = 1,

        /// <summary>
        ///     服务控制
        /// </summary>
        [Description("服务控制")]
        ServiceControl = 2,

        /// <summary>
        ///     列表同步
        /// </summary>
        [Description("列表同步")]
        SyncByList = 3,

        /// <summary>
        ///     全部回滚
        /// </summary>
        [Description("全部回滚")]
        RollBackALL = 4,

        /// <summary>
        ///     文件删除
        /// </summary>
        [Description("文件删除")]
        FileDelete = 5,

        /// <summary>
        /// 计划任务
        /// </summary>
        [Description("计划任务")]
        ScheduleTask = 6
    }

    public enum SysytemRoleEnumType
    {
        /// <summary>
        ///     超级管理员
        /// </summary>
        [Description("超级管理员")]
        SuperAdmin = 1,

        /// <summary>
        ///     开发
        /// </summary>
        [Description("开发")]
        Dev = 2,

        /// <summary>
        /// 测试
        /// </summary>
        [Description("测试")]
        Test = 3,

        /// <summary>
        /// 运维
        /// </summary>
        [Description("运维")]
        Ops = 4
    }
}