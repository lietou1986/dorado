using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

using Vancl.IC.VWS.BLL;
using Vancl.IC.VWS.Model;
using Vancl.IC.VWS.SiteApp.PermissionWCF;


namespace Vancl.IC.VWS.SiteApp.Handler
{
    /// <summary>
    /// DomainListHandler 的摘要说明
    /// </summary>
    public class DomainListHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string curUserName = string.Empty;

            if (HttpContext.Current.Request.Cookies["SSOToken"] != null)
            {
                string ssoToken = HttpContext.Current.Request.Cookies["SSOToken"].Value;
                PermissionWCFClient client = new PermissionWCFClient();
                curUserName = client.GetUserId(ssoToken);

                ServerBLL _serverBLL = new ServerBLL();
                var list = _serverBLL.GetDomainsByUser(curUserName).Select(x => new { name = x.DomainName, id = x.DomainId });
                context.Response.Write(new JavaScriptSerializer().Serialize(list));
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