/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/7 10:25:26
 * 版本号：v1.0
 * 本类主要用途描述：调用类型枚举
 *  -------------------------------------------------------------------------*/

#region using

using System.ComponentModel;

#endregion using

namespace Dorado.VWS.Model.Enum
{
    /// <summary>
    ///     调用类型枚举
    /// </summary>
    public enum EnumCallType
    {
        /// <summary>
        ///     网站调用
        /// </summary>
        [Description("网站调用")]
        WebSite = 1,

        /// <summary>
        ///     接口调用
        /// </summary>
        [Description("接口调用")]
        Interface = 2,

        /// <summary>
        ///     定时调用
        /// </summary>
        [Description("定时调用")]
        Timmer = 3
    }
}