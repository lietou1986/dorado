using Dorado.Core;
using Dorado.Core.Cache;
using Dorado.Core.Logger;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Dorado.Web.Extensions
{
    public static class HtmlHelperExtension
    {
        private const string ClickHeatTemplate = "<script type=\"text/javascript\" src=\"http://{0}/js/clickheat.js\"></script>\r\n<noscript><p><a href=\"http://www.labsmedia.com/clickheat/index.html\">Landing page optimization</a></p></noscript>\r\n<script type=\"text/javascript\"><!--\r\nclickHeatSite = '';\r\nclickHeatGroup = '{1}';\r\nclickHeatServer = 'http://{0}/click.php';\r\ninitClickHeat(); //-->\r\n</script>\r\n";
        private static readonly string IncRoot = HttpContext.Current.Server.MapPath("~/inc");
        private static readonly Cache<string, string> CacheManager = new Cache<string, string>();

        public static MvcHtmlString Cache(this HtmlHelper htmlHelper, string cacheKey, TimeSpan slidingExpiration,
            Func<string> func)
        {
            try
            {
                var content = CacheManager.Get(cacheKey);

                if (content == null)
                {
                    content = func();
                    CacheManager.AddOfRelative(cacheKey, content, slidingExpiration);
                }
                return new MvcHtmlString(content);
            }
            catch (Exception)
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        public static MvcHtmlString Include(this HtmlHelper htmlHelper, string virtualPath)
        {
            try
            {
                var incFile = IncRoot + virtualPath;
                var content = File.ReadAllText(incFile, Encoding.UTF8);
                return new MvcHtmlString(content);
            }
            catch (Exception)
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        public static MvcHtmlString Include(this HtmlHelper htmlHelper, string virtualPath, string cacheKey,
            TimeSpan slidingExpiration)
        {
            try
            {
                var content = CacheManager.Get(cacheKey);

                if (content == null)
                {
                    var incFile = IncRoot + virtualPath;
                    content = File.ReadAllText(incFile, Encoding.UTF8);
                    CacheManager.AddOfRelative(cacheKey, content, slidingExpiration);
                }
                return new MvcHtmlString(content);
            }
            catch (Exception)
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        public static MvcHtmlString IncludeWithFileDepends(this HtmlHelper htmlHelper, string virtualPath,
            string cacheKey, TimeSpan slidingExpiration)
        {
            try
            {
                var cache = htmlHelper.ViewContext.HttpContext.Cache;
                var content = cache.Get(cacheKey) as string;

                if (content == null)
                {
                    var incFile = IncRoot + virtualPath;
                    content = File.ReadAllText(incFile, Encoding.UTF8);
                    CacheDependency fileDepends = new CacheDependency(incFile);
                    cache.Insert(cacheKey, content, fileDepends, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        slidingExpiration);
                }

                return new MvcHtmlString(content);
            }
            catch (Exception)
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        public static void EnableClickHeat(this HtmlHelper html)
        {
            try
            {
                string env = ConfigurationManager.AppSettings["Environment"];
                string srv = ConfigurationManager.AppSettings["ClickHeatServer"];
                string filter = ConfigurationManager.AppSettings["HostFilter"];
                string app = ConfigurationManager.AppSettings["ApplicationName"];
                env = (env ?? "testing");
                srv = (srv ?? "clickheat.Doradocorp.com");
                filter = (filter ?? "(^10\\.129\\..*$)|(^.*Dorado.*)");
                app = (app ?? HostingEnvironment.SiteName);
                if (!env.Equals("testing", StringComparison.OrdinalIgnoreCase) || !Regex.IsMatch(html.ViewContext.HttpContext.Request.UserHostAddress, filter, RegexOptions.IgnoreCase | RegexOptions.Singleline) || !Regex.IsMatch(html.ViewContext.HttpContext.Request.UserHostName, filter, RegexOptions.IgnoreCase | RegexOptions.Singleline))
                {
                    string identity = (app + html.ViewContext.HttpContext.Request.Url.AbsolutePath).Replace("/", "._.");
                    html.ViewContext.HttpContext.Response.Output.WriteClickHeat(srv, identity);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("启用热点跟踪异常", ex);
            }
        }

        internal static void WriteClickHeat(this TextWriter writer, string server, string identity)
        {
            writer.Write(ClickHeatTemplate, server, identity);
        }

        public static string Cache(this HtmlHelper htmlHelper, string cacheKey, TimeSpan slidingExpiration, Func<object> func)
        {
            Cache<string, string> memoryCache = new Cache<string, String>();
            var content = memoryCache.Get(cacheKey);

            if (content == null)
            {
                content = func().ToString();
                memoryCache.AddOfRelative(cacheKey, content, slidingExpiration);
            }

            return content;
        }

        public static string Cache(this HtmlHelper htmlHelper, string cacheKey, DateTime absoluteExpiration, Func<object> func)
        {
            Cache<string, string> memoryCache = new Cache<string, String>();
            var content = memoryCache.Get(cacheKey);

            if (content == null)
            {
                content = func().ToString();
                memoryCache.AddOfTermly(cacheKey, content, absoluteExpiration);
            }

            return content;
        }
    }
}