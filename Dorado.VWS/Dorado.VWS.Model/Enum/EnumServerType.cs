/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/5 14:01:21
 * 版本号：v1.0
 * 本类主要用途描述：服务器类型枚举
 *  -------------------------------------------------------------------------*/

#region using

using System.ComponentModel;

#endregion using

namespace Dorado.VWS.Model.Enum
{
    /// <summary>
    ///     服务器类型枚举
    /// </summary>
    public enum EnumServerType
    {
        /// <summary>
        ///     同步宿
        /// </summary>
        [Description("同步宿")]
        Host = 1,

        /// <summary>
        ///     同步中继
        /// </summary>
        [Description("同步中继")]
        Relay = 2,

        /// <summary>
        ///     同步源
        /// </summary>
        [Description("同步源")]
        Source = 3
    }
}