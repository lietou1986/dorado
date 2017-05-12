using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Dorado.VWS.Services;

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    /// Summary description for GetAllDomains
    /// </summary>
    public class GetAllDomains : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string teamName = context.Request.Params["teamName"];
            var sb = new StringBuilder();
            var serverProvider = new ServerProvider();

            var domainlist =
                serverProvider.GetDomainsByIdcId(0).Select(x => new { name = x.DomainName, id = x.DomainId, environment = x.Environment });
            new JavaScriptSerializer().Serialize(domainlist, sb);
            if (string.IsNullOrEmpty(teamName))
            {
            }
            else
            {
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(sb);
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