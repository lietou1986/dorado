/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/8 17:12:17
 * 版本号：v1.0
 * 本类主要用途描述：同步任务数据访问操作类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dorado.VWS.Model;

#endregion using

namespace Dorado.VWS.Services.Persistence
{
    /// <summary>
    ///     同步任务数据访问操作类
    /// </summary>
    public class SynctaskDao : DBbase<SynctaskEntity>
    {
        /// <summary>
        ///     获取同步任务信息
        /// </summary>
        /// <param name = "taskId">同步任务Id</param>
        /// <returns>同步任务实体</returns>
        public SynctaskEntity GetSyncTaskById(int taskId)
        {
            const string sql =
                @"SELECT a.TaskId,a.OperateType,a.DomainId,vwsdomain.DomainName,vwsdomain.DomainType ,
                            vwsdomain.OperatePathType ,
                            vwsdomain.OperatePath ,a.AddFiles,a.DelFiles,a.UserName,a.OperatorIp,a.CallType,
                          a.CreateTime,a.UpdateTime,a.SyncStatus,a.Description,a.LogInfo +CHAR(10) + CHAR(13) LogInfo
                          FROM zsync_synctask a(NOLOCK)
                          join zsync_domain vwsdomain(NOLOCK) on a.DomainId = vwsdomain.Id
                          WHERE a.TaskId = @TaskId and a.DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TaskId", taskId)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取同步任务信息
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>同步任务实体</returns>
        public SynctaskEntity GetLastSyncTaskByDomainId(int domainId)
        {
            const string sql =
                @"SELECT top 1 a.TaskId,a.OperateType,a.DomainId,vwsdomain.DomainName,vwsdomain.DomainType ,
                            vwsdomain.OperatePathType ,
                            vwsdomain.OperatePath ,a.AddFiles,a.DelFiles,a.UserName,a.OperatorIp,a.CallType,
                          a.CreateTime,a.UpdateTime,a.SyncStatus,a.Description,a.LogInfo +CHAR(10) + CHAR(13) LogInfo
                          FROM zsync_synctask a(NOLOCK)
                          join zsync_domain vwsdomain(NOLOCK) on a.DomainId = vwsdomain.Id
                          WHERE a.DomainId = @DomainId and a.DeleteFlag = 0
                          order by a.TaskId desc";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取上一个TaskId
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "taskid">当前TaskId</param>
        /// <returns>上一个TaskId</returns>
        public int GetPrevTaskId(int domainId, int taskid)
        {
            const string sql =
                @"SELECT Max(TaskId) FROM zsync_synctask(NOLOCK) WHERE TaskId<@TaskId and DomainId=@DomainId  and SyncStatus = 3 and DeleteFlag = 0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@TaskId", taskid)
                                        };
            int id;
            if (int.TryParse(ExecuteScalar(CommandType.Text, sql, parameters).ToString(), out id))
            {
                return id;
            }
            return -1;
        }

        /// <summary>
        ///     获取下一个TaskId
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "taskid">当前taskId</param>
        /// <returns>下一个TaskId</returns>
        public int GetNextTaskId(int domainId, int taskid)
        {
            const string sql =
                @"SELECT Min(TaskId) FROM zsync_synctask(NOLOCK) WHERE TaskId>@TaskId and DomainId=@DomainId and SyncStatus = 3 and DeleteFlag = 0 ";
            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@TaskId", taskid)
                                        };
            int id;
            if (int.TryParse(ExecuteScalar(CommandType.Text, sql, parameters).ToString(), out id))
            {
                return id;
            }
            return -1;
        }

        /// <summary>
        ///     获取同步任务信息
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>同步任务实体列表</returns>
        public IList<SynctaskEntity> GetSyncTaskByDomain(int domainId)
        {
            const string sql =
                @"SELECT synctask.*,vwsdomain.DomainName,vwsdomain.DomainType ,
                            vwsdomain.OperatePathType ,
                            vwsdomain.OperatePath  FROM zsync_synctask synctask(NOLOCK)
                           join zsync_domain vwsdomain(NOLOCK) on synctask.DomainId = vwsdomain.Id
                           WHERE DomainId = @DomainId and synctask.DeleteFlag = 0
                           order by synctask.DomainId,synctask.CreateTime desc";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        public IList<SynctaskEntity> GetProceTasksByDomain(int domainId)
        {
            const string sql =
                @"SELECT synctask.*,vwsdomain.DomainName,vwsdomain.DomainType ,
                            vwsdomain.OperatePathType ,
                            vwsdomain.OperatePath  FROM zsync_synctask synctask(NOLOCK)
                           join zsync_domain vwsdomain(NOLOCK) on synctask.DomainId = vwsdomain.Id
                           WHERE DomainId = @DomainId and (synctask.SyncStatus=1 or synctask.SyncStatus=2) and synctask.DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取同步成功的任务信息
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "beginIndex">开始记录数</param>
        /// <param name = "endIndex">结尾记录数</param>
        /// <returns>同步任务实体列表</returns>
        public IList<SynctaskEntity> GetSucessSyncTaskByDomain(int domainId, int beginIndex, int endIndex)
        {
            var sql =
                @"SELECT * FROM
                        (SELECT synctask.*,vwsdomain.DomainName,vwsdomain.DomainType ,
                        vwsdomain.OperatePathType ,
                        vwsdomain.OperatePath ,ROW_NUMBER() OVER (order by TaskId desc)as RowNumber
                        FROM zsync_synctask(NOLOCK) synctask
                        join zsync_domain vwsdomain(NOLOCK) on synctask.DomainId = vwsdomain.Id
                        WHERE DomainId = @DomainId and synctask.SyncStatus = 3 and synctask.DeleteFlag = 0) tab
                        WHERE RowNumber between {0} AND {1} ORDER BY TaskId DESC";
            sql = string.Format(sql, beginIndex, endIndex);

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取同步成功的任务信息
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "userName">用户名</param>
        /// <param name = "beginIndex">开始记录数</param>
        /// <param name = "endIndex">结尾记录数</param>
        /// <returns>同步任务实体列表</returns>
        public IList<SynctaskEntity> GetSucessSyncTaskByDomain(int domainId, string userName, int beginIndex,
                                                               int endIndex)
        {
            var sql =
                @"SELECT * FROM
                        (SELECT synctask.*,vwsdomain.DomainName,vwsdomain.DomainType ,
                        vwsdomain.OperatePathType ,
                        vwsdomain.OperatePath ,ROW_NUMBER() OVER (order by TaskId desc)as RowNumber
                        FROM zsync_synctask(NOLOCK) synctask
                        join zsync_domain vwsdomain(NOLOCK) on synctask.DomainId = vwsdomain.Id
                        WHERE DomainId = @DomainId and synctask.UserName = @UserName
                        and synctask.SyncStatus = 3 and synctask.DeleteFlag = 0) tab
                        WHERE RowNumber between {0} AND {1} ORDER BY TaskId DESC";
            sql = string.Format(sql, beginIndex, endIndex);

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@UserName", userName)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     成功同步任务个数
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>成功同步任务个数</returns>
        public int GetSucessSyncTaskCount(int domainId)
        {
            const string sql =
                @"SELECT COUNT(*) FROM zsync_synctask(NOLOCK) WHERE DomainId = @DomainId and SyncStatus = 3 and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     成功同步任务个数
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "userName">用户名</param>
        /// <returns>成功同步任务个数</returns>
        public int GetSucessSyncTaskCount(int domainId, string userName)
        {
            const string sql =
                @"SELECT COUNT(*) FROM zsync_synctask(NOLOCK) WHERE DomainId = @DomainId and UserName = @UserName and SyncStatus = 3 and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@UserName", userName)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     获取同步任务信息
        /// </summary>
        /// <returns>同步任务实体列表</returns>
        public IList<SynctaskEntity> GetAllSyncTask()
        {
            const string sql =
                @"SELECT synctask.*,vwsdomain.DomainName,vwsdomain.DomainType ,
                            vwsdomain.OperatePathType ,
                            vwsdomain.OperatePath  FROM zsync_synctask synctask(NOLOCK)
                           join zsync_domain vwsdomain(NOLOCK) on synctask.DomainId = vwsdomain.Id
                           WHERE synctask.DeleteFlag = 0
                           order by synctask.DomainId,synctask.CreateTime desc";

            return GetEntities(CommandType.Text, sql);
        }

        /// <summary>
        /// 获取未结束和异常的同步任务信息--add by han
        /// </summary>
        /// <returns>同步任务实体列表</returns>
        public IList<SynctaskEntity> GetAllExceptionSyncTask(string domainName, string operateType, string userName,
                                                             DateTime startDate, DateTime endDate, int beginIndex,
                                                             int endIndex)
        {
            //获取非结束同步任务
            const string sql =
                @"SELECT *FROM(
                  SELECT synctask.*,vwsdomain.DomainName,vwsdomain.DomainType ,
                  vwsdomain.OperatePathType,
                  vwsdomain.OperatePath,ROW_NUMBER() OVER (order by synctask.TaskId desc)as RowNumber  FROM zsync_synctask synctask(NOLOCK)
                  join zsync_domain vwsdomain(NOLOCK) on synctask.DomainId = vwsdomain.Id
                  WHERE synctask.DeleteFlag = 0 AND
						vwsdomain.DomainName = ISNULL(@DomainName,vwsdomain.DomainName)
						AND synctask.OperateType = ISNULL(@OperateType,synctask.OperateType)
						AND synctask.UserName = ISNULL(@UserName,synctask.UserName)
						AND synctask.Createtime
						BETWEEN ISNULL(@StartDate,synctask.Createtime)
						AND ISNULL(@EndDate,synctask.Createtime)
                        AND synctask.SyncStatus!=3
						AND synctask.SyncStatus!=5
                        AND synctask.SyncStatus!=4
						AND synctask.SyncStatus!=6
                  ) as t
                  WHERE RowNumber between @BeginIndex AND @EndIndex ORDER BY TaskId DESC";
            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainName", domainName),
                                            new SqlParameter("@OperateType", operateType),
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@StartDate", startDate),
                                            new SqlParameter("@EndDate", endDate),
                                            new SqlParameter("@BeginIndex", beginIndex),
                                            new SqlParameter("@EndIndex", endIndex)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 强制结束异常任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool SyncExceptionByTaskId(int taskId)
        {
            const string sql = @"proc_ClearSyncException";
            SqlParameter outPut = new SqlParameter("@Result", SqlDbType.Int);
            outPut.Direction = ParameterDirection.Output;
            SqlParameter[] parameters ={
              new SqlParameter("@TaskId",taskId),
              outPut};
            ExecuteNonQuery(CommandType.StoredProcedure, sql, parameters);
            return (int)outPut.Value == 1;
        }

        /// <summary>
        ///     插入同步任务信息
        /// </summary>
        /// <param name = "synctaskEntity">同步任务实体</param>
        /// <returns>记录Id</returns>
        public int Insert(SynctaskEntity synctaskEntity)
        {
            const string sql =
                @"INSERT INTO zsync_synctask (OperateType,DomainId,AddFiles,DelFiles,UserName,OperatorIp,CallType,LogInfo,SyncStatus,Description)
                         VALUES (@OperateType, @DomainId, ISNULL(@AddFiles,''), ISNULL(@DelFiles,''), @UserName, @OperatorIp,
                         @CallType, ISNULL(@LogInfo,NULL), @SyncStatus,ISNULL(@Description,NULL))
                         select SCOPE_IDENTITY()";

            SqlParameter[] parameters = {
                                            new SqlParameter("@OperateType", synctaskEntity.OperateType),
                                            new SqlParameter("@DomainId", synctaskEntity.DomainId),
                                            new SqlParameter("@AddFiles", synctaskEntity.AddFiles),
                                            new SqlParameter("@DelFiles", synctaskEntity.DelFiles),
                                            new SqlParameter("@UserName", synctaskEntity.UserName),
                                            new SqlParameter("@OperatorIp", synctaskEntity.OperatorIp),
                                            new SqlParameter("@CallType", synctaskEntity.CallType),
                                            new SqlParameter("@LogInfo", synctaskEntity.LogInfo),
                                            new SqlParameter("@SyncStatus", synctaskEntity.SyncStatus),
                                            new SqlParameter("@Description", synctaskEntity.Description)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     修改同步任务信息
        /// </summary>
        /// <param name = "synctaskEntity">同步任务实体</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Update(SynctaskEntity synctaskEntity)
        {
            const string sql =
                @"UPDATE zsync_synctask SET LogInfo = ISNULL(@LogInfo,LogInfo),SyncStatus = ISNULL(@SyncStatus,SyncStatus),
                          Description = ISNULL(@Description,Description), UpdateTime=GETDATE()
                          where TaskId = @TaskId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TaskId", synctaskEntity.TaskId),
                                            new SqlParameter("@LogInfo", synctaskEntity.LogInfo),
                                            new SqlParameter("@SyncStatus", synctaskEntity.SyncStatus),
                                            new SqlParameter("@Description", synctaskEntity.Description)
                                        };

            int result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        public SynctaskEntity GetUnFinishTask(int domainid, string userid)
        {
            const string sql =
                @"SELECT TOP 1 a.TaskId,a.OperateType,a.DomainId,vwsdomain.DomainName,vwsdomain.DomainType ,
                            vwsdomain.OperatePathType ,
                            vwsdomain.OperatePath ,a.AddFiles,a.DelFiles,a.UserName,a.OperatorIp,
                          a.CallType,a.CreateTime,a.UpdateTime,a.SyncStatus,a.Description,a.LogInfo +CHAR(10) + CHAR(13) LogInfo
                          FROM zsync_synctask a
                          join zsync_domain vwsdomain on a.DomainId = vwsdomain.Id
                          WHERE a.DomainId = @DomainId and a.UserName=@UserId and (a.SyncStatus=1 or a.SyncStatus=2) and a.DeleteFlag = 0 order by a.TaskId DESC";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainid),
                                            new SqlParameter("@UserId", userid)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        #region 更新同步任务状态（回滚/回滚失败/同步成功）  雷斌添加

        /// <summary>
        /// 更新同步任务状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateTaskStatus(int taskID, Dorado.VWS.Model.Enum.EnumSyncStatus syncStatus)
        {
            if (taskID < 1)
            {
                return 0;
            }
            StringBuilder ids = new StringBuilder();

            string sql =
                @"UPDATE zsync_synctask
                  SET SyncStatus = @SyncStatus, UpdateTime=GETDATE()
                  WHERE TaskId=@TaskId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@SyncStatus", (int)syncStatus),
                                            new SqlParameter("@TaskId", taskID)
                                        };

            return ExecuteNonQuery(CommandType.Text, sql, parameters);
        }

        #endregion 更新同步任务状态（回滚/回滚失败/同步成功）  雷斌添加

        public bool UpdateForLog(SynctaskEntity synctaskEntity)
        {
            const string sql =
                @"UPDATE zsync_synctask SET LogInfo = LogInfo + @AddLog,SyncStatus = ISNULL(@SyncStatus,SyncStatus),
                          Description = ISNULL(@Description,Description), UpdateTime=GETDATE()
                          where TaskId = @TaskId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TaskId", synctaskEntity.TaskId),
                                            new SqlParameter("@AddLog", synctaskEntity.AddLog),
                                            new SqlParameter("@SyncStatus", synctaskEntity.SyncStatus),
                                            new SqlParameter("@Description", synctaskEntity.Description)
                                        };

            int result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }
    }
}