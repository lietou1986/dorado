using System.ComponentModel;

namespace Dorado.VWS.Model.Enum
{
    /// <summary>
    ///     操作类型枚举
    /// </summary>
    public enum EnumFWOperateType
    {
        /// <summary>
        ///     新增文件
        /// </summary>
        [Description("新增文件")]
        Add = 1,

        /// <summary>
        ///     回滚操作
        /// </summary>
        [Description("更新文件")]
        Mod = 2,

        /// <summary>
        ///     服务控制操作
        /// </summary>
        [Description("删除文件")]
        Del = 3,
    }
}