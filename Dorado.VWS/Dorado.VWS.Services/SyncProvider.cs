/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/6 11:06:16
 * 版本号：v1.0
 * 本类主要用途描述：同步任务业务类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services.Persistence;

#endregion using

namespace Dorado.VWS.Services
{
    public class SyncProvider
    {
        /// <summary>
        ///     定义同步任务数据访问操作对象
        /// </summary>
        private readonly SynctaskDao _syncTaskDao = new SynctaskDao();

        private readonly SynctaskSubDao _syncTaskSubDao = new SynctaskSubDao();

        /// <summary>
        ///     定义版本文件数据访问操作对象
        /// </summary>
        private readonly VersionFileDao _versionFileDao = new VersionFileDao();

        /// <summary>
        ///     获取同步任务信息
        /// </summary>
        /// <param name = "taskId">同步任务Id</param>
        /// <returns>同步任务实体</returns>
        public SynctaskEntity GetSyncTaskById(int taskId)
        {
            return _syncTaskDao.GetSyncTaskById(taskId);
        }

        /// <summary>
        /// 获取未结束和异常的同步任务信息--add by han
        /// </summary>
        /// <returns>同步任务实体列表</returns>
        public IList<SynctaskEntity> GetAllExceptionSyncTask(string domainName, string operateType, string userName,
                                                             DateTime startDate, DateTime endDate, int beginIndex,
                                                             int endIndex)
        {
            return _syncTaskDao.GetAllExceptionSyncTask(domainName, operateType, userName, startDate, endDate, beginIndex, endIndex);
        }

        /// <summary>
        /// 强制结束异常任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool SyncExceptionByTaskId(int taskId)
        {
            return _syncTaskDao.SyncExceptionByTaskId(taskId);
        }

        /// <summary>
        ///     获取同步任务信息
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>同步任务实体</returns>
        public SynctaskEntity GetLastSyncTaskByDomain(int domainId)
        {
            return _syncTaskDao.GetLastSyncTaskByDomainId(domainId);
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
            return _syncTaskDao.GetSucessSyncTaskByDomain(domainId, beginIndex, endIndex);
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
            return _syncTaskDao.GetSucessSyncTaskByDomain(domainId, userName, beginIndex, endIndex);
        }

        /// <summary>
        ///     成功同步任务个数
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>成功同步任务个数</returns>
        public int GetSucessSyncTaskCount(int domainId)
        {
            return _syncTaskDao.GetSucessSyncTaskCount(domainId);
        }

        /// <summary>
        ///     成功同步任务个数
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "userName">用户名</param>
        /// <returns>成功同步任务个数</returns>
        public int GetSucessSyncTaskCount(int domainId, string userName)
        {
            return _syncTaskDao.GetSucessSyncTaskCount(domainId, userName);
        }

        /// <summary>
        ///     获取上一个TaskId
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "taskid">当前TaskId</param>
        /// <returns>上一个TaskId</returns>
        public int GetPrevTaskId(int domainId, int taskid)
        {
            return _syncTaskDao.GetPrevTaskId(domainId, taskid);
        }

        /// <summary>
        ///     获取下一个TaskId
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "taskid">当前taskId</param>
        /// <returns>下一个TaskId</returns>
        public int GetNextTaskId(int domainId, int taskid)
        {
            return _syncTaskDao.GetNextTaskId(domainId, taskid);
        }

        /// <summary>
        ///     添加同步任务
        /// </summary>
        /// <param name = "domainid"></param>
        /// <param name = "userid"></param>
        /// <param name = "loginfo"></param>
        /// <param name = "optIp"></param>
        /// <param name = "adds"></param>
        /// <param name = "dels"></param>
        /// <returns></returns>
        public int AddSyncTask(int domainid, string userid, string loginfo, string optIp, IList<string> adds,
                               IList<string> dels)
        {
            string addFiles = adds != null ? string.Join(",", adds) : string.Empty;
            string delFiles = dels != null ? string.Join(",", dels) : string.Empty;
            return AddSyncTask(domainid, userid, loginfo, optIp, addFiles, delFiles);
        }

        /// <summary>
        ///     添加同步任务
        /// </summary>
        /// <param name = "domainid"></param>
        /// <param name = "userid"></param>
        /// <param name = "loginfo"></param>
        /// <param name = "optIp"></param>
        /// <param name = "adds"></param>
        /// <param name = "dels"></param>
        /// <returns></returns>
        public int AddSyncTask(int domainid, string userid, string loginfo, string optIp, string adds, string dels)
        {
            var syncTaskEntity = new SynctaskEntity
                                                {
                                                    AddFiles = adds,
                                                    DelFiles = dels,
                                                    OperateType = EnumOperateType.Sync,
                                                    UserName = userid,
                                                    OperatorIp = optIp ?? string.Empty,
                                                    CallType = EnumCallType.WebSite,
                                                    CreateTime = DateTime.Now,
                                                    UpdateTime = DateTime.Now,
                                                    Description = loginfo,
                                                    SyncStatus = EnumSyncStatus.Running,
                                                    DomainId = domainid,
                                                    LogInfo =
                                                        string.Format("{0} 任务添加成功",
                                                                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"))
                                                };

            return _syncTaskDao.Insert(syncTaskEntity);
        }

        public IList<SynctaskEntity> GetProceTasksByDomain(int domainid)
        {
            return _syncTaskDao.GetProceTasksByDomain(domainid);
        }

        public int AddSyncSubTask(int taskid, int targetid)
        {
            var syncTaskSubEntity = new SynctaskSubEntity
                                                      {
                                                          TaskId = taskid,
                                                          SyncServerId = targetid,
                                                          CreateTime = DateTime.Now,
                                                          UpdateTime = DateTime.Now,
                                                          SyncStatus = EnumSyncStatus.Running
                                                      };

            return _syncTaskSubDao.Insert(syncTaskSubEntity);
        }

        public bool CancleSyncTask(int taskid)
        {
            SynctaskEntity syncTaskEntity = _syncTaskDao.GetSyncTaskById(taskid);
            syncTaskEntity.LogInfo += string.Format(" {0} 同步任务开始回滚", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
            syncTaskEntity.SyncStatus = EnumSyncStatus.Running;
            return _syncTaskDao.Update(syncTaskEntity);
        }

        public SynctaskEntity GetUnFinishTask(int domainid, string userid)
        {
            return _syncTaskDao.GetUnFinishTask(domainid, userid);
        }

        public IList<SynctaskSubEntity> GetSubTask(int taskid)
        {
            return _syncTaskSubDao.GetSyncTaskSubByTaskId(taskid);
        }

        public bool UpdateSubTask(SynctaskSubEntity entity)
        {
            return _syncTaskSubDao.Update(entity);
        }

        public bool UpdateTask(SynctaskEntity entity)
        {
            return _syncTaskDao.Update(entity);
        }

        public bool UpdateTaskForLog(SynctaskEntity entity)
        {
            return _syncTaskDao.UpdateForLog(entity);
        }

        public int AddVersionFile(VersionFileEntity entity)
        {
            return _versionFileDao.Insert(entity);
        }

        public IList<VersionFileEntity> GetVersionFiles(int domainId, string filePath, int begin, int end)
        {
            return _versionFileDao.GetVersionFiles(domainId, filePath, begin, end);
        }

        /// <summary>
        ///     计算总共有多少个版本
        /// </summary>
        /// <returns></returns>
        public int GetVersionCount(int domainId, string filePath)
        {
            return _versionFileDao.GetVersionCount(domainId, filePath);
        }

        public IList<VersionFileEntity> GetVersionFiles(int syncTaskId)
        {
            return _versionFileDao.GetVersionFiles(syncTaskId);
        }

        public IList<VersionFileEntity> GetPreVesionFiles(int syncTaskId)
        {
            return _versionFileDao.GetPreVersionFile(syncTaskId);
        }

        public VersionFileEntity GetVersionFile(int versionFileId)
        {
            return _versionFileDao.GetVersionFile(versionFileId);
        }

        #region 更新同步任务状态（回滚/回滚失败/同步成功）  雷斌添加

        /// <summary>
        /// 更新同步任务状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateTaskStatus(int taskID, EnumSyncStatus syncStatus)
        {
            return _syncTaskDao.UpdateTaskStatus(taskID, syncStatus);
        }

        #endregion 更新同步任务状态（回滚/回滚失败/同步成功）  雷斌添加

        #region add by heyongdong

        /// <summary>
        /// 检查任务是否存在F
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="serverid"></param>
        /// <returns></returns>
        public bool ExsitSubTask(int taskid, int serverid)
        {
            var subTask = _syncTaskSubDao.GetSyncTaskSubByTaskId(taskid);

            int[] serverids = (from SynctaskSubEntity stask in subTask
                               select stask.SyncServerId).ToArray();
            return serverids.Contains(serverid);
        }

        /// <summary>
        /// 检查任务是否存在
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="serverid"></param>
        /// <returns></returns>
        public bool ExsitSubTask(int taskid, int[] serverids)
        {
            var subTask = _syncTaskSubDao.GetSyncTaskSubByTaskId(taskid);

            int[] exsitServerids = (from SynctaskSubEntity stask in subTask
                                    select stask.SyncServerId).ToArray();
            foreach (int sid in serverids)
            {
                if (exsitServerids.Contains(sid))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion add by heyongdong
    }
}