using System.IO;
using System.Web;

namespace Dorado.Platform.Infrastructure
{
    /// <summary>
    /// A factory class that creates an HttpContext instance and initializes the HttpContext.Current property with that instance.
    /// This is useful when rendering views from a background thread, as some Html Helpers access HttpContext.Current directly, thus preventing a NullReferenceException.
    /// </summary>
    public class BackgroundHttpContextFactory : IBackgroundHttpContextFactory
    {
        public const string IsBackgroundHttpContextKey = "IsBackgroundHttpContext";

        public HttpContext CreateHttpContext(string url)
        {
            var httpContext = new HttpContext(new HttpRequest("", url, ""), new HttpResponse(new StringWriter()));

            httpContext.Items[IsBackgroundHttpContextKey] = true;

            return httpContext;
        }

        public void InitializeHttpContext(string url)
        {
            if (HttpContext.Current != null)
                return;

            HttpContext.Current = CreateHttpContext(url);
        }
    }
}