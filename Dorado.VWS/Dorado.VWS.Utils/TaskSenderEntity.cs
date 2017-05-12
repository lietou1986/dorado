/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：同步任务实体
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.ComponentModel;

#endregion using

namespace Dorado.VWS.Utils
{
    /// <summary>
    ///     同步任务实体
    /// </summary>
    [Serializable]
    public class TaskSenderEntity
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
        ///     任务指令（适应新版java版，暂不确定作用）
        /// </summary>
        public EnumTaskCmd CustomCmd { get; set; }

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

    /// <summary>
    ///     任务命令枚举
    /// </summary>
    public enum EnumTaskCmd
    {
        /// <summary>
        ///     测试
        /// </summary>
        [Description("测试")]
        HELLO,

        /// <summary>
        ///     获取文件列表
        /// </summary>
        [Description("获取文件列表")]
        GETFILELIST,

        /// <summary>
        ///     更新客户端
        /// </summary>
        [Description("更新客户端")]
        UPDATECLIENT,

        /// <summary>
        ///     停止IIS
        /// </summary>
        [Description("停止IIS")]
        IISSTOP,

        /// <summary>
        ///     启动IIS
        /// </summary>
        [Description("启动IIS")]
        IISSTART,

        /// <summary>
        ///     停止windows服务
        /// </summary>
        [Description("停止windows服务")]
        WINSERVICESTOP,

        /// <summary>
        ///     启动windows服务
        /// </summary>
        [Description("启动windows服务")]
        WINSERVICESTART,

        /// <summary>
        ///     同步文件
        /// </summary>
        [Description("同步文件")]
        SYNCFILES,

        /// <summary>
        ///     失败后回滚文件
        /// </summary>
        [Description("失败后回滚文件")]
        ROLLBACKFILES,

        /// <summary>
        ///     确认提交并删除临时备份
        /// </summary>
        [Description("确认提交并删除临时备份")]
        COMMITFILES,

        /// <summary>
        ///     备份文件
        /// </summary>
        [Description("备份文件")]
        BACKUPFILES,

        /// <summary>
        ///     压缩demo文件
        /// </summary>
        [Description("压缩demo文件")]
        COMPRESSFILES,

        /// <summary>
        ///     回滚demo文件
        /// </summary>
        [Description("回滚demo文件")]
        REVERTFILES,

        /// <summary>
        ///     检查文件是否存在(demo)
        /// </summary>
        [Description("测试")]
        CHECKFILELIST,

        /// <summary>
        ///     获取文件流
        /// </summary>
        [Description("获取文件流")]
        GETFILEBYTES,

        /// <summary>
        ///     发送文件流
        /// </summary>
        [Description("发送文件流")]
        SENDFILEBYTES,

        #region 雷斌添加

        /// <summary>
        ///     重新启动IIS
        /// </summary>
        [Description("重新启动IIS")]
        IISRESTART,

        /// <summary>
        ///     重新启动windows服务
        /// </summary>
        [Description("重新启动windows服务")]
        WINSERVICERESTART,

        /// <summary>
        ///     获取文件列表但不获取MD5值
        /// </summary>
        [Description("获取文件列表但不获取MD5值")]
        GetFileListNoMd5,

        /// <summary>
        ///     递归获取文件列表但不获取MD5值
        /// </summary>
        [Description("递归获取文件列表但不获取MD5值")]
        GETALLFILENAME

        #endregion 雷斌添加
    }

    /// <summary>
    ///     文件压缩类型枚举
    /// </summary>
    public enum EnumCompresssType
    {
        /// <summary>
        ///     停止压缩
        /// </summary>
        [Description("停止压缩")]
        NoCommpress = 1,

        /// <summary>
        ///     Html文件压缩
        /// </summary>
        [Description("Html文件压缩")]
        HtmlCompress = 2,

        /// <summary>
        ///     JsCss文件压缩
        /// </summary>
        [Description("JsCss文件压缩")]
        JsCssCompress = 3,

        /// <summary>
        ///     HtmlJsCss文件压缩
        /// </summary>
        [Description("HtmlJsCss文件压缩")]
        HtmlJsCssCompress = 4
    }

    /// <summary>
    ///     IIS操作
    /// </summary>
    public enum EnumIISOperate
    {
        /// <summary>
        ///     启动
        /// </summary>
        [Description("Start")]
        Start = 2,

        /// <summary>
        ///     停止
        /// </summary>
        [Description("Stop")]
        Stop = 4,

        /// <summary>
        ///     暂停
        /// </summary>
        [Description("Pause")]
        Pause = 6,

        /// <summary>
        ///     重新启动
        /// </summary>
        [Description("ReStart")]
        ReStart = 8
    }
}