using System;
using System.Web;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Services;

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    /// Summary description for HeartBeat
    /// </summary>
    public class HeartBeat : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string serverIP = string.Empty;
            try
            {
                ServerProvider serverProvider = new ServerProvider();
                serverIP = context.Request.Params["ip"];
                string[] ips = serverIP.Split('|');
                foreach (string ip in ips)
                {
                    serverProvider.UpdateLastHeartBeatDate(ip, DateTime.Now);
                }
                context.Response.ContentType = "text/plain";
                context.Response.Write("Hello World");
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS.Site.HeartBeat", ex.ToString() + serverIP);
            }
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