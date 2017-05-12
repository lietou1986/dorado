#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class TaskRevert : WebBasePage
    {
        private readonly SyncProvider _syncProvider = new SyncProvider();
        private readonly ServerProvider _serverProvider = new ServerProvider();
        private readonly TestConnectProvider _testProvider = new TestConnectProvider();
        private readonly SyncTaskProcessor _stp = new SyncTaskProcessor();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                InitEnvironmentData();
        }

        /// <summary>
        /// 回滚并更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSyncRevert_Click(object sender, EventArgs e)
        {
            int domainID = 0;
            if (!int.TryParse(hfDomainID.Value, out domainID))
            {
                ShowAlert("请选择域名！");
                return;
            }

            int syncTaskId = 0;
            if (!int.TryParse(hfSyncTaskID.Value, out syncTaskId))
            {
                ShowAlert("请选择任务！");
                return;
            }

            string errMsg = string.Empty;
            StringBuilder sb = new StringBuilder();

            SynctaskEntity syncTaskEntity = _syncProvider.GetSyncTaskById(syncTaskId);
            string adds = syncTaskEntity.AddFiles;
            string dels = syncTaskEntity.DelFiles;
            if (!string.IsNullOrEmpty(adds))
            {
                sb.AppendLine("本任务添加的文件如下:<br/>");
                sb.AppendLine(adds.Replace(",", "<br/>"));
            }
            if (!string.IsNullOrEmpty(dels))
            {
                sb.AppendLine("本任务删除的文件如下:<br/>");
                sb.AppendLine(dels.Replace(",", "<br/>"));
            }
            lbFiles.Text = sb.ToString();

            IList<VersionFileEntity> fileEntities = _syncProvider.GetPreVesionFiles(syncTaskId);
            Handler.SubmitHandler sh = new Handler.SubmitHandler();
            IEnumerable<int> taskIds = sh.RevertFiles(fileEntities, out errMsg);

            if (!string.IsNullOrEmpty(errMsg))
            {
                ShowAlert(errMsg);
                return;
            }

            if (fileEntities == null || fileEntities.Count < 1)
            {
                ShowAlert("所选任务无文件");
                return;
            }

            if (taskIds == null || taskIds.Count() < 1)
            {
                ShowAlert("未产生回滚任务");
                return;
            }

            //等待回滚结果
            TaskProvider taskProvider = new TaskProvider();
            bool isRuning = true;
            while (isRuning)
            {
                Thread.Sleep(1000);
                foreach (int taskID in taskIds)
                {
                    TaskEntity taskEntity = taskProvider.GetTask(taskID);
                    if (taskEntity.TaskStatus == Model.Enum.EnumTaskStatus.Successed)
                    {
                        isRuning = false;
                    }
                    else if (taskEntity.TaskStatus == Model.Enum.EnumTaskStatus.Failed)
                    {
                        ShowAlert("回滚出错！");
                        return;
                    }
                    else
                    {
                        isRuning = true;
                        break;
                    }
                }
            }

            //将开始同步
            NewSync(domainID, syncTaskId, adds, dels, "回滚");
        }

        /// <summary>
        /// 恢复并更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSyncResume_Click(object sender, EventArgs e)
        {
            int domainID = 0;
            if (!int.TryParse(hfDomainID.Value, out domainID))
            {
                ShowAlert("请选择域名！");
                return;
            }

            int syncTaskId = 0;
            if (!int.TryParse(hfSyncTaskID.Value, out syncTaskId))
            {
                ShowAlert("请选择任务！");
                return;
            }

            string errMsg = string.Empty;
            StringBuilder sb = new StringBuilder();

            SynctaskEntity syncTaskEntity = _syncProvider.GetSyncTaskById(syncTaskId);
            string adds = syncTaskEntity.AddFiles;
            string dels = syncTaskEntity.DelFiles;
            if (!string.IsNullOrEmpty(adds))
            {
                sb.AppendLine("本任务添加的文件如下:<br/>");
                sb.AppendLine(adds.Replace(",", "<br/>"));
            }
            if (!string.IsNullOrEmpty(dels))
            {
                sb.AppendLine("本任务删除的文件如下:<br/>");
                sb.AppendLine(dels.Replace(",", "<br/>"));
            }
            lbFiles.Text = sb.ToString();

            IList<VersionFileEntity> fileEntities = _syncProvider.GetVersionFiles(syncTaskId);
            Handler.SubmitHandler sh = new Handler.SubmitHandler();
            IEnumerable<int> taskIds = sh.RevertFiles(fileEntities, out errMsg);

            if (!string.IsNullOrEmpty(errMsg))
            {
                ShowAlert(errMsg);
                return;
            }

            if (fileEntities == null || fileEntities.Count < 1)
            {
                ShowAlert("所选任务无历史文件，通常情况下，这是由该文件第一次同步引起的。");
                return;
            }

            if (taskIds == null || taskIds.Count() < 1)
            {
                ShowAlert("未产生恢复任务");
                return;
            }

            //等待回滚结果
            TaskProvider taskProvider = new TaskProvider();
            bool isRuning = true;
            while (isRuning)
            {
                Thread.Sleep(1000);
                foreach (int taskID in taskIds)
                {
                    TaskEntity taskEntity = taskProvider.GetTask(taskID);
                    if (taskEntity.TaskStatus == Model.Enum.EnumTaskStatus.Successed)
                    {
                        isRuning = false;
                    }
                    else if (taskEntity.TaskStatus == Model.Enum.EnumTaskStatus.Failed)
                    {
                        ShowAlert("恢复出错！");
                        return;
                    }
                    else
                    {
                        isRuning = true;
                        break;
                    }
                }
            }

            //将开始同步
            NewSync(domainID, syncTaskId, adds, dels, "恢复");
        }

        private void InitEnvironmentData()
        {
            DdlEnvironment.Items.Insert(0, new ListItem("请选择", EnvironmentType.UnSelected));
            //DdlEnvironment.Items.Insert(1, new ListItem("开发", EnvironmentType.Development));
            //DdlEnvironment.Items.Insert(2, new ListItem("测试", EnvironmentType.Testing));
            DdlEnvironment.Items.Insert(1, new ListItem("验收", EnvironmentType.Acceptance));
            //DdlEnvironment.Items.Insert(4, new ListItem("预上线", EnvironmentType.Advanced));
            DdlEnvironment.Items.Insert(2, new ListItem("线上", EnvironmentType.Production));
        }

        protected void DdlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            DdlDomains.DataSource = _serverProvider.GetDomainsByUser(CurUserName, 1, DdlEnvironment.SelectedValue);

            DdlDomains.DataValueField = "DomainId";
            DdlDomains.DataTextField = "DomainName";
            DdlDomains.DataBind();
            DdlDomains.Items.Insert(0, new ListItem("请选择", "0"));
        }

        /// <summary>
        /// 建立新的同步
        /// </summary>
        /// <param name="domainID"></param>
        /// <param name="syncTaskId"></param>
        /// <param name="adds"></param>
        /// <param name="dels"></param>
        /// <param name="operText"></param>
        /// <returns></returns>
        private bool NewSync(int domainID, int syncTaskId, string adds, string dels, string operText)
        {
            //测试连接
            ServerEntity sourceServer = _serverProvider.GetSourceServerByDomainId(domainID);
            bool success = _testProvider.TestConnect(sourceServer.ServerId);

            if (!success)
            {
                ShowAlert(string.Format("同步源服务器[{0}]连接失败，请联系管理员！", sourceServer.IP));
                return false;
            }

            IList<ServerEntity> targetServerList = _serverProvider.GetTargetServerByDomainId(domainID);
            List<int> targetids = new List<int>();
            foreach (ServerEntity targetServer in targetServerList)
            {
                success = _testProvider.TestConnect(targetServer.ServerId);
                if (!success)
                {
                    ShowAlert(string.Format("同步宿服务器[{0}]连接失败，请联系管理员！", targetServer.IP));
                    return false;
                }
                targetids.Add(targetServer.ServerId);
            }

            //新建同步任务
            int newSyncTaskID = _stp.AddSyncTask(domainID, CurUserName, operText + "任务【" + syncTaskId + "】", adds, dels);
            _stp.SyncTaskTargets(newSyncTaskID, targetids, CurUserName);

            hfNewSyncTaskID.Value = newSyncTaskID.ToString();

            //读取同步任务状态
            SynctaskEntity newSynctaskEntity = _syncProvider.GetSyncTaskById(newSyncTaskID);
            bool isRuning = true;
            DateTime t = DateTime.Now;
            while (isRuning)
            {
                if ((DateTime.Now - t).TotalSeconds > 60)
                {
                    ShowAlert("更新超时");
                    return false;
                }

                if (newSynctaskEntity == null)
                {
                    ShowAlert("新的同步任务未创建成功！");
                    return false;
                }

                if (newSynctaskEntity.SyncStatus == Model.Enum.EnumSyncStatus.Running)
                {
                    Thread.Sleep(1000);
                    newSynctaskEntity = _syncProvider.GetSyncTaskById(newSyncTaskID);
                }
                else
                {
                    isRuning = false;
                    break;
                }
            }

            string result = string.Empty;
            switch (newSynctaskEntity.SyncStatus)
            {
                case Model.Enum.EnumSyncStatus.Failed:
                    result = "更新失败";
                    break;

                case Model.Enum.EnumSyncStatus.Rollback:
                    result = "更新失败，被回滚";
                    break;

                case Model.Enum.EnumSyncStatus.RollbackFailed:
                    result = "更新失败，回滚失败";
                    break;

                case Model.Enum.EnumSyncStatus.Running:
                    result = "更新中";
                    break;

                case Model.Enum.EnumSyncStatus.Succeed:
                    result = "更新成功";
                    break;

                case Model.Enum.EnumSyncStatus.Suspend:
                    result = "同步挂起";
                    break;

                default:
                    result = "状态未知";
                    break;
            }

            Page.ClientScript.RegisterStartupScript(typeof(string), "Refresh", "<script> SelectDomain('" + domainID + "');</script>");
            ShowAlert(result);

            return true;
        }
    }
}