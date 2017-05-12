#region using

using System;
using System.Data;
using Dorado.VWS.Model.Enum;

#endregion using

namespace Dorado.VWS.Model
{
    public class TaskEntity : EntityBase<TaskEntity>, IConvertToEntity<TaskEntity>
    {
        public int TaskId { get; set; }

        public int SyncTaskId { get; set; }

        public string TaskName { get; set; }

        public string WinServiceName { get; set; }

        public string SourceIP { get; set; }

        public string SourceRoot { get; set; }

        public string TargetIP { get; set; }

        public string TargetRoot { get; set; }

        public string BackupRoot { get; set; }

        public string AddList { get; set; }

        public string DelList { get; set; }

        public string UserName { get; set; }

        public EnumTaskStatus TaskStatus { get; set; }

        #region 当前Server共有域名信息

        /// <summary>
        ///     域名服务器类型
        /// </summary>
        public EnumDomainType DomainType { get; set; }

        /// <summary>
        ///     服务类型
        /// </summary>
        public EnumOperatePathType OperatePathType { get; set; }

        /// <summary>
        /// 操作路径
        /// </summary>
        public string OperatePath { get; set; }

        #endregion 当前Server共有域名信息

        #region IConvertToEntity<TaskEntity> Members

        /// <summary>
        ///     将指定的DataRow转换成指定实体
        /// </summary>
        /// <param name = "row">行数据</param>
        /// <returns>实体信息</returns>
        public TaskEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var taskEntity = new TaskEntity
                                     {
                                         TaskId = Convert.ToInt32(row["TaskId"]),
                                         SyncTaskId = Convert.ToInt32(row["SyncTaskId"]),
                                         TaskName = row["TaskName"].ToString(),
                                         WinServiceName = row["WinServiceName"].ToString(),
                                         SourceIP = row["SourceIP"].ToString(),
                                         SourceRoot = row["SourceRoot"].ToString(),
                                         TargetRoot = row["TargetRoot"].ToString(),
                                         BackupRoot = row["BackupRoot"].ToString(),
                                         AddList = row["AddList"].ToString(),
                                         DelList = row["DelList"].ToString(),
                                         TargetIP = row["TargetIP"].ToString(),
                                         TaskStatus = (EnumTaskStatus)Convert.ToInt32(row["TaskStatus"]),
                                         UserName = row["UserName"].ToString(),
                                     };
                try
                {
                    if (row["DomainType"] != null)
                    {
                        taskEntity.DomainType = (EnumDomainType)ConvertToInt(row["DomainType"].ToString());
                    }
                    if (row["OperatePathType"] != null)
                    {
                        taskEntity.OperatePathType = (EnumOperatePathType)ConvertToInt(row["OperatePathType"].ToString());
                    }
                    if (row["OperatePath"] != null)
                    {
                        taskEntity.OperatePath = row["OperatePath"].ToString();
                    }
                }
                catch (Exception)
                {
                    //Logger.Log("TaskEntity", LogLevel.Info, "HeartBeat Start!");
                    taskEntity.DomainType = 0;
                    taskEntity.OperatePathType = 0;
                    taskEntity.OperatePath = "";
                    return taskEntity;
                }

                return taskEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<TaskEntity> Members
    }
}