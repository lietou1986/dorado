using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web
{
    public class JsonpViewResult : ViewResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            HttpRequestBase request = context.HttpContext.Request;
            HttpResponseBase response = context.HttpContext.Response;
            string callbackFunc = request.QueryString["callback"];
            if (!string.IsNullOrEmpty(callbackFunc))
            {
                if (string.IsNullOrEmpty(base.ViewName))
                    base.ViewName = context.RouteData.GetRequiredString("action");
                ViewEngineResult result = null;
                if (base.View == null)
                {
                    result = FindView(context);
                    base.View = result.View;
                }
                StringBuilder sb = new StringBuilder(1024);
                using (TextWriter writer = new StringWriter(sb))
                {
                    ViewContext viewContext = new ViewContext(context, base.View, base.ViewData, base.TempData, writer);
                    base.View.Render(viewContext, writer);
                    writer.Flush();
                }
                string content = JsonConvert.SerializeObject(sb.ToString());
                response.ContentType = "application/json";
                response.Write(callbackFunc);
                response.Write('(');
                response.Write(content);
                response.Write(')');
                if (result != null)
                {
                    result.ViewEngine.ReleaseView(context, base.View);
                    return;
                }
            }
            else
                base.ExecuteResult(context);
        }
    }
}