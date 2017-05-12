#region using

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Dorado.VWS.Model;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    ///     获取同步宿IP的Handler
    /// </summary>
    public class GetSourceIps : IHttpHandler
    {
        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            string domain = context.Request.Params["domain"];
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(domain))
            {
                var serProvider = new ServerProvider();

                IList<ServerEntity> targets = serProvider.GetTargetServerByDomain(domain);
                if (targets != null && targets.Count > 0)
                {
                    sb.Append("[");
                    bool bFirst = true;
                    foreach (string ip in targets.Select(s => s.IP))
                    {
                        if (!bFirst)
                        {
                            sb.Append(',');
                        }

                        sb.Append(string.Format("\"{0}\"", ip));
                        bFirst = false;
                    }
                    sb.Append(']');
                }
            }

            context.Response.ClearContent();
            context.Response.ContentType = "text/plain";
            context.Response.Write("{\"ips\":" + sb + "}");
            context.Response.End();
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion IHttpHandler Members
    }
}