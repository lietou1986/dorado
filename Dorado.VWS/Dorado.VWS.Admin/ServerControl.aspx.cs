#region using

using System;
using System.Collections.Generic;
using System.Net;
using System.Web.UI.WebControls;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class ServerControl : WebBasePage
    {
        private readonly LogProvider _logProvider = new LogProvider();
        private readonly ServerProvider serverProvider = new ServerProvider();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitEnvironmentData();
                InitDomainData();
            }
        }

        private void InitDomainData()
        {
            //ddlDomain.DataSource = serverProvider.GetDomainsByUser(CurUserName, null);

            var domainsD = GetUserDomians(ddlEnvironment.SelectedValue.ToString(), EnumManageType.DailyManage, EnumManageType.ServiceControl);

            ddlDomain.DataSource = domainsD;
            ddlDomain.DataValueField = "DomainId";
            ddlDomain.DataTextField = "DomainName";
            ddlDomain.DataBind();
            ddlDomain.Items.Insert(0, new ListItem("请选择", "-1"));
        }

        protected void ddlDomain_SelectedIndexChanged(object sender, EventArgs e)
        {
            int domainID = Convert.ToInt32(ddlDomain.SelectedValue);

            IList<ServerEntity> serverList = serverProvider.GetTargetServerByDomainId(domainID);
            lvServer.DataSource = serverList;
            lvServer.DataBind();
        }

        protected void lvServer_DataBound(object sender, ListViewItemEventArgs e)
        {
            var lError = e.Item.FindControl("lError") as Label;
            var lbIis = e.Item.FindControl("lbIis") as LinkButton;
            var lbOther = e.Item.FindControl("lbOther") as LinkButton;
            var lbHtmlCompress = e.Item.FindControl("lbHtmlCompress") as LinkButton;
            var lbJSCssCompress = e.Item.FindControl("lbJSCssCompress") as LinkButton;
            DomainEntity domainEntity = serverProvider.GetDomainById(Convert.ToInt32(ddlDomain.SelectedValue));
            var taskSender = new TaskSenderEntity();
            var client = new SocketClient();
            taskSender.TaskCmd = EnumTaskCmd.HELLO;
            taskSender.SourceRoot = serverProvider.GetSourceServerByDomainId(Convert.ToInt32(ddlDomain.SelectedValue)).Root;
            taskSender.WinServiceName = domainEntity.WinServiceName;
            taskSender.IISSiteName = domainEntity.IISSiteName;
            taskSender.TargetIP = lbIis.CommandArgument.Trim();
            taskSender.DomainType = (int)domainEntity.DomainType;
            taskSender.OperatePathType = (int)domainEntity.OperatePathType;
            taskSender.OperatePath = domainEntity.OperatePath;
            TaskResultEntity hello = client.SyncSend(IPAddress.Parse(lbIis.CommandArgument), taskSender);
            if (hello == null)
            {
                lError.Visible = true;
                lbIis.Visible = false;
                lbOther.Visible = false;
                lbHtmlCompress.Visible = false;
                lbJSCssCompress.Visible = false;
                return;
            }

            switch (hello.IISStatus)
            {
                case 1:
                    lbIis.Text = "停止IIS站点服务";
                    lbIis.CommandArgument = "stop_" + lbIis.CommandArgument;
                    break;

                case 2:
                    lbIis.Text = "启动IIS站点服务";
                    lbIis.CommandArgument = "start_" + lbIis.CommandArgument;
                    break;

                case 3:
                    lbIis.Enabled = false;
                    lbIis.Text = "IIS站点状态未知";
                    lbIis.CommandArgument = "start_" + lbIis.CommandArgument;
                    break;

                case 4:
                    lbIis.Enabled = false;
                    lbIis.Text = "IIS站点没有配置";
                    lbIis.CommandArgument = "start_" + lbIis.CommandArgument;
                    break;

                default:
                    lbIis.Enabled = false;
                    lbIis.Text = "其他异常情况";
                    break;
            }

            switch (hello.RelateSvcStatus)
            {
                case 0:
                    lbOther.Enabled = false;

                    lbOther.Text = "没有相关服务";
                    break;

                case 1:
                    lbOther.Text = "停止相关服务";
                    lbOther.CommandArgument = "stop_" + lbOther.CommandArgument;
                    break;

                case 2:
                    lbOther.Text = "启动相关服务";
                    lbOther.CommandArgument = "start_" + lbOther.CommandArgument;
                    break;
            }

            if (!hello.EnableHtmlCompress)
            {
                lbHtmlCompress.Text = "启用HTML压缩";
                lbHtmlCompress.CommandArgument = "start_" + lbHtmlCompress.CommandArgument;
            }
            else
            {
                lbHtmlCompress.Text = "停用HTML压缩";
                lbHtmlCompress.CommandArgument = "stop_" + lbHtmlCompress.CommandArgument;
            }

            if (!hello.EnableJSCssCompress)
            {
                lbJSCssCompress.Text = "启用JsCss压缩";
                lbJSCssCompress.CommandArgument = "start_" + lbJSCssCompress.CommandArgument;
            }
            else
            {
                lbJSCssCompress.Text = "停用JsCss压缩";
                lbJSCssCompress.CommandArgument = "stop_" + lbJSCssCompress.CommandArgument;
            }
        }

        protected void lvServer_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            //定义操作日志
            var operateLogEntity = new OperationLogEntity
                                                      {
                                                          UserName = CurUserName,
                                                          DomainName = ddlDomain.SelectedItem.Text,
                                                          OperateType = EnumOperateType.ServerControl,
                                                      };

            string[] args = Convert.ToString(e.CommandArgument).Split('_');
            var lb = e.CommandSource as LinkButton;
            var client = new SocketClient();
            DomainEntity domainEntity = serverProvider.GetDomainById(Convert.ToInt32(ddlDomain.SelectedValue));
            var taskSender = new TaskSenderEntity();

            //判断用户是否有服务操作权限
            //if (HasResoucePermission(int.Parse(ddlDomain.SelectedValue), "ServiceControl"))
            if (HasDomainPermission(int.Parse(ddlDomain.SelectedValue), EnumManageType.ServiceControl))
            {
                taskSender.WinServiceName = domainEntity.WinServiceName;
                taskSender.IISSiteName = domainEntity.IISSiteName;
                taskSender.DomainType = (int)domainEntity.DomainType;
                taskSender.OperatePathType = (int)domainEntity.OperatePathType;
                taskSender.OperatePath = domainEntity.OperatePath;
                if (e.CommandName == "iisCtrl")
                {
                    taskSender.TaskCmd = args[0] == "start" ? EnumTaskCmd.IISSTART : EnumTaskCmd.IISSTOP;
                    LoggerWrapper.Logger.Info("VWS", string.Format("重启IIS日志", string.Format("{0}，IIS站点：{1}，目标IP:{2}", taskSender.TaskCmd.GetDescription(),
                                                         taskSender.IISSiteName, IPAddress.Parse(args[1]))));
                    operateLogEntity.Log = string.Format("{0}，IIS站点：{1}，目标IP:{2}", taskSender.TaskCmd.GetDescription(),
                                                         taskSender.IISSiteName, IPAddress.Parse(args[1]));
                }
                else
                {
                    taskSender.TaskCmd = args[0] == "start" ? EnumTaskCmd.WINSERVICESTART : EnumTaskCmd.WINSERVICESTOP;
                    LoggerWrapper.Logger.Info("VWS", string.Format("{0}，Winservice服务：{1}，目标IP:{2}",
                                                         taskSender.TaskCmd.GetDescription(), taskSender.WinServiceName,
                                                         IPAddress.Parse(args[1])));
                    operateLogEntity.Log = string.Format("{0}，Winservice服务：{1}，目标IP:{2}",
                                                         taskSender.TaskCmd.GetDescription(), taskSender.WinServiceName,
                                                         IPAddress.Parse(args[1]));
                }

                try
                {
                    TaskResultEntity ret = client.SyncSend(IPAddress.Parse(args[1]), taskSender);

                    if (ret.Success)
                    {
                        operateLogEntity.Result = true;

                        tip.Text = "操作成功！";
                        if (args[0] == "start")
                        {
                            lb.CommandArgument = "stop_" + args[1];
                            lb.Text = lb.Text.Replace("启动", "停止");
                        }
                        else
                        {
                            lb.CommandArgument = "start_" + args[1];
                            lb.Text = lb.Text.Replace("停止", "启动");
                        }
                    }
                    else
                    {
                        operateLogEntity.Result = false;
                        tip.Text = "操作失败！" + ret.ErrorMsg;
                    }
                }
                catch
                {
                    operateLogEntity.Result = false;

                    tip.Text = "操作失败！客户端服务可能已经关闭！";
                    return;
                }

                #region 压缩控制

                if (e.CommandName == "htmlCompressCtrl" || e.CommandName == "jsCssCompressCtrl")
                {
                    var lbHtmlCompress = e.Item.FindControl("lbHtmlCompress") as LinkButton;
                    var lbJSCssCompress = e.Item.FindControl("lbJSCssCompress") as LinkButton;
                    string[] htmlArgs = lbHtmlCompress.CommandArgument.Split('_');
                    string[] JSCssArgs = lbJSCssCompress.CommandArgument.Split('_');

                    int html = htmlArgs[0] == "start" ? 0 : 1, jsCss = JSCssArgs[0] == "start" ? 0 : 10;
                    if (lb.ID == lbHtmlCompress.ID) html = htmlArgs[0] == "start" ? 1 : 0;
                    if (lb.ID == lbJSCssCompress.ID) jsCss = JSCssArgs[0] == "start" ? 10 : 0;
                    switch (html | jsCss)
                    {
                        case 0:
                            taskSender.CompressType = EnumCompresssType.NoCommpress;
                            break;

                        case 1:
                            taskSender.CompressType = EnumCompresssType.HtmlCompress;
                            break;

                        case 10:
                            taskSender.CompressType = EnumCompresssType.JsCssCompress;
                            break;

                        case 11:
                            taskSender.CompressType = EnumCompresssType.HtmlJsCssCompress;
                            break;

                        default:
                            throw (new Exception("不可接受的压缩选项。"));
                    }

                    taskSender.SourceRoot =
                        serverProvider.GetSourceServerByDomainId(Convert.ToInt32(ddlDomain.SelectedValue)).Root;
                    taskSender.TaskCmd = EnumTaskCmd.COMPRESSFILES;
                    TaskResultEntity ret = client.SyncSend(IPAddress.Parse(args[1]), taskSender);

                    operateLogEntity.Log = string.Format("{0}，CommandName：{1}，目标IP:{2}",
                                                         taskSender.CompressType.GetDescription(), e.CommandName,
                                                         IPAddress.Parse(args[1]));
                    if (ret.Success)
                    {
                        operateLogEntity.Result = true;

                        tip.Text = "操作成功！";
                        if (args[0] == "start")
                        {
                            lb.CommandArgument = "stop_" + args[1];
                            lb.Text = lb.Text.Replace("启用", "停用");
                        }
                        else
                        {
                            lb.CommandArgument = "start_" + args[1];
                            lb.Text = lb.Text.Replace("停用", "启用");
                        }
                    }
                    else
                    {
                        operateLogEntity.Result = false;

                        tip.Text = "操作失败！" + ret.ErrorMsg;
                    }
                }

                #endregion 压缩控制
            }
            else
            {
                operateLogEntity.Result = false;
                ShowAlert("对不起，您没有权限，请联系管理员!");
            }
            _logProvider.AddOperateLog(operateLogEntity);
        }

        protected void ddlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitDomainData();
        }

        private void InitEnvironmentData()
        {
            ddlEnvironment.Items.Insert(0, new ListItem("请选择", EnvironmentType.UnSelected));
            //ddlEnvironment.Items.Insert(1, new ListItem("开发", EnvironmentType.Development));
            //ddlEnvironment.Items.Insert(2, new ListItem("测试", EnvironmentType.Testing));
            ddlEnvironment.Items.Insert(1, new ListItem("验收", EnvironmentType.Acceptance));
            //ddlEnvironment.Items.Insert(4, new ListItem("预上线", EnvironmentType.Advanced));
            ddlEnvironment.Items.Insert(2, new ListItem("线上", EnvironmentType.Production));
        }
    }
}