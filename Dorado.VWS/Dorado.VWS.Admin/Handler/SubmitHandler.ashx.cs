#region using

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

//using Dorado.VWS.Admin.PermissionWCF;

#endregion using

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    ///     提交数据的Handler
    /// </summary>
    public class SubmitHandler : IHttpHandler
    {
        private WebBasePage basePage = new WebBasePage();

        protected string CurUsername
        {
            get
            {
                return WebBasePage.GetUserName();
            }
        }

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            if (string.IsNullOrEmpty(CurUsername))
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("未登录");
                return;
            }

            var sb = new StringBuilder();
            string errMsg;
            switch (context.Request.QueryString["action"])
            {
                case "revertfiles":
                    {
                        string data = context.Server.UrlDecode(context.Request.QueryString["data"]);
                        var fileList =
                            new JavaScriptSerializer().Deserialize<List<VersionFileEntity>>(data);

                        IEnumerable<int> taskIds = RevertFiles(fileList, out errMsg);
                        sb.Append(string.Format("{{\"taskID\": \"{0}\", \"errorMsg\":\"{1}\"}}",
                                                string.Join(",", taskIds), errMsg));

                        break;
                    }
                case "reverttask":
                    {
                        int syncTaskId;
                        if (int.TryParse(context.Request.QueryString["synctaskid"], out syncTaskId))
                        {
                            //获取该版本下的文件列表
                            var syncProvider = new SyncProvider();
                            IList<VersionFileEntity> fileEntities = syncProvider.GetPreVesionFiles(syncTaskId);
                            if (fileEntities != null && fileEntities.Count > 0)
                            {
                                IEnumerable<int> taskIds = RevertFiles(fileEntities, out errMsg);
                                sb.Append(string.Format("{{\"taskID\": \"{0}\", \"errorMsg\":\"{1}\"}}",
                                                        string.Join(",", taskIds), errMsg));
                            }
                            else
                            {
                                sb.Append(string.Format("{{\"taskID\": \"{0}\", \"errorMsg\":\"{1}\"}}",
                                                        "0", "所选任务无历史文件，通常情况下，这是由该文件第一次同步引起的。"));
                            }

                            #region 修改任务状态为“已回滚” 雷斌添加

                            //if (taskIds != null && taskIds.Count<int>() > 0)
                            //{
                            //    Provider.SyncProvider syncProvider = new SyncProvider();
                            //    int i = 0;
                            //    if (string.IsNullOrEmpty(errMsg))
                            //    {
                            //        i = syncProvider.UpdateTaskStatus(syncTaskId, EnumSyncStatus.Rollback);
                            //    }
                            //    else
                            //    {
                            //        i = syncProvider.UpdateTaskStatus(syncTaskId, EnumSyncStatus.RollbackFailed);
                            //    }
                            //}

                            #endregion 修改任务状态为“已回滚” 雷斌添加
                        }
                        break;
                    }
                case "resumetask":
                    {
                        int syncTaskId;
                        if (int.TryParse(context.Request.QueryString["synctaskid"], out syncTaskId))
                        {
                            //获取该版本下的文件列表
                            var syncProvider = new SyncProvider();
                            IList<VersionFileEntity> fileEntities = syncProvider.GetVersionFiles(syncTaskId);
                            IEnumerable<int> taskIds = RevertFiles(fileEntities, out errMsg);
                            sb.Append(string.Format("{{\"taskID\": \"{0}\", \"errorMsg\":\"{1}\"}}",
                                                    string.Join(",", taskIds), errMsg));

                            //#region 修改任务状态为“已回滚” 雷斌添加

                            //if (taskIds != null && taskIds.Count<int>() > 0)
                            //{
                            //    Provider.SyncProvider syncProvider = new SyncProvider();
                            //    int i = 0;
                            //    if (!string.IsNullOrEmpty(errMsg))
                            //    {
                            //        i = syncProvider.UpdateTaskStatus(syncTaskId, EnumSyncStatus.Succeed);
                            //    }
                            //}

                            //#endregion
                        }
                        break;
                    }
                case "deleteSchedultTask":
                    {
                        int scheduleTaskId;
                        if (int.TryParse(context.Request.QueryString["taskid"], out scheduleTaskId))
                        {
                            var schProvider = new ScheduleTaskProvider();
                            TimerSynctaskEntity timerSyncTask = schProvider.GetTask(scheduleTaskId);

                            bool result = schProvider.DeleteTask(scheduleTaskId);
                            sb.Append("{\"Result\":\"" + result + "\"}");

                            //定义操作日志
                            OperationLogEntity operateLogEntity = new OperationLogEntity()
                            {
                                UserName = CurUsername,
                                DomainName = timerSyncTask.DomainName,
                                OperateType = EnumOperateType.ScheduleTask,
                                Log = string.Format("删除计划任务，计划同步任务Id:{0}，计划时间:{1}，同步原因:{2}", timerSyncTask.TimerSyncTaskId, timerSyncTask.ScheduleTime, timerSyncTask.Description),
                                Result = result
                            };

                            LogProvider logProvider = new LogProvider();
                            logProvider.AddOperateLog(operateLogEntity);
                        }
                    }
                    break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(sb);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion IHttpHandler Members

        /// <summary>
        /// 回滚文件
        /// </summary>
        /// <param name="filelist"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public IEnumerable<int> RevertFiles(IList<VersionFileEntity> filelist, out string errorMsg)
        {
            var taskIds = new List<int>();
            errorMsg = string.Empty;
            List<int> domainIds = filelist.GroupBy(x => x.DomainId).Select(x => x.Key).ToList();
            // 判断demo是否被连接
            var serProvider = new ServerProvider();
            var testProvider = new TestConnectProvider();
            foreach (int domainid in domainIds)
            {
                ServerEntity server = serProvider.GetSourceServerByDomainId(domainid);

                if (!testProvider.TestConnect(server.ServerId))
                {
                    errorMsg = string.Format("服务器[{0}]连接失败，请联系管理员！", server.IP);
                    return taskIds;
                }
            }

            var task = new SyncTaskProcessor();
            foreach (int id in domainIds)
            {
                List<int> fileID = filelist.Where(x => x.DomainId == id).Select(x => x.VersionFileId).ToList();
                taskIds.Add(task.RevertFiles(id, fileID, CurUsername));
            }

            return taskIds;
        }
    }
}