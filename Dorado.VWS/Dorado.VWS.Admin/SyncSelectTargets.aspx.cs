#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class SyncSelectTargets : WebBasePage, ISyncTaskParams
    {
        private readonly LogProvider _logProvider = new LogProvider();
        private readonly ScheduleTaskProvider _schProvider = new ScheduleTaskProvider();
        private readonly ServerProvider _serverProvider = new ServerProvider();
        private readonly SyncTaskProcessor _stp = new SyncTaskProcessor();
        private readonly SyncProvider _syncProvider = new SyncProvider();
        private readonly TestConnectProvider _testProvider = new TestConnectProvider();

        #region ISyncTaskParams Members

        public SyncTaskParams Params { get; set; }

        #endregion ISyncTaskParams Members

        private int domainID;//域名id

        protected void Page_Load(object sender, EventArgs e)
        {
            //在页面加载时设置页面的缓存为“SetNoStore()”，即无缓存
            //Response.Cache.SetNoStore();

            if (string.IsNullOrWhiteSpace(Request.QueryString["param"]))
            {
                ShowAlertAndRedirect("参数错误,请选择域名", "SyncSelectFiles.aspx");
                return;
            }
            else
            {
                //string p = Des.DESDecrypt(Server.UrlDecode(Request.QueryString["param"]), "123", "456");
                //string p = AESHelper.AESDecrypt(Request.QueryString["param"]);
                //Params = new JavaScriptSerializer().Deserialize<SyncTaskParams>(p);
                //Params = ((ISyncTaskParams)Context.Handler).Params;
                if (Params == null)
                {
                    Params = (SyncTaskParams)Session[Request.QueryString["param"] + "_SyncTaskParams"];
                }
            }
            if (Params == null)
            {
                ShowAlertAndRedirect("参数错误,请选择域名", "SyncSelectFiles.aspx");
                return;
            }

            //if (string.IsNullOrWhiteSpace(Request.QueryString["param"]))
            //{
            //    ShowAlertAndRedirect("参数错误", "SyncSelectFiles.aspx");
            //    //Response.End();
            //}
            //else
            //{
            //    Params = new JavaScriptSerializer().Deserialize<SyncTaskParams>(Request.QueryString["param"]);

            //}

            if (!IsPostBack)
            {
                //Session[Params.DomainId + "_SyncTaskParams"] = Params;
                //ViewState["SyncTaskParams"] = Params;
                DomainEntity domainEntity = _serverProvider.GetDomainById(Params.DomainId);
                hdfDomain.Value = domainEntity.DomainName;

                #region 设置添加删除列表

                if (Params.AddFiles != null && Params.AddFiles.Count > 0)
                {
                    LtlAddFiles.Text = string.Join("<br>", Params.AddFiles);
                }
                if (Params.DelFiles != null && Params.DelFiles.Count > 0)
                {
                    LtlDelFiles.Text = string.Join("<br>", Params.DelFiles);
                }
                HfTaskid.Value = Params.TaskId.ToString();
                if (Params.TaskId != 0)
                {
                    ShowAlert("您的同步任务正在同步……");
                    Page.ClientScript.RegisterStartupScript(typeof(string), "Refresh",
                                                            "<script>getsyncresult();</script>");
                }

                #endregion 设置添加删除列表

                #region 设置同步宿列表

                IList<ServerEntity> targetServers = _serverProvider.GetTargetServerByDomainId(Params.DomainId);
                IList<SynctaskSubEntity> syncTaskSubEntities = new List<SynctaskSubEntity>();
                if (Params.TaskId != 0)
                {
                    syncTaskSubEntities = _syncProvider.GetSubTask(Params.TaskId);

                    TxtDescription.Text = _syncProvider.GetSyncTaskById(Params.TaskId).Description;
                    TxtDescription.Enabled = false;
                }

                SynctaskSubEntity subEntity;

                var list = from t in targetServers
                           select new
                                      {
                                          t.ServerId,
                                          t.IP,
                                          t.IsAdvanced,
                                          t.HostName,
                                          Status =
                               ((subEntity = syncTaskSubEntities.FirstOrDefault(s => s.SyncServerId == t.ServerId)) ==
                                null
                                    ? "正常"
                                    : subEntity.SyncStatus.GetDescription()),
                                          IsConnect =
                               ((subEntity = syncTaskSubEntities.FirstOrDefault(s => s.SyncServerId == t.ServerId)) ==
                                null)
                                      };

                RptServers.DataSource = list;
                RptServers.DataBind();

                #endregion 设置同步宿列表

                #region 判断是否有计划任务权限

                trScheduleTask.Visible = false;
                trScheduleTaskLink.Visible = false;
                if (Params.TaskId <= 0)
                {
                    //bool hasPms = HasResoucePermission(Params.DomainId, "ScheduleTask");
                    bool hasPms = HasDomainPermission(Params.DomainId, EnumManageType.ScheduleTask);
                    trScheduleTaskLink.Visible = hasPms;
                    trScheduleTask.Visible = hasPms;
                }

                #endregion 判断是否有计划任务权限

                //pTH.Visible = HasResoucePermission(Params.DomainId, "ServiceControl");
                pTH.Visible = HasDomainPermission(Params.DomainId, EnumManageType.ServiceControl);
                pTH1.Visible = pTH.Visible;
            }
        }

        protected void BtnSync_Click(object sender, EventArgs e)
        {
            //if (Session["Sync"] != null && (bool)Session["Sync"])
            //{
            //    Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('不要刷新或重复提交，回到域名页面重新选择域名！');", true);
            //    BtnSync.Enabled = false;
            //    return;
            //}

            trScheduleTask.Visible = false;
            // 判断是否文件有被正在使用中
            IList<SynctaskEntity> tasks = _stp.GetProceTasksByDomainAndFiles(Params.DomainId,
                                                                             Params.AddFiles.Concat(Params.DelFiles).
                                                                                 ToArray());

            if (tasks.Count > 0 && !tasks[0].UserName.Equals(CurUserName))
            {
                ShowAlert(string.Format("您选择的文件被其它人[{0}]处理中，请稍候！", tasks[0].UserName));
                return;
            }

            IList<int> targetids = new List<int>();
            foreach (RepeaterItem item in RptServers.Items)
            {
                var chb = (CheckBox)(item.FindControl("CkbSync"));
                if (chb.Checked)
                {
                    var hfServerId = (HiddenField)item.FindControl("HfServerId");
                    int nServerId = int.Parse(hfServerId.Value);

                    targetids.Add(nServerId);
                    chb.Checked = false;
                    chb.Visible = false;
                }
            }

            if (targetids.Count > 0)
            {
                //测试连接
                ServerEntity sourceServer = _serverProvider.GetSourceServerByDomainId(Params.DomainId);
                bool success = _testProvider.TestConnect(sourceServer.ServerId);

                if (!success)
                {
                    ShowAlert(string.Format("服务器[{0}]连接失败，请联系管理员！", sourceServer.IP));
                    return;
                }

                foreach (var targetid in targetids)
                {
                    success = _testProvider.TestConnect(targetid);

                    if (!success)
                    {
                        ServerEntity server = _serverProvider.GetServerById(targetid);
                        ShowAlert(string.Format("服务器[{0}]连接失败，请联系管理员！", server.IP));
                        return;
                    }
                }

                SyncTargets(targetids);
                TxtDescription.Enabled = false;
                HfTaskid.Value = Params.TaskId.ToString();
                Page.ClientScript.RegisterStartupScript(typeof(string), "Refresh",
                                                        " <script>setTimeout(\"getsyncresult()\",1000);</script>");
            }
            else
            {
                ShowAlert("请选择同步目标");
            }

            BtnSync.Enabled = false;
            //Session["Sync"] = true;
            //Session[Params.GetHashCode()+"_SyncTaskParams"] = Params;
            //Server.Transfer("SyncSelectTargets.aspx");
            //Response.Redirect("SyncSelectTargets.aspx?param=" + Params.GetHashCode());

            //string param = new JavaScriptSerializer().Serialize(Params);
            //param = AESHelper.AESEncrypt(param);
            //Response.Redirect("SyncSelectTargets.aspx?param=" + Server.UrlEncode(para m), true);

            //保存数据，然后跳转，防止刷新重复提交
            string randid = RandomStr.GetRndStrOnlyFor(16);
            Session[randid + "_SyncTaskParams"] = Params;
            Response.Redirect("SyncSelectTargets.aspx?param=" + randid, true);
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            if (Params != null && Params.TaskId != 0)
            {
                //测试连接
                ServerEntity sourceServer = _serverProvider.GetSourceServerByDomainId(Params.DomainId);
                bool success = _testProvider.TestConnect(sourceServer.ServerId);

                if (!success)
                {
                    ShowAlert(string.Format("服务器[{0}]连接失败，请联系管理员！", sourceServer.IP));
                    return;
                }
                IList<SynctaskSubEntity> synctaskSubEntities = _syncProvider.GetSubTask(Params.TaskId);
                foreach (var synctaskSub in synctaskSubEntities)
                {
                    success = _testProvider.TestConnect(synctaskSub.SyncServerId);

                    if (!success)
                    {
                        ServerEntity server = _serverProvider.GetServerById(synctaskSub.SyncServerId);
                        ShowAlert(string.Format("服务器[{0}]连接失败，请联系管理员！", server.IP));
                        return;
                    }
                }

                _stp.CancelSyncTask(Params.TaskId, CurUserName);
                HfTaskid.Value = Params.TaskId.ToString();
                Page.ClientScript.RegisterStartupScript(typeof(string), "Refresh",
                                                        " <script>setTimeout(\"getsyncresult()\",1000);</script>");
            }

            //Server.Transfer("SyncSelectTargets.aspx");
        }

        private void SyncTargets(IList<int> targetids)
        {
            // 任务未建立时，先建立任务
            if (Params.TaskId == 0)
            {
                Params.TaskId = _stp.AddSyncTask(Params.DomainId, CurUserName, TxtDescription.Text.Trim(),
                                                 Params.AddFiles, Params.DelFiles);
            }

            // 同步目标
            if (Params.TaskId != 0)
            {
                if (_syncProvider.ExsitSubTask(Params.TaskId, targetids.ToArray()))
                {
                    Page.ClientScript.RegisterStartupScript(typeof(string), "exsitsubalert",
                                                        " <script>alert('任务已经存在，重复提交');</script>");
                    return;
                }
                else
                {
                    _stp.SyncTaskTargets(Params.TaskId, targetids, CurUserName);
                }
            }
            //return Params.TaskId;
        }

        protected string GetOperate(string ip, bool isIis, int index)
        {
            string retStr = string.Empty;
            DomainEntity domainEntity = _serverProvider.GetDomainById(Params.DomainId);

            var taskSender = new TaskSenderEntity();
            var client = new SocketClient();
            taskSender.TaskCmd = EnumTaskCmd.HELLO;
            taskSender.SourceRoot = _serverProvider.GetSourceServerByDomainId(Params.DomainId).Root;
            taskSender.WinServiceName = domainEntity.WinServiceName;
            taskSender.IISSiteName = domainEntity.IISSiteName;
            taskSender.TargetIP = ip.Trim();
            taskSender.DomainType = (int)domainEntity.DomainType;
            taskSender.OperatePathType = (int)domainEntity.OperatePathType;
            taskSender.OperatePath = domainEntity.OperatePath;
            TaskResultEntity hello = client.SyncSend(IPAddress.Parse(ip), taskSender);
            if (hello == null) return "无法连接服务器";

            if (isIis)
            {
                switch (hello.IISStatus)
                {
                    case 1:
                        retStr =
                            string.Format(
                                "<a id='iis{0}' href='javascript:;' onclick='serviceCtrl({0}, \"{1}\", \"{2}\", \"{3}\", false, true)'>停止IIS站点服务</a>",
                                index, ip, taskSender.IISSiteName, domainEntity.DomainName);
                        break;

                    case 2:
                        retStr =
                            string.Format(
                                "<a id='iis{0}' href='javascript:;' onclick='serviceCtrl({0}, \"{1}\", \"{2}\", \"{3}\", true, true)'>启动IIS站点服务</a>",
                                index, ip, taskSender.IISSiteName, domainEntity.DomainName);
                        break;

                    case 3:
                        retStr = "IIS站点状态未知";
                        break;

                    case 4:
                        retStr = "IIS站点没有配置";
                        break;

                    default:
                        retStr = "其他异常情况";
                        break;
                }
            }
            else
            {
                switch (hello.RelateSvcStatus)
                {
                    case 0:
                        retStr = "没有相关服务";
                        break;

                    case 1:
                        retStr =
                            string.Format(
                                "<a id='other{0}' href='javascript:;' onclick='serviceCtrl({0}, \"{1}\", \"{2}\", \"{3}\", false, false)'>停止相关服务</a>",
                                index, ip, taskSender.WinServiceName, domainEntity.DomainName);
                        break;

                    case 2:
                        retStr =
                            string.Format(
                                "<a id='other{0}' href='javascript:;' onclick='serviceCtrl({0}, \"{1}\", \"{2}\", \"{3}\", true, false)'>启动相关服务</a>",
                                index, ip, taskSender.WinServiceName, domainEntity.DomainName);
                        break;
                }
            }
            return retStr;
        }

        protected void pTD_Load(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null) panel.Visible = pTH.Visible;
        }

        protected void BtnScheduleSync_Click(object sender, EventArgs e)
        {
            DateTime scheduleTime = DateTime.Parse(txtScheduleTime.Value);
            int taskid = _schProvider.AddTask(CurUserName, scheduleTime, Params.DomainId, Params.AddFiles, Params.DelFiles,
                                         TxtDescription.Text.Trim());

            DomainEntity domainEntity = _serverProvider.GetDomainById(Params.DomainId);
            //定义操作日志
            var operateLogEntity = new OperationLogEntity
                                                      {
                                                          UserName = CurUserName,
                                                          DomainName = domainEntity.DomainName,
                                                          OperateType = EnumOperateType.ScheduleTask,
                                                          Log =
                                                              string.Format("添加计划任务，计划同步任务Id:{0}，计划时间:{1}，同步原因:{2}", taskid,
                                                                            scheduleTime, TxtDescription.Text.Trim()),
                                                      };

            if (taskid > 0)
            {
                operateLogEntity.Result = true;
                ShowAlertAndRedirect("任务添加成功", "ScheduleTasks.aspx");
            }
            else
            {
                operateLogEntity.Result = false;
                ShowAlert("任务添加失败");
            }
            _logProvider.AddOperateLog(operateLogEntity);
        }

        //protected void RptServers_Bound(object sender, RepeaterItemEventArgs e)
        //{
        //    var ckbSync =(CheckBox) e.Item.FindControl("ckbSync");

        //    dynamic data=  e.Item.DataItem;
        //    if (data != null)
        //    {
        //        ckbSync.Attributes.Add("serverip", data.IP);
        //        ckbSync.Attributes.Add("serverid", data.ServerId.ToString());
        //    }
        //}
        //private class server
        //{
        //  public  string IP;
        //  public int ServerID;
        //}
    }
}