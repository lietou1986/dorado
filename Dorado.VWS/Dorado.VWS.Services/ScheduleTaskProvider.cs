/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/6 9:50:33
 * 版本号：v1.0
 * 本类主要用途描述：计划任务业务类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Threading;
using Dorado.VWS.Model;
using Dorado.VWS.Services.Persistence;

#endregion using

namespace Dorado.VWS.Services
{
    public class ScheduleTaskProvider
    {
        private readonly SyncTaskProcessor _stp = new SyncTaskProcessor();
        private readonly TimerSynctaskDao _timerSyncDao = new TimerSynctaskDao();

        /// <summary>
        /// 处理计划任务
        /// </summary>
        public void DoWork()
        {
            IList<TimerSynctaskEntity> timerTasks = _timerSyncDao.GetTasksNextPeriod(5);
            if (timerTasks != null && timerTasks.Count > 0)
            {
                //对每个任务单独使用线程处理
                foreach (var item in timerTasks)
                {
                    TimerSynctaskEntity entity = item;
                    new Thread(
                        () =>
                        {
                            // 任务未建立时，先建立任务
                            int taskid = _stp.AddSyncTask(entity.DomainId, entity.Creator, entity.Description,
                                                          entity.AddFiles, entity.DelFiles);

                            // 同步目标
                            if (taskid != 0)
                            {
                                entity.TaskId = taskid;
                                _timerSyncDao.Update(entity);
                                _stp.SyncTaskTargets(taskid, entity.Creator);
                            }
                        }
                        ).Start();
                }
            }
        }

        /// <summary>
        /// 添加计划任务
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="scheduleTime">计划时间</param>
        /// <param name="domainId">域名Id</param>
        /// <param name="adds">添加文件列表</param>
        /// <param name="dels">删除文件列表</param>
        /// <param name="loginfo">日志</param>
        /// <returns>结果</returns>
        public int AddTask(string userName, DateTime scheduleTime, int domainId, string adds, string dels,
                           string loginfo)
        {
            var entity = new TimerSynctaskEntity
                                             {
                                                 Creator = userName,
                                                 DomainId = domainId,
                                                 ScheduleTime = scheduleTime,
                                                 AddFiles = adds,
                                                 DelFiles = dels,
                                                 TaskId = 0,
                                                 Description = loginfo
                                             };

            return _timerSyncDao.Insert(entity);
        }

        /// <summary>
        /// 添加计划任务
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="scheduleTime">计划时间</param>
        /// <param name="domainId">域名Id</param>
        /// <param name="addFiles">添加文件列表</param>
        /// <param name="delFiles">删除文件列表</param>
        /// <param name="loginfo">日志</param>
        /// <returns>结果</returns>
        public int AddTask(string userName, DateTime scheduleTime, int domainId, IList<string> addFiles,
                           IList<string> delFiles, string loginfo)
        {
            string adds = string.Join(",", addFiles);
            string dels = string.Join(",", delFiles);
            return AddTask(userName, scheduleTime, domainId, adds, dels, loginfo);
        }

        /// <summary>
        /// 获取计划任务
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="begin">起始位</param>
        /// <param name="end">终止位</param>
        /// <returns>结果</returns>
        public IList<TimerSynctaskEntity> GetTasks(int domainId, int begin, int end)
        {
            return _timerSyncDao.GetTasksByDomainId(domainId, begin, end);
        }

        /// <summary>
        /// 获取任务个数
        /// </summary>
        /// <param name="domainId">域名Id</param>
        /// <returns>结果</returns>
        public int GetTaskCount(int domainId)
        {
            return _timerSyncDao.GetTaskCount(domainId);
        }

        /// <summary>
        /// 获取计划任务
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns>结果</returns>
        public TimerSynctaskEntity GetTask(int id)
        {
            return _timerSyncDao.GetTimerSyncTaskById(id);
        }

        /// <summary>
        /// 获取域名的最后一个任务
        /// </summary>
        /// <param name="domainId">域名Id</param>
        /// <returns>任务实体</returns>
        public TimerSynctaskEntity GetLastTask(int domainId)
        {
            return _timerSyncDao.GetLastScheduleTaskByDomainId(domainId);
        }

        /// <summary>
        /// 获取前一任务Id
        /// </summary>
        /// <param name="domainId">域名Id</param>
        /// <param name="curId">当前Id</param>
        /// <returns>结果</returns>
        public int GetPreScheduleTaskId(int domainId, int curId)
        {
            return _timerSyncDao.GetPreScheduleTaskId(domainId, curId);
        }

        /// <summary>
        /// 获取下一任务Id
        /// </summary>
        /// <param name="domainId">域名Id</param>
        /// <param name="curId">当前Id</param>
        /// <returns>结果</returns>
        public int GetNextScheduleTaskId(int domainId, int curId)
        {
            return _timerSyncDao.GetNextScheduleTaskId(domainId, curId);
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskid">任务Id</param>
        /// <returns>结果</returns>
        public bool DeleteTask(int taskid)
        {
            return _timerSyncDao.Delete(taskid);
        }
    }
}