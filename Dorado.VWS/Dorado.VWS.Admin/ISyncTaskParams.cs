#region using

using System;
using System.Collections.Generic;

#endregion using

namespace Dorado.VWS.Admin
{
    public interface ISyncTaskParams
    {
        SyncTaskParams Params { get; set; }
    }

    [Serializable]
    public class SyncTaskParams
    {
        /// <summary>
        ///     SyncTaskId
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        ///     DomainId
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        ///     要添加的文件列表，string:文件相对路径，int:VersionFiles中的Id(新文件设置为0)
        /// </summary>
        public IList<string> AddFiles { get; set; }

        /// <summary>
        ///     要删除的文件列表，string:文件相对路径，int:0
        /// </summary>
        public IList<string> DelFiles { get; set; }
    }
}