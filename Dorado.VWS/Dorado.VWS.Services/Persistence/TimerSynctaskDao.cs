/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/11 11:30:07
 * 版本号：v1.0
 * 本类主要用途描述：定时同步任务数据操作访问类
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
    ///     定时同步任务数据操作访问类
    /// </summary>
    public class TimerSynctaskDao : DBbase<TimerSynctaskEntity>
    {
        /// <summary>
        ///     获取定时同步任务信息
        /// </summary>
        /// <param name = "timerSyncTaskId">定时同步任务Id</param>
        /// <returns>定时同步任务实体</returns>
        public TimerSynctaskEntity GetTimerSyncTaskById(int timerSyncTaskId)
        {
            const string sql =
                @"SELECT timesynctask.* , zsync_domain.DomainName FROM zsync_timer_synctask timesynctask(NOLOCK)
                           join zsync_domain(NOLOCK) on timesynctask.DomainId = zsync_domain.Id
                           WHERE timesynctask.Id = @TimerSyncTaskId and timesynctask.DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TimerSyncTaskId", timerSyncTaskId)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取定时同步任务信息
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>定时同步任务实体列表</returns>
        public IList<TimerSynctaskEntity> GetSyncTaskByDomain(int domainId)
        {
            const string sql =
                @"SELECT timesynctask.* , zsync_domain.DomainName FROM zsync_timer_synctask timesynctask(NOLOCK)
                           join zsync_domain(NOLOCK) on timesynctask.DomainId = zsync_domain.Id
                           WHERE timesynctask.DomainId = @DomainId timesynctask.DeleteFlag = 0
                           order by timesynctask.DomainId,timesynctask.CreateTime desc";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        public IList<TimerSynctaskEntity> GetTasksByDomainId(int domainId, int begin, int end)
        {
            var sql =
                string.Format(
                    @"SELECT * FROM
(SELECT zsync_timer_synctask.*,ROW_NUMBER() OVER (order by zsync_timer_synctask.CreateTime desc)as RowNumber, zsync_domain.DomainName
FROM zsync_timer_synctask(NOLOCK) join zsync_domain(NOLOCK) on zsync_timer_synctask.DomainId = zsync_domain.Id WHERE zsync_timer_synctask.DomainId = @DomainId and zsync_timer_synctask.DeleteFlag = 0) tab
WHERE RowNumber between {0} AND {1} ORDER BY CreateTime DESC",
                    begin, end);

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取域名最后一个计划任务
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>计划任务实体</returns>
        public TimerSynctaskEntity GetLastScheduleTaskByDomainId(int domainId)
        {
            const string sql =
                @"SELECT top 1 zsync_timer_synctask.*, zsync_domain.DomainName
                          FROM zsync_timer_synctask (NOLOCK)
                          join zsync_domain(NOLOCK) on zsync_timer_synctask.DomainId = zsync_domain.Id
                          WHERE zsync_timer_synctask.DomainId = @DomainId and zsync_timer_synctask.DeleteFlag = 0
                          order by zsync_timer_synctask.Id desc";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     计算总共有多少个版本
        /// </summary>
        /// <returns></returns>
        public int GetTaskCount(int domainId)
        {
            const string sql =
                @"SELECT COUNT(*) FROM zsync_timer_synctask(NOLOCK) WHERE DomainId=@DomainId and DeleteFlag=0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     获取定时同步任务信息
        /// </summary>
        /// <returns>定时同步任务实体列表</returns>
        public IList<TimerSynctaskEntity> GetAllSyncTask()
        {
            const string sql =
                @"SELECT timesynctask.* , zsync_domain.DomainName FROM zsync_timer_synctask  timesynctask(NOLOCK)
                           join zsync_domain(NOLOCK) on timesynctask.DomainId = zsync_domain.Id
                           WHERE timesynctask.DeleteFlag = 0
                           order by timesynctask.DomainId,timesynctask.CreateTime desc";

            return GetEntities(CommandType.Text, sql);
        }

        /// <summary>
        ///     获取若干分钟后的计划
        /// </summary>
        /// <param name = "minutes">分钟</param>
        /// <returns>计划任务集合</returns>
        public IList<TimerSynctaskEntity> GetTasksNextPeriod(int minutes)
        {
            DateTime dtFrom = DateTime.Now;
            DateTime dtTo = dtFrom.AddMinutes(minutes);
            const string sql =
                @"SELECT timesynctask.*,zsync_domain.DomainName FROM zsync_timer_synctask timesynctask(NOLOCK) join zsync_domain(NOLOCK) on timesynctask.DomainId = zsync_domain.Id
                           WHERE timesynctask.DeleteFlag = 0 and timesynctask.ScheduleTime between @DtFrom and @DtTo
                           order by timesynctask.DomainId,timesynctask.CreateTime";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DtFrom", dtFrom),
                                            new SqlParameter("@DtTo", dtTo)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     插入定时同步任务信息
        /// </summary>
        /// <param name = "timerSynctaskEntity">定时同步任务实体</param>
        /// <returns>记录Id</returns>
        public int Insert(TimerSynctaskEntity timerSynctaskEntity)
        {
            const string sql =
                @"INSERT INTO zsync_timer_synctask (TaskId,ScheduleTime,DomainId,AddFiles,DelFiles,Creator,Description)
                           VALUES (@TaskId,@ScheduleTime, @DomainId, ISNULL(@AddFiles,NULL), ISNULL(@DelFiles,NULL),
                           @Creator,ISNULL(@Description,NULL))
                           select SCOPE_IDENTITY()";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TaskId", timerSynctaskEntity.TaskId),
                                            new SqlParameter("@ScheduleTime", timerSynctaskEntity.ScheduleTime),
                                            new SqlParameter("@DomainId", timerSynctaskEntity.DomainId),
                                            new SqlParameter("@AddFiles", timerSynctaskEntity.AddFiles),
                                            new SqlParameter("@DelFiles", timerSynctaskEntity.DelFiles),
                                            new SqlParameter("@Creator", timerSynctaskEntity.Creator),
                                            new SqlParameter("@Description", timerSynctaskEntity.Description)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     修改定时同步任务信息
        /// </summary>
        /// <param name = "timerSynctaskEntity">定时同步任务实体</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Update(TimerSynctaskEntity timerSynctaskEntity)
        {
            const string sql =
                @"UPDATE zsync_timer_synctask SET TaskId=ISNULL(@TaskId,NULL),ScheduleTime = ISNULL(@ScheduleTime,NULL),
                         DomainId = ISNULL(@DomainId,NULL),AddFiles = ISNULL(@AddFiles,NULL),DelFiles = ISNULL(@DelFiles,NULL),
                         Updator = ISNULL(@Updator,Updator),UpdateTime=GETDATE(),Description = ISNULL(@Description,Description)
                         where Id = @TimerSyncTaskId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TimerSyncTaskId", timerSynctaskEntity.TimerSyncTaskId),
                                            new SqlParameter("@TaskId", timerSynctaskEntity.TaskId),
                                            new SqlParameter("@ScheduleTime", timerSynctaskEntity.ScheduleTime),
                                            new SqlParameter("@DomainId", timerSynctaskEntity.DomainId),
                                            new SqlParameter("@AddFiles", timerSynctaskEntity.AddFiles),
                                            new SqlParameter("@DelFiles", timerSynctaskEntity.DelFiles),
                                            new SqlParameter("@Updator", timerSynctaskEntity.Updator),
                                            new SqlParameter("@Description", timerSynctaskEntity.Description)
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     删除定时同步任务信息
        /// </summary>
        /// <param name = "timerSyncTaskId">定时同步任务Id</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Delete(int timerSyncTaskId)
        {
            const string sql = @"UPDATE zsync_timer_synctask SET DeleteFlag = 1 where Id = @TimerSyncTaskId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TimerSyncTaskId", timerSyncTaskId)
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        public int GetNextScheduleTaskId(int domainId, int curId)
        {
            const string sql =
                @"SELECT Min(Id) FROM zsync_timer_synctask(NOLOCK) WHERE Id>@TimerSyncTaskId and DomainId=@DomainId and DeleteFlag = 0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@TimerSyncTaskId", curId)
                                        };
            int id;
            if (int.TryParse(ExecuteScalar(CommandType.Text, sql, parameters).ToString(), out id))
            {
                return id;
            }
            return -1;
        }

        public int GetPreScheduleTaskId(int domainId, int curId)
        {
            const string sql =
                @"SELECT Max(Id) FROM zsync_timer_synctask(NOLOCK) WHERE Id<@TimerSyncTaskId and DomainId=@DomainId and DeleteFlag = 0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@TimerSyncTaskId", curId)
                                        };
            int id;
            if (int.TryParse(ExecuteScalar(CommandType.Text, sql, parameters).ToString(), out id))
            {
                return id;
            }
            return -1;
        }
    }
}