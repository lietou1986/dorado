/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/6 11:06:16
 * 版本号：v1.0
 * 本类主要用途描述：同步任务处理类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services.Persistence;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Services
{
    public class SyncTaskProcessor
    {
        private readonly LogProvider _logProvider = new LogProvider();
        private readonly ManualResetEvent _manualevent = new ManualResetEvent(true);
        private readonly object _objLock = new object();
        private readonly ServerProvider _serProvider = new ServerProvider();
        private readonly SyncProvider _syncProvider = new SyncProvider();
        private readonly TaskProvider _taskProvider = new TaskProvider();

        //private readonly ILog _logger = LogManager.GetLogger(typeof(SyncTaskProcessor));
        private int _resetCount;

        public SyncTaskProcessor()
        {
            _taskProvider.TaskResultReceived += ReceivedTaskResult;
        }

        /// <summary>
        /// 接收到任务返回并处理
        /// </summary>
        /// <param name="task">任务实体</param>
        /// <param name="taskResult">任务结果</param>
        private void ReceivedTaskResult(TaskEntity task, TaskResultEntity taskResult)
        {
            switch (task.TaskName)
            {
                case "syncfiles":
                    LoggerWrapper.Logger.Info("ReceivedTaskResult--syncfiles(同步回调IP):" + task.TargetIP);
                    CallbackSyncFiles(task.TargetIP, task.SyncTaskId, taskResult.Success, task.UserName, taskResult.ErrorMsg);
                    break;

                case "backupfiles":
                    CallbackBackupFiles(task, taskResult);
                    lock (_objLock)
                    {
                        _resetCount--;
                        if (_resetCount <= 0)
                        {
                            _manualevent.Set();
                        }
                    }
                    break;

                case "commitfiles":
                    lock (_objLock)
                    {
                        _resetCount--;
                        if (_resetCount <= 0)
                        {
                            _manualevent.Set();
                        }
                    }
                    break;

                case "rollbackfiles":
                    CallbackRollback(task.TargetIP, task.SyncTaskId, taskResult.Success);
                    break;

                case "revertfiles":
                    ServerEntity serverEntity = _serProvider.GetSourceServersByIpAndRoot(task.TargetIP, task.TargetRoot);

                    //定义操作日志
                    var operateLogEntity = new OperationLogEntity
                                                              {
                                                                  UserName = task.UserName,
                                                                  DomainName = serverEntity.DomainName,
                                                                  OperateType = EnumOperateType.RollBack,
                                                                  Log =
                                                                      string.Format("回滚添加文件列表:{0}，删除文件列表:{1}",
                                                                                    task.AddList, task.DelList),
                                                                  Result = taskResult.Success
                                                              };
                    _logProvider.AddOperateLog(operateLogEntity);
                    break;
            }
        }

        /// <summary>
        ///     建立同步任务
        /// </summary>
        /// <param name = "domainid"></param>
        /// <param name="loginfo"></param>
        /// <param name = "adds"></param>
        /// <param name="userid"></param>
        /// <param name="dels"></param>
        public int AddSyncTask(int domainid, string userid, string loginfo, IList<string> adds, IList<string> dels)
        {
            return _syncProvider.AddSyncTask(domainid, userid, loginfo, null, adds, dels);
        }

        /// <summary>
        ///     建立同步任务
        /// </summary>
        /// <param name = "domainid"></param>
        /// <param name="loginfo"></param>
        /// <param name = "adds"></param>
        /// <param name="userid"></param>
        /// <param name="dels"></param>
        public int AddSyncTask(int domainid, string userid, string loginfo, string adds, string dels)
        {
            if (adds == null) throw new ArgumentNullException("adds");
            return _syncProvider.AddSyncTask(domainid, userid, loginfo, null, adds, dels);
        }

        /// <summary>
        ///     判断是否具有正在处理的任务
        /// </summary>
        /// <param name = "domainid"></param>
        /// <param name = "files"></param>
        /// <returns></returns>
        public IList<SynctaskEntity> GetProceTasksByDomainAndFiles(int domainid, IList<string> files)
        {
            IList<SynctaskEntity> tasks = _syncProvider.GetProceTasksByDomain(domainid);

            return tasks.Where(item => (!string.IsNullOrEmpty(item.AddFiles) && item.AddFiles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Intersect(files).ToArray().Any()) || (!string.IsNullOrEmpty(item.DelFiles) && item.DelFiles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Intersect(files).ToArray().Count() > 0)).ToList();
        }

        /// <summary>
        ///     取消任务
        /// </summary>
        /// <param name = "taskid"></param>
        /// <param name="username"></param>
        public bool CancelSyncTask(int taskid, string username)
        {
            // 对已经同步过的目标进行回滚操作
            IList<SynctaskSubEntity> taskSubs = _syncProvider.GetSubTask(taskid);
            foreach (var item in taskSubs)
            {
                ServerEntity targetEntity = _serProvider.GetServerById(item.SyncServerId);
                _taskProvider.AddTask("rollbackfiles", taskid, string.Empty, string.Empty, targetEntity.IP, targetEntity.Root,
                                 string.Empty, string.Empty, string.Empty, string.Empty, username);
                item.SyncStatus = EnumSyncStatus.Running;
            }

            // 设置任务状态为取消
            return _syncProvider.CancleSyncTask(taskid);
        }

        private void CallbackRollback(string ip, int syncTaskid, bool success)
        {
            //获取任务实体
            SynctaskEntity syncTaskEntity = _syncProvider.GetSyncTaskById(syncTaskid);
            if (syncTaskEntity != null)
            {
                //获取目标实体
                ServerEntity targetEntity = _serProvider.GetTargetServerByDomainIdAndIP(syncTaskEntity.DomainId, ip);
                //设置当前目标处理结果
                IList<SynctaskSubEntity> subEntities = _syncProvider.GetSubTask(syncTaskid);

                SynctaskSubEntity subTmp = subEntities.FirstOrDefault(s => s.SyncServerId == targetEntity.ServerId);
                if (subTmp != null)
                {
                    // 更新同步结果
                    subTmp.ReplyFlag = EnumReplyFlag.Yes;
                    subTmp.SyncStatus = success ? EnumSyncStatus.Rollback : EnumSyncStatus.RollbackFailed;
                    _syncProvider.UpdateSubTask(subTmp);

                    syncTaskEntity.AddLog += string.Format(" {0} 回滚({1}){2}",
                                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                                                                targetEntity.IP, success ? "成功" : "失败");
                    _syncProvider.UpdateTaskForLog(syncTaskEntity);
                    SetRollbackTaskStatus(syncTaskid);
                }
            }
        }

        private void SetRollbackTaskStatus(int taskid)
        {
            //获取子任务实体
            IList<SynctaskSubEntity> subEntities = _syncProvider.GetSubTask(taskid);

            // 如果有正在处理的目标，则退出
            if (subEntities.Count(sub => sub.SyncStatus == EnumSyncStatus.Running) > 0)
            {
                return;
            }

            //获取任务实体
            SynctaskEntity syncTaskEntity = _syncProvider.GetSyncTaskById(taskid);

            if (subEntities.Count(sub => sub.SyncStatus == EnumSyncStatus.RollbackFailed) > 0)
            {
                syncTaskEntity.AddLog += string.Format(" {0} 全部回滚完成(有失败)",
                                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                syncTaskEntity.SyncStatus = EnumSyncStatus.RollbackFailed;
            }
            else
            {
                syncTaskEntity.AddLog += string.Format(" {0} 全部回滚完成(无失败)",
                                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                syncTaskEntity.SyncStatus = EnumSyncStatus.Rollback;
            }

            _syncProvider.UpdateTaskForLog(syncTaskEntity);

            //定义操作日志
            var operateLogEntity = new OperationLogEntity
                                                      {
                                                          UserName = syncTaskEntity.UserName,
                                                          DomainName = syncTaskEntity.DomainName,
                                                          //必须记录为同步操作（因为是同步操作失败后进行回滚的）
                                                          OperateType = EnumOperateType.Sync,
                                                          Log = syncTaskEntity.LogInfo,
                                                          //同步操作失败，进行回滚
                                                          Result = false
                                                      };
            _logProvider.AddOperateLog(operateLogEntity);
        }

        /// <summary>
        ///     同步任务到目标
        /// </summary>
        /// <param name = "taskid"></param>
        /// <param name = "targetids"></param>
        /// <param name="username"></param>
        public int SyncTaskTargets(int taskid, IList<int> targetids, string username)
        {
            SynctaskEntity syncTaskEntity = _syncProvider.GetSyncTaskById(taskid);
            ServerEntity sourceEntity = _serProvider.GetSourceServerByDomainId(syncTaskEntity.DomainId);
            int i = 0;
            foreach (var targetid in targetids)
            {
                ServerEntity targetEntity = _serProvider.GetServerById(targetid);
                // 为目标设置同步子任务
                i = _syncProvider.AddSyncSubTask(taskid, targetid);
                syncTaskEntity.LogInfo += string.Format("{0} 开始同步({1})",
                                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                                                        targetEntity.IP);
                // 发布同步任务
                _taskProvider.AddTask("syncfiles", syncTaskEntity.TaskId, sourceEntity.IP, sourceEntity.Root, targetEntity.IP,
                                 targetEntity.Root, string.Empty, syncTaskEntity.AddFiles, syncTaskEntity.DelFiles,
                                 string.Empty, username);
            }
            return i;
        }

        /// <summary>
        ///     同步任务到目标
        /// </summary>
        /// <param name = "taskid"></param>
        /// <param name="username"></param>
        public void SyncTaskTargets(int taskid, string username)
        {
            SynctaskEntity syncTaskEntity = _syncProvider.GetSyncTaskById(taskid);
            ServerEntity sourceEntity = _serProvider.GetSourceServerByDomainId(syncTaskEntity.DomainId);

            IList<ServerEntity> targetEntities = _serProvider.GetTargetServerByDomainId(syncTaskEntity.DomainId);

            foreach (var target in targetEntities)
            {
                // 为目标设置同步子任务
                _syncProvider.AddSyncSubTask(taskid, target.ServerId);
                syncTaskEntity.LogInfo += string.Format("{0} 开始同步({1})",
                                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), target.IP);
                // 发布同步任务
                _taskProvider.AddTask("syncfiles", syncTaskEntity.TaskId, sourceEntity.IP, sourceEntity.Root, target.IP,
                                 target.Root, string.Empty, syncTaskEntity.AddFiles, syncTaskEntity.DelFiles,
                                 string.Empty, username);
            }
        }

        /// <summary>
        ///     同步文件callback
        /// </summary>
        /// <param name = "ip"></param>
        /// <param name = "syncTaskid"></param>
        /// <param name = "success"></param>
        /// <param name="username"></param>
        /// <param name="errorMsg"></param>
        public void CallbackSyncFiles(string ip, int syncTaskid, bool success, string username, string errorMsg)
        {
            //获取任务实体
            SynctaskEntity syncTaskEntity = _syncProvider.GetSyncTaskById(syncTaskid);
            if (syncTaskEntity != null)
            {
                //获取目标实体
                ServerEntity targetEntity = _serProvider.GetTargetServerByDomainIdAndIP(syncTaskEntity.DomainId, ip);
                //设置当前目标处理结果
                IList<SynctaskSubEntity> subEntities = _syncProvider.GetSubTask(syncTaskid);

                SynctaskSubEntity subTmp = subEntities.FirstOrDefault(s => s.SyncServerId == targetEntity.ServerId);

                LoggerWrapper.Logger.Error("VWS.CallbackSyncFiles", "同步回调success为:" + success.ToString());
                LoggerWrapper.Logger.Error("VWS.CallbackSyncFiles", "同步回调ip为:" + ip);
                LoggerWrapper.Logger.Error("VWS.CallbackSyncFiles", "同步回调错误信息:" + errorMsg);
                if (subTmp != null)
                {
                    LoggerWrapper.Logger.Error("VWS.CallbackSyncFiles", "同步回调subTmp不为null" + subTmp.ErrorMsg);
                    if (success)
                    {
                        syncTaskEntity.AddLog += string.Format(" {0} 同步({1})成功",
                                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                                                                targetEntity.IP);
                    }
                    else
                    {
                        syncTaskEntity.AddLog += string.Format(" {0} 同步({1})失败 (原因:{2})",
                                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                                                                targetEntity.IP, errorMsg);
                    }
                    _syncProvider.UpdateTaskForLog(syncTaskEntity);

                    // 更新同步结果
                    subTmp.ReplyFlag = EnumReplyFlag.Yes;
                    subTmp.SyncStatus = success ? EnumSyncStatus.Succeed : EnumSyncStatus.Failed;
                    _syncProvider.UpdateSubTask(subTmp);

                    SetSyncTaskStatus(syncTaskid, username);
                }
                else
                {
                    LoggerWrapper.Logger.Error("VWS.CallbackSyncFiles", "同步回调subTmp为null");
                }
            }
        }

        private void SetSyncTaskStatus(int taskid, string username)
        {
            //获取子任务实体
            IList<SynctaskSubEntity> subEntities = _syncProvider.GetSubTask(taskid);

            // 如果有正在处理的目标，则退出
            if (subEntities.Count(sub => sub.SyncStatus == EnumSyncStatus.Running) > 0)
            {
                return;
            }

            //获取任务实体
            SynctaskEntity syncTaskEntity = _syncProvider.GetSyncTaskById(taskid);
            //获取宿主实体
            IList<ServerEntity> targetEntities = _serProvider.GetTargetServerByDomainId(syncTaskEntity.DomainId);

            // 如果有失败的，则回滚
            if (subEntities.Count(sub => sub.SyncStatus == EnumSyncStatus.Failed) > 0)
            {
                syncTaskEntity.AddLog += string.Format(" {0} 同步失败开始回滚",
                                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                syncTaskEntity.SyncStatus = EnumSyncStatus.Running;
                _syncProvider.UpdateTaskForLog(syncTaskEntity);
                //发送回滚命令
                foreach (var item in subEntities)
                {
                    item.SyncStatus = EnumSyncStatus.Running;
                    _syncProvider.UpdateSubTask(item);
                    ServerEntity target = targetEntities.First(t => t.ServerId == item.SyncServerId);

                    _taskProvider.AddTask("rollbackfiles", syncTaskEntity.TaskId, string.Empty, string.Empty, target.IP,
                                     target.Root, string.Empty, string.Empty, string.Empty, string.Empty, username);
                }
                return;
            }

            // 所有的目标都处理完成
            if (
                subEntities.OrderBy(sub => sub.SyncServerId).Select(sub => sub.SyncServerId).SequenceEqual(
                    targetEntities.OrderBy(t => t.ServerId).Select(t => t.ServerId)))
            {
                //同步成功发送commit任务
                _manualevent.Reset();
                _resetCount = targetEntities.Count + 1;
                foreach (var item in targetEntities)
                {
                    _taskProvider.AddTask("commitfiles", syncTaskEntity.TaskId, string.Empty, string.Empty, item.IP,
                                     item.Root, string.Empty, string.Empty, string.Empty, string.Empty, username);
                }

                // 同步成功向demo发送备份命令
                ServerEntity source = _serProvider.GetSourceServerByDomainId(syncTaskEntity.DomainId);
                _taskProvider.AddTask("backupfiles", syncTaskEntity.TaskId, string.Empty, string.Empty, source.IP,
                                 source.Root, DateTime.Now.ToString("yyyy_MM_dd"), syncTaskEntity.AddFiles,
                                 syncTaskEntity.DelFiles, string.Empty, username);

                // 判断是否需要清缓存
                DomainEntity domain = _serProvider.GetDomainById(source.DomainId);
                if (!string.IsNullOrEmpty(domain.CacheUrl))
                {
                    string[] urlTmps = domain.CacheUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] files = syncTaskEntity.AddFiles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    files =
                        files.Concat(syncTaskEntity.DelFiles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).
                            ToArray();

                    var urls = new List<string>();
                    foreach (var file in files)
                    {
                        urls.AddRange(urlTmps.Select(url => string.Format("http://{0}/{1}", url, file)));
                    }
                    ClearCache.Clear(urls);
                }

                _manualevent.WaitOne(600000);
                syncTaskEntity.AddLog += string.Format(" {0} 同步成功", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                syncTaskEntity.SyncStatus = EnumSyncStatus.Succeed;
                _syncProvider.UpdateTaskForLog(syncTaskEntity);

                //定义操作日志
                var operateLogEntity = new OperationLogEntity
                                                          {
                                                              UserName = syncTaskEntity.UserName,
                                                              DomainName = syncTaskEntity.DomainName,
                                                              OperateType = EnumOperateType.Sync,
                                                              Log = syncTaskEntity.LogInfo,
                                                              //同步操作成功
                                                              Result = true
                                                          };
                _logProvider.AddOperateLog(operateLogEntity);

                return;
            }

            // 还有目标没有处理完成
            syncTaskEntity.AddLog += string.Format(" {0} 同步任务挂起", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
            syncTaskEntity.SyncStatus = EnumSyncStatus.Suspend;
            _syncProvider.UpdateTaskForLog(syncTaskEntity);
        }

        /// <summary>
        ///     备份文件callback
        /// </summary>
        /// <param name = "task"></param>
        /// <param name="taskResult"></param>
        public void CallbackBackupFiles(TaskEntity task, TaskResultEntity taskResult)
        {
            if (!taskResult.Success) return;
            // 备份成功，添加文件版本到表中
            SynctaskEntity syncTask = _syncProvider.GetSyncTaskById(task.SyncTaskId);
            foreach (var add in task.AddList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var entity = new VersionFileEntity
                                 {
                                     SyncTaskId = task.SyncTaskId,
                                     FilePath = add,
                                     VersionPath =
                                         task.BackupRoot + '\\' + task.SyncTaskId + '\\' + add,
                                     DomainId = syncTask.DomainId,
                                     Creator = syncTask.UserName,
                                     Description = syncTask.Description
                                 };
                _syncProvider.AddVersionFile(entity);
            }

            foreach (var del in task.DelList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var entity = new VersionFileEntity
                                               {
                                                   SyncTaskId = task.SyncTaskId,
                                                   FilePath = del,
                                                   DomainId = syncTask.DomainId,
                                                   Creator = syncTask.UserName,
                                                   Description = syncTask.Description,
                                                   VersionPath =
                                                       task.BackupRoot + '\\' + task.SyncTaskId + '\\' + del
                                               };
                _syncProvider.AddVersionFile(entity);
            }

            var fileMD5Dao = new FileMD5Dao();
            foreach (var item in taskResult.FileList)
            {
                fileMD5Dao.Save(syncTask.DomainId, item.FullName, item.MD5);
            }
        }

        /// <summary>
        /// 回滚文件
        /// </summary>
        /// <param name="domainid"></param>
        /// <param name="versionFileIds"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public int RevertFiles(int domainid, List<int> versionFileIds, string username)
        {
            ServerEntity source = _serProvider.GetSourceServerByDomainId(domainid);
            var files = new List<string>();

            foreach (var item in versionFileIds)
            {
                VersionFileEntity vfEntity = _syncProvider.GetVersionFile(item);
                files.Add(vfEntity.VersionPath);
            }

            return _taskProvider.AddTask("revertfiles", 0, string.Empty, string.Empty, source.IP, source.Root, string.Empty,
                                    string.Join(",", files), string.Empty, string.Empty, username);
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
            return _syncProvider.UpdateTaskStatus(taskID, syncStatus);
        }

        #endregion 更新同步任务状态（回滚/回滚失败/同步成功）  雷斌添加

        #region 对同步中的文件锁定，判断要同步的文件是否被锁定 雷斌添加

        /// <summary>
        /// 获取缓存键名
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        private string GetCacheKey(string domain)
        {
            return "ImageSyncDomain_" + domain;
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="addFiles"></param>
        /// <param name="delFiles"></param>
        /// <returns></returns>
        private List<string> GetFileList(string[] addFiles, string[] delFiles)
        {
            List<string> list = new List<string>();
            if (addFiles != null)
            {
                foreach (string fileName in addFiles)
                {
                    if (!list.Contains(fileName))
                    {
                        list.Add(fileName);
                    }
                }
            }

            if (delFiles != null)
            {
                foreach (string fileName in delFiles)
                {
                    if (!list.Contains(fileName))
                    {
                        list.Add(fileName);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 将要同步的文件名添加到缓存
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="addFiles"></param>
        /// <param name="delFiles"></param>
        private void AddFileNameIntoCache(string domain, string[] addFiles, string[] delFiles)
        {
            string cacheKey = GetCacheKey(domain);
            List<string> list = new List<string>();

            foreach (string fileName in GetFileList(addFiles, delFiles))
            {
                if (!list.Contains(fileName))
                {
                    list.Add(fileName);
                }
            }

            if (list.Count > 0)
            {
                object obj = WebCache.Get(cacheKey);

                if (obj == null)
                {
                    WebCache.Add(cacheKey, list, 30);
                }
                else
                {
                    List<string> lastList = (List<string>)obj;
                    foreach (string f in list)
                    {
                        lastList.Add(f);
                    }
                    WebCache.Add(cacheKey, lastList, 30);
                }
            }
        }

        /// <summary>
        /// 在缓存中验证文件是否被同步中
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="addFiles"></param>
        /// <param name="delFiles"></param>
        private bool IsUsingByFileName(string domain, string[] addFiles, string[] delFiles, out string lockedFileName)
        {
            string cacheKey = GetCacheKey(domain);
            object obj = WebCache.Get(cacheKey);
            lockedFileName = string.Empty;

            if (obj != null)
            {
                List<string> list = (List<string>)obj;

                foreach (string fileName in GetFileList(addFiles, delFiles))
                {
                    if (list.Contains(fileName))
                    {
                        lockedFileName = fileName;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 将已经同步成功的文件名从缓存中删除
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="addFiles"></param>
        /// <param name="delFiles"></param>
        private void DeleteFileNameFromCache(string domain, string[] addFiles, string[] delFiles)
        {
            string cacheKey = GetCacheKey(domain);
            object obj = WebCache.Get(cacheKey);

            if (obj != null)
            {
                List<string> list = (List<string>)obj;
                List<string> newList = new List<string>();

                foreach (string fileName in GetFileList(addFiles, delFiles))
                {
                    if (!list.Contains(fileName))
                    {
                        newList.Add(fileName);
                    }
                }

                if (newList.Count > 0)
                {
                    WebCache.Add(cacheKey, newList, 30);
                }
                else
                {
                    WebCache.Remove(cacheKey);
                }
            }
        }

        #endregion 对同步中的文件锁定，判断要同步的文件是否被锁定 雷斌添加

        /// <summary>
        /// 同步文件(简单同步）
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="domain"></param>
        /// <param name="addFiles"></param>
        /// <param name="delFiles"></param>
        /// <param name="cacheDomains"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool SyncUnverTask(string userName, string domain, string[] addFiles, string[] delFiles, out string errorMsg)
        {
            DateTime now = DateTime.Now;
            Random rand = new Random(now.Millisecond);
            int taskID = now.Second * 100000 + now.Millisecond * 1000 + rand.Next(now.Second);

            IList<ServerEntity> servers = _serProvider.GetServersByDomain(domain);
            if (servers == null || servers.Count == 0)
            {
                errorMsg = "域名错误";
                return false;
            }

            ServerEntity source = servers.First(x => x.ServerType == EnumServerType.Source);
            IList<ServerEntity> targetServers = servers.Where(x => x.ServerType == EnumServerType.Host).ToList();
            var client = new SocketClient();

            //检查服务器连接
            foreach (ServerEntity target in servers)
            {
                var testProvider = new TestConnectProvider();
                if (testProvider.TestConnect(target.ServerId)) continue;

                errorMsg = "服务器[{0}]连接失败";
                return false;
            }

            var filelistProvider = new FileListProvider();
            string nonExist;
            if (!filelistProvider.FilesExist(source.DomainId, addFiles, out nonExist))
            {
                errorMsg = string.Format("文件不存在:{0}", nonExist);
                return false;
            }

            #region 判断文件是否被占用

            string lockedFileName = string.Empty;
            // 缓存中验证是否被锁定
            if (IsUsingByFileName(domain, addFiles, delFiles, out lockedFileName))
            {
                errorMsg = string.Format("文件被锁定:{0}", lockedFileName);
                return false;
            }

            #endregion 判断文件是否被占用

            #region 将要同步的文件名加入缓存

            // 添加缓存
            AddFileNameIntoCache(domain, addFiles, delFiles);

            #endregion 将要同步的文件名加入缓存

            var syncedTargets = new List<ServerEntity>();
            //定义操作日志
            var logProvider = new LogProvider();
            var operateLogEntity = new OperationLogEntity
            {
                UserName = userName,
                DomainName = domain,
                OperateType = EnumOperateType.ImageSync,
                Log =
                    string.Format("添加文件列表:{0}，删除文件列表:{1}", addFiles == null ? string.Empty : string.Join(",", addFiles),
                                  delFiles == null ? string.Empty : string.Join(",", delFiles))
            };
            //尝试同步到宿
            foreach (ServerEntity target in targetServers)
            {
                var taskSender = new TaskSenderEntity
                {
                    SyncTaskId = taskID,
                    TaskCmd = EnumTaskCmd.SYNCFILES,
                    ////CustomCmd
                    //CustomCmd=EnumTaskCmd.SYNCFILES,
                    SourceIP = source.IP,
                    SourceRoot = source.Root,
                    TargetIP = target.IP,
                    TargetRoot = target.Root,
                    AddList = addFiles == null ? string.Empty : string.Join(",", addFiles),
                    DelList = delFiles == null ? string.Empty : string.Join(",", delFiles),
                    DomainType = (int)target.DomainType,
                    OperatePathType = (int)target.OperatePathType,
                    OperatePath = target.OperatePath,
                };

                client = new SocketClient();
                TaskResultEntity result = client.SyncSend(IPAddress.Parse(target.IP), taskSender);
                syncedTargets.Add(target);
                //同步失败回滚
                if (!result.Success)
                {
                    foreach (ServerEntity syncedTarget in syncedTargets)
                    {
                        var rollbackTask = new TaskSenderEntity
                        {
                            SyncTaskId = taskID,
                            TaskCmd = EnumTaskCmd.ROLLBACKFILES,
                            ////CustomCmd
                            //CustomCmd = EnumTaskCmd.ROLLBACKFILES,
                            TargetIP = syncedTarget.IP,
                            TargetRoot = syncedTarget.Root,
                            DomainType = (int)syncedTarget.DomainType,
                            OperatePathType = (int)syncedTarget.OperatePathType,
                            OperatePath = syncedTarget.OperatePath,
                        };
                        client.SyncSend(IPAddress.Parse(syncedTarget.IP), rollbackTask);
                    }
                    errorMsg = string.Format("同步{0}失败！", result.IP);

                    operateLogEntity.Log += errorMsg;
                    operateLogEntity.Result = false;
                    logProvider.AddOperateLog(operateLogEntity);

                    #region 将已经同步完成的文件名从缓存中清除

                    // 清除缓存
                    DeleteFileNameFromCache(domain, addFiles, delFiles);

                    #endregion 将已经同步完成的文件名从缓存中清除

                    return false;
                }
            }

            //同步成功提交
            foreach (ServerEntity syncedTarget in syncedTargets)
            {
                var rollbackTask = new TaskSenderEntity
                {
                    SyncTaskId = taskID,
                    TaskCmd = EnumTaskCmd.COMMITFILES,
                    ////CustomCmd
                    //CustomCmd = EnumTaskCmd.COMMITFILES,
                    TargetIP = syncedTarget.IP,
                    TargetRoot = syncedTarget.Root,
                    DomainType = (int)syncedTarget.DomainType,
                    OperatePathType = (int)syncedTarget.OperatePathType,
                    OperatePath = syncedTarget.OperatePath,
                };
                client.SyncSend(IPAddress.Parse(syncedTarget.IP), rollbackTask);
            }
            errorMsg = "同步成功！";
            operateLogEntity.Result = true;
            logProvider.AddOperateLog(operateLogEntity);

            #region 将已经同步完成的文件名从缓存中清除

            // 清除缓存
            DeleteFileNameFromCache(domain, addFiles, delFiles);

            #endregion 将已经同步完成的文件名从缓存中清除

            // 判断是否需要清缓存

            LoggerWrapper.Logger.Info("VWS.Site", string.Format("Source DomainId : " + source.DomainId));
            DomainEntity domainEntity = _serProvider.GetDomainById(source.DomainId);
            LoggerWrapper.Logger.Info("VWS.Site", string.Format("domainEntity.CacheUrl : " + domainEntity.CacheUrl));
            if (!string.IsNullOrEmpty(domainEntity.CacheUrl))
            {
                if (addFiles != null)
                {
                    LoggerWrapper.Logger.Info("VWS.Site", string.Format("addFiles Count : " + addFiles.Length));
                }
                if (delFiles != null)
                {
                    LoggerWrapper.Logger.Info("VWS.Site", string.Format("deleteFiles Count : " + delFiles.Length));
                }
                string[] urlTmps = domainEntity.CacheUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] files = addFiles;
                if (files != null && delFiles != null)
                {
                    files =
                                    files.Concat(delFiles).
                                        ToArray();
                }
                else if (files == null)
                {
                    files = delFiles;
                }

                if (files != null)
                {
                    var urls = new List<string>();
                    foreach (var file in files)
                    {
                        urls.AddRange(urlTmps.Select(url => string.Format("http://{0}/{1}", url, file)));
                    }

                    //image.dorado.com 域名单独处理
                    if (domain.ToLower() == "image.dorado.com ")
                    {
                        foreach (var file in files)
                        {
                            urls.Add(string.Format(string.Format("http://{0}/{1}", GetServerIndex(file), file)));
                        }
                    }
                    //_logger.Debug("Call Clear Cache");
                    LoggerWrapper.Logger.Info("VWS.Site", "Call Clear Cache");
                    ClearCache.Clear(urls);
                }
            }
            return true;
        }

        #region 对于image.dorado.com 清理缓存，除了固定的images.dorado.com 和i.dorado.com ，还有i[1-5].dorado.com ，分配算法由(张鑫)提供

        /// <summary>
        /// 获取image.dorado.com 的随机缓存域名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetServerIndex(string fileName)
        {
            string[] s = fileName.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length < 1)
            {
                return string.Empty;
            }
            string[] domainList = { "i1.dorado.com ", "i2.dorado.com ", "i3.dorado.com ", "i4.dorado.com ", "i5.dorado.com " };

            int Index = Math.Abs(s[s.Length - 1].GetHashCode()) % domainList.Length;
            return domainList[Index];
        }

        #endregion 对于image.dorado.com 清理缓存，除了固定的images.dorado.com 和i.dorado.com ，还有i[1-5].dorado.com ，分配算法由(张鑫)提供
    }
}