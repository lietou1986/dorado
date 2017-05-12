/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2014/6/23 10:10:11
 * 作者：han
 * 联系方式：shanfeng.han@dorado.com
 * 本类主要用途描述：同步任务返回实体（Linux）
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;

#endregion using

namespace Dorado.VWS.Utils
{
    /// <summary>
    ///     同步任务返回实体
    /// </summary>
    [Serializable]
    public class TaskResultForLinuxEntity
    {
        /// <summary>
        ///     当前IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        ///     任务ID
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        ///     任务指令
        /// </summary>
        public EnumTaskCmd TaskCmd { get; set; }

        /// <summary>
        ///     完成结果
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        ///     错误信息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        ///     用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     客户端版本信息
        /// </summary>
        public string ClientVersion { get; set; }

        /// <summary>
        ///     客户端主机名
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        ///     IIS状态
        /// </summary>
        public int IISStatus
        {
            get;
            set;
        }

        /// <summary>
        ///     相关服务的状态
        /// </summary>
        public int RelateSvcStatus { get; set; }

        /// <summary>
        ///     文件列表信息
        /// </summary>
        public List<VwsDirectoryInfo> FileList { get; set; }

        /// <summary>
        ///     是否启用HTML压缩
        /// </summary>
        public bool EnableHtmlCompress { get; set; }

        /// <summary>
        ///     是否启用JS及CSS压缩
        /// </summary>
        public bool EnableJSCssCompress { get; set; }

        /// <summary>
        /// 所有文件名列表
        /// </summary>
        public List<string> AllFileList { get; set; }

        /// <summary>
        /// 域名类型
        /// </summary>
        public int DomainType { get; set; }

        /// <summary>
        /// Linux服务类型
        /// </summary>
        public int OperatePathType { get; set; }

        /// <summary>
        /// 服务操作路径
        /// </summary>
        public string OperatePath { get; set; }
    }
}