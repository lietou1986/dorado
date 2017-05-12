using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Vancl.IC.VWS.ClientService.Model
{
    /// <summary>
    /// 同步任务实体
    /// </summary>
    [Serializable]
    public class TaskEntity
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// 同步计划任务Id
        /// </summary>
        public int SyncTaskId { get; set; }

        /// <summary>
        /// 任务名
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Windows服务名称
        /// </summary>
        public string WinServiceName { get; set; }

        /// <summary>
        /// 源服务器IP
        /// </summary>
        public string SourceIP { get; set; }

        /// <summary>
        /// 源服务器文件根目录（绝对路径）
        /// </summary>
        public string SourceRoot { get; set; }

        /// <summary>
        /// 目标服务器IP
        /// </summary>
        public string TargetIP { get; set; }

        /// <summary>
        /// 目标服务器文件目录（绝对路径）
        /// </summary>
        public string TargetRoot { get; set; }

        /// <summary>
        /// 备份目录
        /// </summary>
        public string BackupRoot { get; set; }

        /// <summary>
        /// 需要添加的文件（文件之间以","隔开）
        /// </summary>
        public string AddList { get; set; }

        /// <summary>
        /// 需要删除的文件（文件之间以","隔开）
        /// </summary>
        public string DelList { get; set; }

        /// <summary>
        /// 文件压缩类型
        /// </summary>
        public EnumCompresssType CompressType { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public EnumTaskStatus TaskStatus { get; set; }
    }

    /// <summary>
    /// 任务状态枚举
    /// </summary>
    public enum EnumTaskStatus
    {
        /// <summary>
        /// 任务等待领取
        /// </summary>
        [Description("Wait")]
        Wait = 1,

        /// <summary>
        /// 任务被领取，正在处理中
        /// </summary>
        [Description("Running")]
        Running = 2,

        /// <summary>
        /// 任务结果：成功
        /// </summary>
        [Description("Successed")]
        Successed = 3,

        /// <summary>
        /// 任务结果：失败
        /// </summary>
        [Description("Failed")]
        Failed = 4,
    }

    /// <summary>
    /// 文件压缩类型枚举
    /// </summary>
    public enum EnumCompresssType
    {
        /// <summary>
        /// 不压缩
        /// </summary>
        [Description("NoCommpress")]
        NoCommpress = 1,

        /// <summary>
        /// Html文件压缩
        /// </summary>
        [Description("HtmlCompress")]
        HtmlCompress =2,

        /// <summary>
        /// JsCss文件压缩
        /// </summary>
        [Description("JsCssCompress")]
        JsCssCompress =3,

        /// <summary>
        /// HtmlJsCss文件压缩
        /// </summary>
        [Description("HtmlJsCssCompress")]
        HtmlJsCssCompress = 4
    }
}
