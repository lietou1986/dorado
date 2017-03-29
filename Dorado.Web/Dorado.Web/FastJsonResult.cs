using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web
{
    public class FastJsonResult : JsonResult
    {
        public string CallbackFunction
        {
            get;
            set;
        }

        public FastJsonResult()
        {
            CallbackFunction = "callback";
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (base.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("donn't allow http method GET,Use JsonBehavior");
            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(base.ContentType))
                response.ContentType = base.ContentType;
            else
                response.ContentType = "application/json";
            if (base.ContentEncoding != null)
                response.ContentEncoding = base.ContentEncoding;
            if (base.Data != null)
            {
                string funcName = context.HttpContext.Request.QueryString[CallbackFunction];
                bool enableJsonp = !string.IsNullOrEmpty(funcName);
                if (enableJsonp)
                {
                    response.Write(funcName);
                    response.Write('(');
                }
                response.Write(JsonConvert.SerializeObject(base.Data));
                if (enableJsonp)
                    response.Write(')');
            }
        }
    }
}