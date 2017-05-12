#region using

using System;
using Dorado.VWS.Model;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class ScheduleTaskInfo : WebBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LinkBefore.Visible = false;
            LinkAfter.Visible = false;

            int domainId;
            int scheduleTaskId;
            var schProvider = new ScheduleTaskProvider();
            TimerSynctaskEntity syncTaskEntity = null;

            // 获取当前需要显示的任务
            if (int.TryParse(Request.QueryString["scheduleTaskId"], out scheduleTaskId))
            {
                syncTaskEntity = schProvider.GetTask(scheduleTaskId);
            }
            else if (int.TryParse(Request.QueryString["domainid"], out domainId))
            {
                syncTaskEntity = schProvider.GetLastTask(domainId);
            }

            if (syncTaskEntity == null)
            {
                return;
            }

            // 任务显示
            LblDomain.Text = syncTaskEntity.DomainName;
            LblUserName.Text = syncTaskEntity.Creator;
            LblDescription.Text = syncTaskEntity.Description;
            LblScheduleTime.Text = syncTaskEntity.ScheduleTime.ToString();

            if (!string.IsNullOrEmpty(syncTaskEntity.AddFiles))
            {
                LtlAddFiles.Text = syncTaskEntity.AddFiles.Replace(",", "<br/>");
            }

            if (!string.IsNullOrEmpty(syncTaskEntity.DelFiles))
            {
                LtlAddFiles.Text = syncTaskEntity.DelFiles.Replace(",", "<br/>");
            }

            // 向前向后显示
            int before = schProvider.GetPreScheduleTaskId(syncTaskEntity.DomainId, syncTaskEntity.TimerSyncTaskId);
            int after = schProvider.GetNextScheduleTaskId(syncTaskEntity.DomainId, syncTaskEntity.TimerSyncTaskId);
            if (before != -1)
            {
                LinkBefore.HRef = string.Format("ScheduleTaskInfo.aspx?domainid={0}&scheduleTaskId={1}",
                                                syncTaskEntity.DomainId, before);
                LinkBefore.Visible = true;
            }
            else
            {
                LinkBefore.Visible = false;
            }
            if (after != -1)
            {
                LinkAfter.HRef = string.Format("ScheduleTaskInfo.aspx?domainid={0}&scheduleTaskId={1}",
                                               syncTaskEntity.DomainId, after);
                LinkAfter.Visible = true;
            }
            else
            {
                LinkAfter.Visible = false;
            }
        }
    }
}