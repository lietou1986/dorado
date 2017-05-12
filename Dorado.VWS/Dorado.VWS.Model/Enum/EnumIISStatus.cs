/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/5 14:24:28
 * 版本号：v1.0
 * 本类主要用途描述：服务器IIS状态枚举
 *  -------------------------------------------------------------------------*/

#region using

using System.ComponentModel;

#endregion using

namespace Dorado.VWS.Model.Enum
{
    /// <summary>
    ///     服务器IIS状态枚举
    /// </summary>
    public enum EnumIISStatus
    {
        /// <summary>
        ///     运行中
        /// </summary>
        [Description("运行中")]
        Running = 1,

        /// <summary>
        ///     停止
        /// </summary>
        [Description("停止状态")]
        Stop = 2,

        /// <summary>
        ///     未知
        /// </summary>
        [Description("未知状态")]
        UnKnow = 3
    }
}