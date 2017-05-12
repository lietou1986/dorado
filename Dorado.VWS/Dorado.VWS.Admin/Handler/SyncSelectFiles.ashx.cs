using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Net;
using System.Threading;
using Vancl.IC.VWS.BLL;
using Vancl.IC.VWS.Model;
using Vancl.IC.VWS.SiteApp.PermissionWCF;
using Vancl.IC.VWS.Common;
using Vancl.IC.VWS.Common.Sockets;

namespace Vancl.IC.VWS.SiteApp.Handler
{
    /// <summary>
    /// SyncSelectFiles 的摘要说明
    /// </summary>
    public class SyncSelectFiles : IHttpHandler
    {
        protected string curUsername
        {
            get
            {
                if (HttpContext.Current.Request.Cookies["SSOToken"] == null) return "";
                string ssoToken = HttpContext.Current.Request.Cookies["SSOToken"].Value;
                PermissionWCFClient client = new PermissionWCFClient();
                return client.GetUserId(ssoToken);
            }
        }

        private AutoResetEvent are = null;
        private bool downloadSucess = false;
        
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            switch (context.Request.QueryString["action"])
            {
                case "getdomains":
                    {
                        ServerBLL _serverBLL = new ServerBLL();
                        var list = _serverBLL.GetDomainsByUser(curUsername).Select(x => new { name = x.DomainName, id = x.DomainId });
                        context.Response.Write(new JavaScriptSerializer().Serialize(list));
                    }
                    break;
                case "submit":
                    {
                        string data = context.Server.UrlDecode(context.Request.QueryString["data"]);
                        List<VersionFileEntity> fileList = new JavaScriptSerializer().Deserialize<List<VersionFileEntity>>(data);
                        SyncTaskProcessor task = new SyncTaskProcessor();
                        List<int> domainID = fileList.GroupBy(x => x.DomainId).Select(x => x.Key).ToList();
                        List<int> taskID = new List<int>();
                        foreach (int id in domainID)
                        {
                            List<int> fileID = fileList.Where(x => x.DomainId == id).Select(x => x.VersionFileId).ToList();
                            taskID.Add(task.RevertFiles(id, fileID, curUsername));
                        }
                        context.Response.Write(string.Format("{{\"taskID\": \"{0}\"}}", string.Join(",", taskID.ToArray())));
                    }
                    break;
                case "download":
                    {
                        string filePath = context.Request.QueryString["filePath"];
                        int domainID = Convert.ToInt32(context.Request.QueryString["domainID"]);
                        bool isHistory = context.Request.QueryString["isHistory"] == "true";
                        string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

                        ServerEntity server = new ServerBLL().GetSourceServerByDomainId(domainID);
                        TaskSenderEntity task = new TaskSenderEntity()
                        {
                            TaskName = "getfilebytes",
                            TargetRoot = server.Root.TrimEnd('\\') + (isHistory ? ".vws\\" : "\\") + filePath
                        };

                        SocketFileClient client = new SocketFileClient();
                        client.ReceivedFile += ReceivedFile;
                        client.FileFolder = context.Server.MapPath("~/tmp/");
                        client.Send(IPAddress.Parse(server.IP), task);
                        are = new AutoResetEvent(false);
                        are.WaitOne();
                        context.Response.Write(string.Format("{{\"ret\": {0}}}", downloadSucess ? "true" : "false"));
                    }
                    break;
                default:
                    break;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private void ReceivedFile(string filePath, string md5, bool success, string errorMsg)
        {
            downloadSucess = success;
            are.Set();
        }
    }
}