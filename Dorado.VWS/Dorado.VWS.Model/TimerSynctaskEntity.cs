/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/7 11:29:45
 * 版本号：v1.0
 * 本类主要用途描述：定时同步任务实体
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Data;

#endregion using

namespace Dorado.VWS.Model
{
    /// <summary>
    ///     定时同步任务实体
    /// </summary>
    [Serializable]
    public class TimerSynctaskEntity : EntityBase<TimerSynctaskEntity>, IConvertToEntity<TimerSynctaskEntity>
    {
        /// <summary>
        ///     定时同步任务Id
        /// </summary>
        public int TimerSyncTaskId { get; set; }

        /// <summary>
        ///     总执行同步任务Id
        ///     备注：当到定时时间执行任务开始后，才会有TaskId，初始是null
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        ///     同步执行时间
        /// </summary>
        public DateTime ScheduleTime { get; set; }

        /// <summary>
        ///     域名Id
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        ///     域名名称
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        ///     同步需要增加或修改文件集合
        /// </summary>
        public string AddFiles { get; set; }

        /// <summary>
        ///     同步需要删除文件集合
        /// </summary>
        public string DelFiles { get; set; }

        /// <summary>
        ///     记录创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        ///     记录创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     记录修改人
        /// </summary>
        public string Updator { get; set; }

        /// <summary>
        ///     记录修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        ///     同步任务描述
        /// </summary>
        public string Description { get; set; }

        #region IConvertToEntity<TimerSynctaskEntity> Members

        /// <summary>
        ///     将指定的DataRow转换成指定实体
        /// </summary>
        /// <param name = "row">行数据</param>
        /// <returns>实体信息</returns>
        public TimerSynctaskEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var entity = new TimerSynctaskEntity
                                 {
                                     TimerSyncTaskId = Convert.ToInt32(row["Id"]),
                                     TaskId = Convert.ToInt32(row["TaskId"]),
                                     ScheduleTime = Convert.ToDateTime(row["ScheduleTime"]),
                                     DomainId = Convert.ToInt32(row["DomainId"]),
                                     DomainName = row["DomainName"].ToString(),
                                     Creator = row["Creator"].ToString(),
                                     CreateTime = Convert.ToDateTime(row["CreateTime"])
                                 };

                if (!string.IsNullOrEmpty(row["AddFiles"].ToString()))
                {
                    entity.AddFiles = row["AddFiles"].ToString();
                }
                if (!string.IsNullOrEmpty(row["DelFiles"].ToString()))
                {
                    entity.DelFiles = row["DelFiles"].ToString();
                }

                if (!string.IsNullOrEmpty(row["Updator"].ToString()))
                {
                    entity.Updator = row["Updator"].ToString();
                }
                if (row["UpdateTime"] != DBNull.Value)
                {
                    entity.UpdateTime = Convert.ToDateTime(row["UpdateTime"]);
                }
                if (row["Description"] != DBNull.Value)
                {
                    entity.Description = row["Description"].ToString();
                }

                return entity;
            }
            return null;
        }

        #endregion IConvertToEntity<TimerSynctaskEntity> Members
    }
}