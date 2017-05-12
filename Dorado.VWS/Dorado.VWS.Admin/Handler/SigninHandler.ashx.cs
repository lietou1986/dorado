using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vancl.IC.VWS.Model;
using Vancl.IC.VWS.BLL;
using Vancl.IC.VWS.Model.Enum;
using Memcached.ClientLibrary;
using Vancl.IC.VWS.Model.IC;

namespace Vancl.IC.VWS.SiteApp.Handler
{
    /// <summary>
    /// SigninHandler 的摘要说明
    /// </summary>
    public class SigninHandler : IHttpHandler
    {
        private ServerBLL _serBll = new ServerBLL();
        public void ProcessRequest(HttpContext context)
        {
             
            //ip, version, iisstatus, hostname
            string ip = context.Request.Params["ip"];
            string version = context.Request.Params["version"];
            string hostName = context.Request.Params["hostname"];
            int iisStatus = 0;

            int interval = 60;

            if (int.TryParse(context.Request.Params["iisstatus"], out iisStatus) &&
                !string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(hostName))
            {
                ServerInfo si = new ServerInfo()
                {
                    IP = ip,
                    ClientVersion = version,
                    HostName = hostName
                };
                MemcachedClient mc = new MemcachedClient();

                mc.Set("vws_serverinfo_" + ip, si, DateTime.Now.AddSeconds(interval + 5));
            }
            
            
            context.Response.ContentType = "text/plain";
            context.Response.Write(interval);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}