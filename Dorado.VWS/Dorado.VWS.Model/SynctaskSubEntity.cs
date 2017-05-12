/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/7 14:03:51
 * 版本号：v1.0
 * 本类主要用途描述：同步子任务实体
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Data;
using Dorado.VWS.Model.Enum;

#endregion using

namespace Dorado.VWS.Model
{
    /// <summary>
    ///     同步子任务实体
    /// </summary>
    [Serializable]
    public class SynctaskSubEntity : EntityBase<SynctaskSubEntity>, IConvertToEntity<SynctaskSubEntity>
    {
        /// <summary>
        ///     同步子任务Id
        /// </summary>
        public int SynctaskSubId { get; set; }

        /// <summary>
        ///     关联同步父任务Id
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        ///     同步目标服务器(宿主服务器)Id
        /// </summary>
        public int SyncServerId { get; set; }

        /// <summary>
        ///     记录创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     记录修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        ///     同步状态
        /// </summary>
        public EnumSyncStatus SyncStatus { get; set; }

        /// <summary>
        ///     错误信息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        ///     服务器回复标志
        /// </summary>
        public EnumReplyFlag ReplyFlag { get; set; }

        #region IConvertToEntity<SynctaskSubEntity> Members

        /// <summary>
        ///     将指定的DataRow转换成指定实体
        /// </summary>
        /// <param name = "row">行数据</param>
        /// <returns>实体信息</returns>
        public SynctaskSubEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var entity = new SynctaskSubEntity
                                 {
                                     SynctaskSubId = Convert.ToInt32(row["Id"]),
                                     TaskId = Convert.ToInt32(row["TaskId"]),
                                     SyncServerId = Convert.ToInt32(row["SyncServerId"]),
                                     CreateTime = Convert.ToDateTime(row["CreateTime"]),
                                     SyncStatus = (EnumSyncStatus)Convert.ToInt32(row["SyncStatus"]),
                                     ErrorMsg = row["ErrorMsg"].ToString(),
                                     ReplyFlag = (EnumReplyFlag)Convert.ToInt32(row["ReplyFlag"])
                                 };

                if (row["UpdateTime"] != DBNull.Value)
                {
                    entity.UpdateTime = Convert.ToDateTime(row["UpdateTime"]);
                }

                return entity;
            }
            return null;
        }

        #endregion IConvertToEntity<SynctaskSubEntity> Members
    }
}