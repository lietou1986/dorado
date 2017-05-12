using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vancl.IC.VWS.BLL;
using System.Text;
using System.Web.Script.Serialization;
using Vancl.IC.VWS.Model;

namespace Vancl.IC.VWS.SiteApp.Handler
{
    /// <summary>
    /// VersionFilesHandler 的摘要说明
    /// </summary>
    public class VersionFilesHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();
            SyncBLL syncBll = new SyncBLL();
            ServerBLL serBll = new ServerBLL();
            //domainid, filepath, begin, end
            string filepath = context.Request.Params["filepath"];
            int domainid, begin, end;
            int count = 0;
            if (!string.IsNullOrEmpty(filepath) && int.TryParse(context.Request.Params["domainid"], out domainid)
                && int.TryParse(context.Request.Params["begin"], out begin)
                && int.TryParse(context.Request.Params["end"], out end))
            {
                ServerEntity source = serBll.GetSourceServerByDomainId(domainid);

                filepath = filepath.Substring(source.Root.Length);

                var objs = syncBll.GetVersionFiles(domainid, filepath, begin, end);
                count = syncBll.GetVersionCount(domainid, filepath);
                new JavaScriptSerializer().Serialize(objs, sb);
            }
            context.Response.ClearContent();
            context.Response.ContentType = "text/plain";
            context.Response.Write("{\"Version\":" + sb.ToString() + ",\"Count\":" + count + "}");
            context.Response.End();
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