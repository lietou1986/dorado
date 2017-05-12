using System.ComponentModel;

namespace Dorado.VWS.Model.Enum
{
    /// <summary>
    ///     域名服务器类型枚举
    /// </summary>
    public enum EnumDomainType
    {
        /// <summary>
        /// 域名服务器类型-未知
        /// </summary>
        [Description("其他")]
        other = 0,

        /// <summary>
        /// 域名服务器类型-Windows
        /// </summary>
        [Description("Windows")]
        Windows = 1,

        /// <summary>
        /// 域名服务器类型-Linux
        /// </summary>
        [Description("Linux")]
        Linux = 2,
    }

    /// <summary>
    ///     域名服务器类型枚举
    /// </summary>
    public enum EnumOperatePathType
    {
        /// <summary>
        /// 域名服务器类型-未知
        /// </summary>
        [Description("其他")]
        other = 0,

        /// <summary>
        /// Linux环境下Jar包
        /// </summary>
        [Description("Jar")]
        Jar = 1,

        /// <summary>
        /// Linux环境下Jar包
        /// </summary>
        [Description("Linux")]
        Tomcat = 2,
    }
}