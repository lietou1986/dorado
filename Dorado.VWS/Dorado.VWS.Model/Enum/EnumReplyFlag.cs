/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/7 14:16:46
 * 版本号：v1.0
 * 本类主要用途描述：服务器回复标志枚举
 *  -------------------------------------------------------------------------*/

#region using

using System.ComponentModel;

#endregion using

namespace Dorado.VWS.Model.Enum
{
    /// <summary>
    ///     服务器回复标志枚举
    /// </summary>
    public enum EnumReplyFlag
    {
        /// <summary>
        ///     无回复
        /// </summary>
        [Description("无回复")]
        No = 0,

        /// <summary>
        ///     已经回复
        /// </summary>
        [Description("已经回复")]
        Yes = 1
    }
}