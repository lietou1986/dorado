#region using

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;

#endregion using

namespace Dorado.VWS.Services.Persistence
{
    public class TaskDao : DBbase<TaskEntity>
    {
        /// <summary>
        ///     插入任务信息
        /// </summary>
        /// <param name = "taskEntity">任务实体</param>
        /// <returns>记录Id</returns>
        public int Insert(TaskEntity taskEntity)
        {
            const string sql =
                @"INSERT INTO zsync_task (TaskName, SyncTaskId, WinServiceName, SourceIP, SourceRoot, TargetIP, TargetRoot, BackupRoot, AddList, DelList, TaskStatus, UserName)
                            VALUES(@TaskName, @SyncTaskId, @WinServiceName, @SourceIP, @SourceRoot, @TargetIP, @TargetRoot, @BackupRoot, @AddList, @DelList, 1, @UserName)
                         select SCOPE_IDENTITY()";

            SqlParameter[] parameters = {
                                            new SqlParameter("@SyncTaskId", taskEntity.SyncTaskId),
                                            new SqlParameter("@TaskName", taskEntity.TaskName),
                                            new SqlParameter("@WinServiceName", taskEntity.WinServiceName),
                                            new SqlParameter("@SourceIP", taskEntity.SourceIP),
                                            new SqlParameter("@SourceRoot", taskEntity.SourceRoot),
                                            new SqlParameter("@TargetIP", taskEntity.TargetIP),
                                            new SqlParameter("@TargetRoot", taskEntity.TargetRoot),
                                            new SqlParameter("@BackupRoot", taskEntity.BackupRoot),
                                            new SqlParameter("@AddList", taskEntity.AddList),
                                            new SqlParameter("@DelList", taskEntity.DelList),
                                            new SqlParameter("@UserName", taskEntity.UserName)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     修改任务状态信息
        /// </summary>
        /// <param name = "taskid">任务Id</param>
        /// <param name = "status">任务状态</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool UpdateStatus(int taskid, EnumTaskStatus status)
        {
            const string sql = @"UPDATE zsync_task SET TaskStatus=@TaskStatus where TaskId = @TaskId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TaskId", taskid),
                                            new SqlParameter("@TaskStatus", status)
                                        };
            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        #region 原有Server获取未完成任务方法

        public IList<TaskEntity> GetTasksByIPAndStatus(string ip, EnumTaskStatus status)
        {
            const string sql =
                @"SELECT * from zsync_task(NOLOCK) WHERE zsync_task.TargetIP = @IP and zsync_task.TaskStatus = @TaskStatus order by zsync_task.TaskId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IP", ip),
                                            new SqlParameter("@TaskStatus", status)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        public TaskEntity GetTaskById(int taskid)
        {
            const string sql = @"SELECT * from zsync_task(NOLOCK) WHERE zsync_task.TaskId = @TaskId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@TaskId", taskid)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        #endregion 原有Server获取未完成任务方法

        //        public IList<TaskEntity> GetTasksByIPAndStatus(string ip, EnumTaskStatus status)
        //        {
        //            const string sql =
        //                @"SELECT  *
        //                    FROM    zsync_task (NOLOCK) task
        //                    LEFT JOIN dbo.zsync_server(NOLOCK) server1 ON server1.IP = task.TargetIP
        //                                                           AND server1.Root = task.TargetRoot
        //                    INNER JOIN dbo.zsync_domain(NOLOCK) domain ON domain.Id = server1.DomainId WHERE task.TargetIP = @IP and task.TaskStatus = @TaskStatus order by task.TaskId";

        //            SqlParameter[] parameters = {
        //                                            new SqlParameter("@IP", ip),
        //                                            new SqlParameter("@TaskStatus", status)
        //                                        };

        //            return GetEntities(CommandType.Text, sql, parameters);
        //        }

        //        public TaskEntity GetTaskById(int taskid)
        //        {
        //            const string sql = @"SELECT  *
        //                    FROM    zsync_task (NOLOCK) task
        //                    LEFT JOIN dbo.zsync_server(NOLOCK) server1 ON server1.IP = task.TargetIP
        //                                                           AND server1.Root = task.TargetRoot
        //                    INNER JOIN dbo.zsync_domain(NOLOCK) domain ON domain.Id = server1.DomainId WHERE task.TaskId = @TaskId";

        //            SqlParameter[] parameters = {
        //                                            new SqlParameter("@TaskId", taskid)
        //                                        };

        //            return GetEntity(CommandType.Text, sql, parameters);
        //        }
    }
}