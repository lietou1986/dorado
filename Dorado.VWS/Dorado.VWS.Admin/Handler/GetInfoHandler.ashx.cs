#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

//using Dorado.VWS.Admin.PermissionWCF;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    ///     获取信息的Handler
    /// </summary>
    public class GetInfoHandler : IHttpHandler
    {
        private LogProvider logProvider = new LogProvider();

        private WebBasePage basePage = new WebBasePage();

        protected string CurUserName
        {
            get
            {
                return WebBasePage.GetUserName();
            }
        }

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            //string userName = context.Session["UserName"].ToString();
            var sb = new StringBuilder();
            switch (context.Request.QueryString["action"])
            {
                //获取有权限用户域名【普通同步类型】
                case "domains":
                    {
                        int selDomainId = 0;
                        // 设置Cookie中的域名
                        var httpCookie = context.Request.Cookies["tmpdomainid"];
                        if (httpCookie != null)
                            int.TryParse(httpCookie.Value, out selDomainId);

                        var serverProvider = new ServerProvider();
                        var list =
                            serverProvider.GetDomainsByUser(CurUserName, 1).Select(
                                x => new { name = x.DomainName, id = x.DomainId, selected = (x.DomainId == selDomainId) });
                        new JavaScriptSerializer().Serialize(list, sb);
                    }
                    break;

                case "alldomains":
                    {
                        var serverProvider = new ServerProvider();
                        var domainlist =
                            serverProvider.GetDomainsByIdcId(0).Select(x => new { name = x.DomainName, id = x.DomainId });
                        new JavaScriptSerializer().Serialize(domainlist, sb);
                    }
                    break;

                case "task":
                    {
                        int taskid;
                        if (int.TryParse(context.Request.QueryString["taskid"], out taskid))
                        {
                            var taskProvider = new TaskProvider();
                            TaskEntity taskEntity = taskProvider.GetTask(taskid);

                            if (taskEntity != null)
                            {
                                new JavaScriptSerializer().Serialize(taskEntity, sb);
                            }
                        }
                    }
                    break;

                case "synctask":
                    {
                        int taskid;
                        if (int.TryParse(context.Request.Params["taskid"], out taskid))
                        {
                            var syncProvider = new SyncProvider();
                            SynctaskEntity synctask = syncProvider.GetSyncTaskById(taskid);

                            if (synctask != null)
                            {
                                sb.Append("{");
                                sb.AppendFormat("\"Id\":{0},", synctask.TaskId);
                                sb.AppendFormat("\"Status\":\"{0}\",", synctask.SyncStatus);
                                sb.AppendFormat("\"Msg\":\"{0}\"",
                                                synctask.LogInfo.Replace("\n\r", "<br/>").Replace("\\", "\\\\"));
                                IList<SynctaskSubEntity> syncsubtasks = syncProvider.GetSubTask(taskid);
                                if (syncsubtasks != null && syncsubtasks.Count != 0)
                                {
                                    sb.Append(",\"Subtasks\":[");
                                    bool bFirst = true;
                                    foreach (var item in syncsubtasks)
                                    {
                                        if (!bFirst)
                                        {
                                            sb.Append(",");
                                        }

                                        bFirst = false;
                                        sb.Append("{");
                                        sb.AppendFormat("\"ServerId\":{0},", item.SyncServerId);
                                        sb.AppendFormat("\"Status\":\"{0}\"", item.SyncStatus.GetDescription());
                                        sb.Append("}");
                                    }
                                    sb.Append("]");
                                }
                                sb.Append("}");
                            }
                        }
                    }
                    break;

                case "sucsynctasks":
                    {
                        //domainid, begin, end
                        int domainid, begin, end;
                        int count = 0;
                        //以下增加参数读取所有同步历史
                        bool viewALL = false;
                        if (!string.IsNullOrWhiteSpace(context.Request.Params["view"]) && context.Request.Params["view"].ToLower() == "all")
                        {
                            viewALL = true;
                        }
                        if (int.TryParse(context.Request.Params["domainid"], out domainid)
                            && int.TryParse(context.Request.Params["begin"], out begin)
                            && int.TryParse(context.Request.Params["end"], out end))
                        {
                            sb.Append("{\"Version\":");
                            var syncProvider = new SyncProvider();
                            //var pmsProvider = new PermissionProvider();
                            //if (pmsProvider.HasResoucePermission(CurUserName, domainid, "TaskRevertOwner")) //老的资源权限
                            //新的资源权限
                            var pmsProvider = new DomainPermissionProvider();
                            //if (pmsProvider.HasPermission(domainid, CurUserName,EnumManageType.RollBackALL))
                            if (pmsProvider.HasPermission(domainid, CurUserName, EnumManageType.RollBackALL) || viewALL)//增加参数读取所有同步历史
                            {
                                var obj = syncProvider.GetSucessSyncTaskByDomain(domainid, begin, end);
                                new JavaScriptSerializer().Serialize(obj, sb);
                                count = syncProvider.GetSucessSyncTaskCount(domainid);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(CurUserName))
                                {
                                    var obj = syncProvider.GetSucessSyncTaskByDomain(domainid, CurUserName, begin, end);
                                    new JavaScriptSerializer().Serialize(obj, sb);
                                    count = syncProvider.GetSucessSyncTaskCount(domainid, CurUserName);
                                }
                            }

                            sb.Append(",\"Count\":" + count + "}");
                        }
                    }
                    break;

                case "versionfiles":
                    {
                        var syncProvider = new SyncProvider();
                        var serProvider = new ServerProvider();
                        //domainid, filepath, begin, end
                        string filepath = context.Request.Params["filepath"];
                        int domainid, begin, end;
                        if (!string.IsNullOrEmpty(filepath) &&
                            int.TryParse(context.Request.Params["domainid"], out domainid)
                            && int.TryParse(context.Request.Params["begin"], out begin)
                            && int.TryParse(context.Request.Params["end"], out end))
                        {
                            sb.Append("{\"Version\":");
                            ServerEntity source = serProvider.GetSourceServerByDomainId(domainid);

                            filepath = filepath.Substring(source.Root.Length);

                            var objs = syncProvider.GetVersionFiles(domainid, filepath, begin, end);
                            int count = syncProvider.GetVersionCount(domainid, filepath);
                            new JavaScriptSerializer().Serialize(objs, sb);
                            sb.Append(",\"Count\":" + count + "}");
                        }
                    }
                    break;
                //add by han
                case "SyncExceptionList":
                    {
                        SyncProvider syncProvider = new SyncProvider();
                        int begin, end;
                        string domainname = context.Request.Params["domainname"];
                        string operatetype = context.Request.Params["operatetype"];
                        string username = context.Request.Params["queryusername"];

                        if (string.IsNullOrEmpty(domainname))
                        {
                            domainname = null;
                        }

                        if (string.IsNullOrEmpty(operatetype))
                        {
                            operatetype = null;
                        }

                        if (string.IsNullOrEmpty(username))
                        {
                            username = null;
                        }

                        DateTime startdate = Convert.ToDateTime("1900-1-1");
                        DateTime enddate = Convert.ToDateTime("2500-1-1");
                        if (context.Request.Params["startdate"] != null && context.Request.Params["enddate"] != null)
                        {
                            if (!string.IsNullOrEmpty(context.Request.Params["startdate"].Trim()))
                            {
                                startdate = Convert.ToDateTime(context.Request.Params["startdate"]);
                            }
                            if (!string.IsNullOrEmpty(context.Request.Params["enddate"].Trim()))
                            {
                                enddate = Convert.ToDateTime(context.Request.Params["enddate"]);
                            }
                        }

                        if (int.TryParse(context.Request.Params["begin"], out begin)
                            && int.TryParse(context.Request.Params["end"], out end))
                        {
                            sb.Append("{\"SyncExceptionList\":");

                            var objs = syncProvider.GetAllExceptionSyncTask(domainname, operatetype, username, startdate, enddate,
                                                              begin, end);
                            int count = 0;
                            if (objs != null && objs.Count > 0)
                            {
                                count = objs.Count;
                            }
                            new JavaScriptSerializer().Serialize(objs, sb);

                            sb.Append(",\"Count\":" + count + "}");
                        }
                    }
                    break;

                case "operatelog":
                    {
                        var logProvider = new LogProvider();
                        int begin, end;
                        string domainname = context.Request.Params["domainname"];
                        string operatetype = context.Request.Params["operatetype"];
                        string username = context.Request.Params["queryusername"];

                        if (string.IsNullOrEmpty(domainname))
                        {
                            domainname = null;
                        }

                        if (string.IsNullOrEmpty(operatetype))
                        {
                            operatetype = null;
                        }

                        if (string.IsNullOrEmpty(username))
                        {
                            username = null;
                        }

                        DateTime startdate = Convert.ToDateTime("1900-1-1");
                        DateTime enddate = Convert.ToDateTime("2500-1-1");
                        if (context.Request.Params["startdate"] != null && context.Request.Params["enddate"] != null)
                        {
                            if (!string.IsNullOrEmpty(context.Request.Params["startdate"].Trim()))
                            {
                                startdate = Convert.ToDateTime(context.Request.Params["startdate"]);
                            }
                            if (!string.IsNullOrEmpty(context.Request.Params["enddate"].Trim()))
                            {
                                enddate = Convert.ToDateTime(context.Request.Params["enddate"]);
                            }
                        }

                        if (int.TryParse(context.Request.Params["begin"], out begin)
                            && int.TryParse(context.Request.Params["end"], out end))
                        {
                            sb.Append("{\"OperateLogList\":");

                            var objs = logProvider.GetOperationLog(domainname, operatetype, username, startdate, enddate,
                                                              begin, end);
                            int count = logProvider.GetOperationLogCout(domainname, operatetype, username, startdate, enddate);
                            new JavaScriptSerializer().Serialize(objs, sb);

                            sb.Append(",\"Count\":" + count + "}");
                        }
                    }
                    break;

                case "operateTypes":
                    {
                        var logProvider = new LogProvider();
                        var objs = logProvider.GetOperateType().Select(x => new { name = x.Value, id = x.Key });
                        new JavaScriptSerializer().Serialize(objs, sb);
                    }
                    break;

                case "getvws2user":
                    {
                        //edit by shanfeng.han
                        //var client = new PermissionWCFClient();
                        ActivateUserProvider Provider = new ActivateUserProvider();

                        var listResource = Provider.GetAllUsers().Select(
                                x => new { name = x.UserName });
                        //client.GetAllUsers(AppSettingProvider.Get("Dorado.Permission.AppCode"]).Select(
                        //    x => new { name = x.ToString() });
                        new JavaScriptSerializer().Serialize(listResource, sb);
                    }
                    break;
                //测试服务器的连接
                case "testConn":
                    {
                        string ip = context.Request.QueryString["ip"];
                        var testProvider = new TestConnectProvider();
                        bool success = testProvider.TestConnect(ip);
                        sb = new StringBuilder(string.Format("{{ \"ret\":{0} }}", success ? "true" : "false"));
                    }
                    break;
                // 测试demo服务器连接
                case "testConnByDomainId":
                    {
                        bool success = false;
                        int dominId;
                        string strDomainId = context.Request.QueryString["domainid"];
                        if (int.TryParse(strDomainId, out dominId))
                        {
                            var testProvider = new TestConnectProvider();
                            success = testProvider.TestConnectByDomainId(dominId);
                        }

                        sb = new StringBuilder(string.Format("{{ \"ret\":{0} }}", success ? "true" : "false"));
                    }
                    break;
                //启停服务
                case "serviceCtrl":
                    {
                        try
                        {
                            //string ip = context.Request.QueryString["ip"];
                            //string serviceName = context.Request.QueryString["servicename"];
                            string domainName = context.Request.QueryString["domainname"];
                            int serverID = 0;
                            int.TryParse(context.Request.QueryString["serverid"], out serverID);
                            bool isStart = Convert.ToBoolean(context.Request.QueryString["isStart"]);
                            bool isIis = Convert.ToBoolean(context.Request.QueryString["isIis"]);
                            DomainProvider _domainProvider = new DomainProvider();

                            ServerProvider _serverProvider = new ServerProvider();
                            var domain = _domainProvider.GetDomainByName(domainName);
                            var server = _serverProvider.GetServerById(serverID);
                            if (domain == null || server == null)
                            {
                                //sb = new StringBuilder(string.Format("{{ \"ret\":{0},\"message\":\"{1}\" }}","false","server or domain  error"));
                                sb = new StringBuilder((new JsonResult(false, "server or domain  error")).ToString());
                                break;
                            }

                            //定义操作日志
                            var operateLogEntity = new OperationLogEntity
                                                                      {
                                                                          UserName = CurUserName,
                                                                          DomainName = domainName,
                                                                          OperateType = EnumOperateType.ServerControl,
                                                                      };

                            var taskSender = new TaskSenderEntity
                            {
                                WinServiceName = domain.WinServiceName,
                                IISSiteName = domain.IISSiteName,
                                DomainType = (int)domain.DomainType,
                                OperatePathType = (int)domain.OperatePathType,
                                OperatePath = domain.OperatePath,
                            };
                            if (isIis)
                            {
                                taskSender.TaskCmd = isStart ? EnumTaskCmd.IISSTART : EnumTaskCmd.IISSTOP;
                                operateLogEntity.Log = string.Format("{0}，IIS站点：{1}，目标IP:{2}",
                                                                     taskSender.TaskCmd.GetDescription(),
                                                                     taskSender.IISSiteName, server.IP);
                            }
                            else
                            {
                                taskSender.TaskCmd = isStart ? EnumTaskCmd.WINSERVICESTART : EnumTaskCmd.WINSERVICESTOP;
                                operateLogEntity.Log = string.Format("{0}，Winservice服务：{1}，目标IP:{2}",
                                                                     taskSender.TaskCmd.GetDescription(),
                                                                     taskSender.WinServiceName, server.IP);
                            }
                            //if (!WebBasePage.HasDomainPermission(CurUserName, domain.DomainId, EnumManageType.ServiceControl))
                            //{
                            //    operateLogEntity.Result =false;
                            //    operateLogEntity.Log += "操作被拒绝，没有权限";
                            //    logProvider.AddOperateLog(operateLogEntity);
                            //}
                            var client = new SocketClient();
                            TaskResultEntity ret = client.SyncSend(IPAddress.Parse(server.IP), taskSender);

                            string iisState = "";
                            string svcState = "";

                            switch (ret.IISStatus)
                            {
                                case 1:
                                    iisState = "已启动";
                                    break;

                                case 2:
                                    iisState = "已停止";

                                    break;

                                case 3:
                                    iisState = "IIS站点状态未知";
                                    break;

                                case 4:
                                    iisState = "IIS站点没有配置";
                                    break;

                                default:
                                    iisState = "其他异常情况";
                                    break;
                            }

                            switch (ret.RelateSvcStatus)
                            {
                                case 0:
                                    svcState = "没有相关服务";
                                    break;

                                case 1:
                                    svcState = "已启动";
                                    break;

                                case 2:
                                    svcState = "已停止";
                                    break;

                                default:
                                    svcState = "其他异常情况";
                                    break;
                            }

                            //sb = new StringBuilder(string.Format("{{ \"ret\":{0},\"message\":\"{1}\",state:\"{2}\"}}", ret.Success ? "true" : "false","command send",isIis?iisState:svcState));
                            sb = new StringBuilder((new JsonResult(ret.Success, "command send", isIis ? iisState : svcState)).ToString());
                            //Thread.Sleep(5000);
                            operateLogEntity.Result = ret.Success;
                            //var logProvider = new LogProvider();
                            //logProvider.AddOperateLog(operateLogEntity);
                            Thread.Sleep(100);
                        }
                        catch (Exception ex)
                        {
                            LoggerWrapper.Logger.Error("VWS", "服务控制异常：" + ex.Message);
                        }
                    }
                    break;

                case "getserverstatus":
                    {
                        int serverID = 0;
                        int.TryParse(context.Request.QueryString["serverid"], out serverID);
                        bool isIis = Convert.ToBoolean(context.Request.QueryString["isIis"]);
                        TestConnectProvider _testConnect = new TestConnectProvider();
                        var result = _testConnect.GetServerStatus(serverID);

                        if (result == null)
                        {
                            //sb = new StringBuilder(string.Format("{{ \"ret\":{0},\"message\":\"{1}\" }}","false","server or domain  error"));
                            sb = new StringBuilder((new JsonResult(false, "no server")).ToString());
                            break;
                        }

                        string iisState = "";
                        string svcState = "";

                        switch (result.IISStatus)
                        {
                            case 1:
                                iisState = "已启动";
                                break;

                            case 2:
                                iisState = "已停止";

                                break;

                            case 3:
                                iisState = "IIS站点状态未知";
                                break;

                            case 4:
                                iisState = "IIS站点没有配置";
                                break;

                            default:
                                iisState = "其他异常情况";
                                break;
                        }

                        switch (result.RelateSvcStatus)
                        {
                            case 0:
                                svcState = "没有相关服务";
                                break;

                            case 1:
                                svcState = "已启动";
                                break;

                            case 2:
                                svcState = "已停止";
                                break;

                            default:
                                svcState = "其他异常情况";
                                break;
                        }
                        sb = new StringBuilder((new JsonResult<List<string>>(result.Success, "command send", serverID.ToString(), new List<string>() { iisState, svcState })).ToString());
                        //Thread.Sleep(5000);
                        break;
                    }
                //更新客户端服务
                case "updateClient":
                    {
                        string ip = context.Request.QueryString["ip"];
                        int domainID = Convert.ToInt32(context.Request.QueryString["domainID"]);

                        var updateProvider = new UpdateClientProvider();
                        if (string.IsNullOrEmpty(ip))
                        {
                            var serverProvider = new ServerProvider();
                            IList<ServerEntity> serverList = domainID.Equals("0") ? serverProvider.GetServersByIdc(0) : serverProvider.GetServersByDomainId(domainID);
                            updateProvider.UpdateClientService(serverList, CurUserName);
                        }
                        else
                        {
                            updateProvider.UpdateClientService(ip, CurUserName);
                        }
                    }
                    break;

                case "scheduletask":
                    {
                        var schProvider = new ScheduleTaskProvider();
                        //domainid, begin, end
                        int domainid, begin, end;
                        var count = 0;
                        if (int.TryParse(context.Request.Params["domainid"], out domainid)
                            && int.TryParse(context.Request.Params["begin"], out begin)
                            && int.TryParse(context.Request.Params["end"], out end))
                        {
                            sb.Append("{\"ScheduleTasks\":");
                            //切换到新的权限验证接口
                            //var pmsProvider = new PermissionProvider();
                            //if (pmsProvider.HasResoucePermission(CurUserName, domainid, "TaskRevertOwner"))
                            var pmsProvider = new DomainPermissionProvider();
                            if (pmsProvider.HasPermission(domainid, CurUserName, EnumManageType.RollBackALL))
                            {
                                var objs = schProvider.GetTasks(domainid, begin, end);
                                count = schProvider.GetTaskCount(domainid);
                                new JavaScriptSerializer().Serialize(objs, sb);
                            }
                            sb.Append(",\"Count\":" + count + "}");
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
    }

    #region json数据输出类 add by heyongdong

    /// <summary>
    /// json数据输出基类
    /// </summary>
    public abstract class BaseJsonResult
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool ret { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 详细状态
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 输出json
        /// </summary>
        /// <returns>json</returns>
        public override string ToString()
        {
            return (new JavaScriptSerializer()).Serialize(this);
            //return base.ToString();
        }
    }

    /// <summary>
    /// json数据输出类
    /// </summary>
    public class JsonResult : BaseJsonResult
    {
        public JsonResult(bool _ret, string _message, string _status)
        {
            ret = _ret;
            message = _message;
            status = _status;
        }

        public JsonResult(bool _ret, string _message)
        {
            ret = _ret;
            message = _message;
            status = null;
        }

        public JsonResult(bool _ret)
        {
            ret = _ret;
            message = null;
            status = null;
        }
    }

    /// <summary>
    /// 带泛型的json数据输出类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonResult<T> : BaseJsonResult
    {
        /// <summary>
        /// 泛型结果
        /// </summary>
        public T result { get; set; }

        public JsonResult(bool _ret, string _message, string _status, T _result)
        {
            ret = _ret;
            message = _message;
            status = _status;
            result = _result;
        }

        public JsonResult(bool _ret, string _message, T _result)
        {
            ret = _ret;
            message = _message;
            status = null;
            result = _result;
        }

        public JsonResult(bool _ret, T _result)
        {
            ret = _ret;
            message = null;
            status = null;
            result = _result;
        }
    }

    #endregion json数据输出类 add by heyongdong
}