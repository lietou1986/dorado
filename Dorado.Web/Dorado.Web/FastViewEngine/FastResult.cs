using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine
{
    public class FastResult : ActionResult
    {
        public FastResult(string data)
            : this(data, Encoding.UTF8)
        {
            Data = data;
        }

        public FastResult(string data, Encoding contentEncoding)
        {
            Data = data;
            ContentEncoding = contentEncoding;
        }

        public string Data
        {
            get;
            set;
        }

        public Encoding ContentEncoding
        {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = "text/xml";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                string action = context.RouteData.Route.GetRouteData(context.HttpContext).Values["action"].ToString();
                string responseTmp = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body><{0}Response xmlns=\"http://Dorado/\"><{0}Result><![CDATA[{1}]]></{0}Result></{0}Response></soap:Body></soap:Envelope>";
                response.Write(string.Format(responseTmp, action.ToLower(), Data));
            }
        }
    }
}