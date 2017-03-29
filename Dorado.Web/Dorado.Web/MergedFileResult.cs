using Dorado.Web.Fileset;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dorado.Web
{
    public class MergedFileResult : ActionResult
    {
        public const string HttpIfNoneMatch = "HTTP_IF_NONE_MATCH";
        public const string Etag = "ETag";
        public const string NotModified = "304 Not Modified";
        public const string ContentLength = "Content-Length";

        public string FileName
        {
            get;
            set;
        }

        public MergedFileResult(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName", "参数fileName不能为空");
            FileName = fileName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            MergedStaticFile file = StaticFilesetManager.GetStaticFile(FileName) as MergedStaticFile;
            if (file != null)
            {
                HttpResponseBase response = context.HttpContext.Response;
                response.ContentEncoding = Encoding.UTF8;
                StaticFileType type = file.Type;
                switch (type)
                {
                    case StaticFileType.Css:
                        {
                            response.ContentType = "text/stylesheet";
                            break;
                        }
                    case StaticFileType.Javascript:
                        {
                            response.ContentType = "text/javacript";
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                response.ContentType = "text/javascript";
                string content = file.Cache;
                if (!IsCachedOnBrowser(context.RequestContext, file.ETag))
                {
                    response.AppendHeader("ETag", file.ETag);
                    response.Write(content);
                }
            }
        }

        protected bool IsCachedOnBrowser(RequestContext requestContext, string etag)
        {
            if (!string.IsNullOrEmpty(requestContext.HttpContext.Request.ServerVariables["HTTP_IF_NONE_MATCH"]) && requestContext.HttpContext.Request.ServerVariables["HTTP_IF_NONE_MATCH"].Equals(etag))
            {
                requestContext.HttpContext.Response.ClearHeaders();
                requestContext.HttpContext.Response.AppendHeader("ETag", etag);
                requestContext.HttpContext.Response.Status = "304 Not Modified";
                requestContext.HttpContext.Response.AppendHeader("Content-Length", "0");
                return true;
            }
            return false;
        }
    }
}