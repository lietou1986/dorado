/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/6 11:06:16
 * 版本号：v1.0
 * 本类主要用途描述：通信任务业务类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Net;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services.Persistence;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Services
{
    public class TaskProvider
    {
        #region Delegates

        public delegate void TaskResultHandler(TaskEntity task, TaskResultEntity taskResult);

        #endregion Delegates

        private readonly TaskDao _taskDao = new TaskDao();
        //private readonly ILog _logger = LogManager.GetLogger(typeof (TaskProvider));

        public TaskResultHandler TaskResultReceived;

        /// <summary>
        ///     获取任务
        /// </summary>
        /// <param name = "ip"></param>
        /// <returns></returns>
        public IList<TaskEntity> SeekTask(string ip)
        {
            //获取IP下所有未完成的任务
            IList<TaskEntity> tasks = _taskDao.GetTasksByIPAndStatus(ip, EnumTaskStatus.Wait);
            foreach (var item in tasks)
            {
                _taskDao.UpdateStatus(item.TaskId, EnumTaskStatus.Running);
            }
            return tasks;
        }

        /// <summary>
        ///     添加任务并发送
        /// </summary>
        /// <param name = "taskname"></param>
        /// <param name = "syncTaskId"></param>
        /// <param name = "sourceIP"></param>
        /// <param name = "sourceRoot"></param>
        /// <param name = "targetIP"></param>
        /// <param name = "targetRoot"></param>
        /// <param name = "backupRoot"></param>
        /// <param name = "addlist"></param>
        /// <param name = "dellist"></param>
        /// <param name = "winServiceName"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public int AddTask(string taskname, int syncTaskId, string sourceIP, string sourceRoot, string targetIP,
                           string targetRoot, string backupRoot, string addlist, string dellist, string winServiceName,
                           string username)
        {
            var task = new TaskEntity
                                  {
                                      TaskName = taskname,
                                      SyncTaskId = syncTaskId,
                                      SourceIP = sourceIP,
                                      SourceRoot = sourceRoot,
                                      TargetIP = targetIP,
                                      TargetRoot = targetRoot,
                                      BackupRoot = backupRoot,
                                      AddList = addlist,
                                      DelList = dellist,
                                      WinServiceName = winServiceName,
                                      TaskStatus = EnumTaskStatus.Wait,
                                      UserName = username
                                  };

            int taskid = _taskDao.Insert(task);
            if (taskid != 0)
            {
                //发送任务
                var taskSender = new TaskSenderEntity
                                                  {
                                                      TaskId = taskid,
                                                      TaskCmd =
                                                          (EnumTaskCmd)
                                                          Enum.Parse(typeof(EnumTaskCmd), task.TaskName, true),
                                                      SyncTaskId = task.SyncTaskId,
                                                      WinServiceName = task.WinServiceName,
                                                      SourceIP = task.SourceIP,
                                                      SourceRoot = task.SourceRoot,
                                                      TargetIP = task.TargetIP,
                                                      TargetRoot = task.TargetRoot,
                                                      BackupRoot = task.BackupRoot,
                                                      AddList = task.AddList,
                                                      DelList = task.DelList,
                                                  };

                var client = new SocketClient();
                client.ReceiveTaskResult += ReceivedTaskResult;
                client.AsyncSend(IPAddress.Parse(task.TargetIP), taskSender);
            }

            return taskid;
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public TaskEntity GetTask(int taskid)
        {
            return _taskDao.GetTaskById(taskid);
        }

        private void ReceivedTaskResult(TaskResultEntity taskResult)
        {
            //任务失败时发异常邮件
            if (!taskResult.Success)
            {
                string errMsg = string.Format("IP[{0}] Task[{1}] Error:{2}", taskResult.IP, taskResult.TaskId,
                                              taskResult.ErrorMsg);
                LoggerWrapper.Logger.Error("VWS.Site", errMsg);
                //MailSender.SendErrorEmail(errMsg);
            }

            //获取该Taskid对应的同步任务
            TaskEntity task = _taskDao.GetTaskById(taskResult.TaskId);
            if (task != null && task.TargetIP.Equals(taskResult.IP))
            {
                EnumTaskStatus status = taskResult.Success ? EnumTaskStatus.Successed : EnumTaskStatus.Failed;
                bool flag = _taskDao.UpdateStatus(taskResult.TaskId, status);
                //对结果进行处理
                if (flag)
                {
                    if (TaskResultReceived != null)
                    {
                        TaskResultReceived(task, taskResult);
                    }
                }
            }
        }
    }
}