/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2014/6/23 10:10:11
 * 作者：han
 * 联系方式：shanfeng.han@dorado.com
 * 本类主要用途描述：同步任务实体（Linux）
 *  -------------------------------------------------------------------------*/

using System;

namespace Dorado.VWS.Utils
{
    /// <summary>
    ///     同步任务实体
    /// </summary>
    [Serializable]
    public class TaskSenderForLinuxEntity
    {
        /// <summary>
        ///     任务Id
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        ///     同步计划任务Id
        /// </summary>
        public int SyncTaskId { get; set; }

        /// <summary>
        ///     任务指令
        /// </summary>
        public EnumTaskCmd TaskCmd { get; set; }

        /// <summary>
        ///     Windows服务名称
        /// </summary>
        public string WinServiceName { get; set; }

        /// <summary>
        ///     IIS配置的网站名称
        /// </summary>
        public string IISSiteName { get; set; }

        /// <summary>
        ///     源服务器IP
        /// </summary>
        public string SourceIP { get; set; }

        /// <summary>
        ///     源服务器文件根目录（绝对路径）
        /// </summary>
        public string SourceRoot { get; set; }

        /// <summary>
        ///     目标服务器IP
        /// </summary>
        public string TargetIP { get; set; }

        /// <summary>
        ///     目标服务器文件目录（绝对路径）
        /// </summary>
        public string TargetRoot { get; set; }

        /// <summary>
        ///     备份目录
        /// </summary>
        public string BackupRoot { get; set; }

        /// <summary>
        ///     需要添加的文件（文件之间以","隔开）
        /// </summary>
        public string AddList { get; set; }

        /// <summary>
        ///     需要删除的文件（文件之间以","隔开）
        /// </summary>
        public string DelList { get; set; }

        /// <summary>
        ///     文件压缩类型
        /// </summary>
        public EnumCompresssType CompressType { get; set; }

        /// <summary>
        ///     任务发起人
        /// </summary>
        public string UserName { get; set; }

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