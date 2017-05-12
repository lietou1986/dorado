/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/11 10:26:11
 * 版本号：v1.0
 * 本类主要用途描述：同步子任务数据操作访问类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dorado.VWS.Model;

#endregion using

namespace Dorado.VWS.Services.Persistence
{
    /// <summary>
    ///     同步子任务数据操作访问类
    /// </summary>
    public class SynctaskSubDao : DBbase<SynctaskSubEntity>
    {
        /// <summary>
        ///     获取同步子任务信息
        /// </summary>
        /// <param name = "synctaskId">同步子任务Id</param>
        /// <returns>同步子任务实体</returns>
        public SynctaskSubEntity GetSyncTaskSubById(int synctaskId)
        {
            const string sql =
                @"SELECT * FROM zsync_synctask_sub(NOLOCK)
                           WHERE Id = @SynctaskId and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@SynctaskId", synctaskId)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取同步子任务信息
        /// </summary>
        /// <param name = "taskId">同步任务Id</param>
        /// <returns>同步子任务实体列表</returns>
        public IList<SynctaskSubEntity> GetSyncTaskSubByTaskId(int taskId)
        {
            const string sql =
                @"SELECT * FROM zsync_synctask_sub(NOLOCK)
                           WHERE TaskId = @TaskId and DeleteFlag = 0
                           order by CreateTime desc";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TaskId", taskId)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     插入同步子任务信息
        /// </summary>
        /// <param name = "synctaskSubEntity">同步子任务实体</param>
        /// <returns>记录Id</returns>
        public int Insert(SynctaskSubEntity synctaskSubEntity)
        {
            const string sql =
                @"if not Exists(select * from zsync_synctask_sub where TaskId=@TaskId and SyncServerId=@SyncServerId)
                    begin
                            INSERT INTO zsync_synctask_sub (TaskId,SyncServerId,SyncStatus,ErrorMsg,ReplyFlag)
                            VALUES (@TaskId, @SyncServerId, @SyncStatus, ISNULL(@ErrorMsg,NULL), @ReplyFlag)
                            select SCOPE_IDENTITY()
                    end";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TaskId", synctaskSubEntity.TaskId),
                                            new SqlParameter("@SyncServerId", synctaskSubEntity.SyncServerId),
                                            new SqlParameter("@SyncStatus", synctaskSubEntity.SyncStatus),
                                            new SqlParameter("@ErrorMsg", synctaskSubEntity.ErrorMsg),
                                            new SqlParameter("@ReplyFlag", synctaskSubEntity.ReplyFlag)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     修改同步子任务信息
        /// </summary>
        /// <param name = "synctaskSubEntity">同步子任务实体</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Update(SynctaskSubEntity synctaskSubEntity)
        {
            const string sql =
                @"UPDATE zsync_synctask_sub set SyncStatus =ISNULL(@SyncStatus,SyncStatus),ErrorMsg=ISNULL(@ErrorMsg,ErrorMsg),ReplyFlag = ISNULL(@ReplyFlag,ReplyFlag),UpdateTime=GETDATE() where Id =@SynctaskIfaceId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@SynctaskIfaceId", synctaskSubEntity.SynctaskSubId),
                                            new SqlParameter("@SyncStatus", synctaskSubEntity.SyncStatus),
                                            new SqlParameter("@ErrorMsg", synctaskSubEntity.ErrorMsg),
                                            new SqlParameter("@ReplyFlag", synctaskSubEntity.ReplyFlag)
                                        };

            int result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }
    }
}