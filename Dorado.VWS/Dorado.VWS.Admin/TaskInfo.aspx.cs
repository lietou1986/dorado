#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dorado.VWS.Model;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class TaskInfo : WebBasePage
    {
        /// <summary>
        ///     定义同步业务逻辑类
        /// </summary>
        private readonly SyncProvider _syncProvider = new SyncProvider();

        protected void Page_Load(object sender, EventArgs e)
        {
            LinkBefore.Visible = false;
            LinkAfter.Visible = false;

            int domainId;
            int syncTaskId;
            SynctaskEntity syncTaskEntity = null;

            // 获取当前需要显示的任务
            if (int.TryParse(Request.QueryString["synctaskid"], out syncTaskId))
            {
                syncTaskEntity = _syncProvider.GetSyncTaskById(syncTaskId);
            }
            else if (int.TryParse(Request.QueryString["domainid"], out domainId))
            {
                syncTaskEntity = _syncProvider.GetLastSyncTaskByDomain(domainId);
            }

            if (syncTaskEntity == null)
            {
                return;
            }

            // 任务显示
            LblDomain.Text = syncTaskEntity.DomainName;
            LblUserName.Text = syncTaskEntity.UserName;
            LblDescription.Text = syncTaskEntity.Description;
            LtlLogInfo.Text = Server.HtmlDecode(syncTaskEntity.LogInfo.Replace("\n\r", "<br/>"));

            if (!string.IsNullOrEmpty(syncTaskEntity.AddFiles))
            {
                var sb = new StringBuilder();
                string[] files = syncTaskEntity.AddFiles.Split(',');
                foreach (string file in files)
                {
                    IList<VersionFileEntity> list = _syncProvider.GetVersionFiles(syncTaskId);
                    string filePath = list.Where(x => x.VersionPath.Contains(file)).First().VersionPath;
                    sb.AppendFormat("{0} <a href='javascript:;' onclick='DownloadFile(\"{1}\",{2},true)'>下载</a><br/>",
                                    filePath, filePath.Replace("\\", "\\\\"), syncTaskEntity.DomainId);
                }
                LtlAddFiles.Text = sb.ToString();
            }

            if (!string.IsNullOrEmpty(syncTaskEntity.DelFiles))
            {
                var sb = new StringBuilder();
                string[] files = syncTaskEntity.DelFiles.Split(',');
                foreach (string file in files)
                {
                    IList<VersionFileEntity> list = _syncProvider.GetVersionFiles(syncTaskId);
                    string filePath = list.Where(x => x.VersionPath.Contains(file)).First().VersionPath;
                    sb.AppendFormat("{0} <a href='javascript:;' onclick='DownloadFile(\"{1}\",{2},true)'>下载</a><br/>",
                                    filePath, filePath.Replace("\\", "\\\\"), syncTaskEntity.DomainId);
                }
                LtlDeleteFiles.Text = sb.ToString();
            }
            LblUpdateTime.Text = syncTaskEntity.UpdateTime.ToString();

            // 向前向后显示
            int before = _syncProvider.GetPrevTaskId(syncTaskEntity.DomainId, syncTaskEntity.TaskId);
            int after = _syncProvider.GetNextTaskId(syncTaskEntity.DomainId, syncTaskEntity.TaskId);
            if (before != -1)
            {
                LinkBefore.HRef = string.Format("TaskInfo.aspx?domainid={0}&synctaskid={1}", syncTaskEntity.DomainId,
                                                before);
                LinkBefore.Visible = true;
            }
            else
            {
                LinkBefore.Visible = false;
            }
            if (after != -1)
            {
                LinkAfter.HRef = string.Format("TaskInfo.aspx?domainId={0}&synctaskid={1}", syncTaskEntity.DomainId,
                                               after);
                LinkAfter.Visible = true;
            }
            else
            {
                LinkAfter.Visible = false;
            }
        }
    }
}