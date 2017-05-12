/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/6 16:08:22
 * 版本号：v1.0
 * 本类主要用途描述：版本文件实体
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Data;

#endregion using

namespace Dorado.VWS.Model
{
    /// <summary>
    ///     版本文件实体
    /// </summary>
    [Serializable]
    public class VersionFileEntity : EntityBase<VersionFileEntity>, IConvertToEntity<VersionFileEntity>
    {
        /// <summary>
        ///     版本文件Id
        /// </summary>
        public int VersionFileId { get; set; }

        /// <summary>
        ///     同步任务Id
        /// </summary>
        public int SyncTaskId { get; set; }

        /// <summary>
        ///     文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        ///     版本文件路径
        /// </summary>
        public string VersionPath { get; set; }

        /// <summary>
        ///     域名
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        ///     文件同步人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        ///     同步时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     日志
        /// </summary>
        public string Description { get; set; }

        #region IConvertToEntity<VersionFileEntity> Members

        /// <summary>
        ///     将指定的DataRow转换成指定实体
        /// </summary>
        /// <param name = "row">行数据</param>
        /// <returns>实体信息</returns>
        public VersionFileEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var entity = new VersionFileEntity
                                 {
                                     VersionFileId = Convert.ToInt32(row["Id"]),
                                     SyncTaskId = Convert.ToInt32(row["SyncTaskId"]),
                                     FilePath = row["FilePath"].ToString(),
                                     VersionPath = row["VersionPath"].ToString(),
                                     DomainId = Convert.ToInt32(row["DomainId"]),
                                     Creator = row["Creator"].ToString(),
                                     CreateTime = Convert.ToDateTime(row["CreateTime"]),
                                     Description = row["Description"].ToString()
                                 };

                return entity;
            }
            return null;
        }

        #endregion IConvertToEntity<VersionFileEntity> Members
    }
}