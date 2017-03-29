using System;
using System.Globalization;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class CompressAttribute : ActionFilterAttribute
    {
        internal const string gzip = "gzip";
        internal const string deflate = "deflate";
        internal const string ContentEncoding = "Content-encoding";
        internal const string AcceptEncoding = "Accept-encoding";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            HttpRequestBase request = filterContext.HttpContext.Request;
            string acceptEncoding = request.Headers["Accept-encoding"];
            if (string.IsNullOrEmpty(acceptEncoding))
                return;
            acceptEncoding = acceptEncoding.ToLower(CultureInfo.CurrentCulture);
            HttpResponseBase response = filterContext.HttpContext.Response;
            if (acceptEncoding.Contains("gzip"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                return;
            }
            if (acceptEncoding.Contains("deflate"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}