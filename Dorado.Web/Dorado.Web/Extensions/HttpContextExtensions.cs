using Dorado.Extensions;
using Dorado.Web.Context.FastCookie;
using Dorado.Web.Fakes;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dorado.Web.Extensions
{
    /// <remarks>codehint: sm-add</remarks>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Indicates whether this context is fake
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Result</returns>
        public static bool IsFakeContext(this HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            return httpContext is FakeHttpContext;
        }

        public static Stream ToFileStream(this HttpRequestBase request, out string fileName, out string contentType, string paramName = "qqfile")
        {
            fileName = contentType = "";
            Stream stream = null;

            if (request[paramName].HasValue())
            {
                stream = request.InputStream;
                fileName = request[paramName];
            }
            else
            {
                if (request.Files.Count > 0)
                {
                    stream = request.Files[0].InputStream;
                    contentType = request.Files[0].ContentType;
                    fileName = Path.GetFileName(request.Files[0].FileName);
                }
            }

            if (contentType.IsNullOrEmpty())
            {
                contentType = MimeTypes.MapNameToMimeType(fileName);
            }

            return stream;
        }

        public static RouteData GetRouteData(this HttpContextBase httpContext)
        {
            Guard.ArgumentNotNull(() => httpContext);

            var handler = httpContext.Handler as MvcHandler;
            if (handler != null && handler.RequestContext != null)
            {
                return handler.RequestContext.RouteData;
            }

            return null;
        }

        public static bool TryGetRouteData(this HttpContextBase httpContext, out RouteData routeData)
        {
            routeData = httpContext.GetRouteData();
            return routeData != null;
        }

        public static T GetCookie<T>(this HttpContextBase httpContext) where T : FastCookie
        {
            var cookie = Activator.CreateInstance(typeof(T), httpContext) as T;
            return cookie;
        }

        public static T GetCookie<T>(this HttpContext httpContext) where T : FastCookie
        {
            HttpContextWrapper context = new HttpContextWrapper(httpContext);

            var cookie = Activator.CreateInstance(typeof(T), context) as T;
            return cookie;
        }
    }
}