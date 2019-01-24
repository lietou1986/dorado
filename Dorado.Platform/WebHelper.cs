using Dorado.Core;
using Dorado.Core.Collection;
using Dorado.Core.Logger;
using Dorado.Extensions;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace Dorado.Platform
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class WebHelper
    {
        private static object s_lock = new object();
        private static bool? s_optimizedCompilationsEnabled;
        private static AspNetHostingPermissionLevel? s_trustLevel;
        private static readonly Regex s_staticExts = new Regex(@"(.*?)\.(css|js|png|jpg|jpeg|gif|bmp|html|htm|xml|pdf|doc|xls|rar|zip|ico|eot|svg|ttf|woff|otf|axd|ashx|less)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex s_htmlPathPattern = new Regex(@"(?<=(?:href|src)=(?:""|'))(?!https?://)(?<url>[^(?:""|')]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex s_cssPathPattern = new Regex(@"url\('(?<url>.+)'\)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        private static ConcurrentDictionary<int, string> s_safeLocalHostNames = new ConcurrentDictionary<int, string>();

        private readonly HttpContextBase _httpContext;
        private bool? _isCurrentConnectionSecured;

        public WebHelper(HttpContextBase httpContext)
        {
            this._httpContext = httpContext;
        }

        public virtual string GetUrlReferrer()
        {
            string referrerUrl = string.Empty;

            if (_httpContext != null &&
                _httpContext.Request != null &&
                _httpContext.Request.UrlReferrer != null)
                referrerUrl = _httpContext.Request.UrlReferrer.ToString();

            return referrerUrl;
        }

        public virtual string GetCurrentIpAddress()
        {
            string result = null;

            if (_httpContext != null && _httpContext.Request != null)
                result = _httpContext.Request.UserHostAddress;

            if (result == "::1")
                result = "127.0.0.1";

            return result.EmptyNull();
        }

        public virtual bool IsCurrentConnectionSecured()
        {
            if (!_isCurrentConnectionSecured.HasValue)
            {
                _isCurrentConnectionSecured = false;
                if (_httpContext != null && _httpContext.Request != null)
                {
                    _isCurrentConnectionSecured = _httpContext.Request.IsSecureConnection();
                }
            }

            return _isCurrentConnectionSecured.Value;
        }

        public virtual string ServerVariables(string name)
        {
            string result = string.Empty;

            try
            {
                if (_httpContext != null && _httpContext.Request != null)
                {
                    if (_httpContext.Request.ServerVariables[name] != null)
                    {
                        result = _httpContext.Request.ServerVariables[name];
                    }
                }
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        public virtual bool IsStaticResource(HttpRequest request)
        {
            return IsStaticResourceRequested(new HttpRequestWrapper(request));
        }

        public static bool IsStaticResourceRequested(HttpRequest request)
        {
            Guard.ArgumentNotNull(() => request);
            return s_staticExts.IsMatch(request.Path);
        }

        public static bool IsStaticResourceRequested(HttpRequestBase request)
        {
            // unit testable
            Guard.ArgumentNotNull(() => request);
            return s_staticExts.IsMatch(request.Path);
        }

        public virtual string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                //not hosted. For example, run in unit tests
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                return Path.Combine(baseDirectory, path);
            }
        }

        public virtual string ModifyQueryString(string url, string queryStringModification, string anchor)
        {
            url = url.EmptyNull();
            queryStringModification = queryStringModification.EmptyNull();

            string curAnchor = null;

            var hsIndex = url.LastIndexOf('#');
            if (hsIndex >= 0)
            {
                curAnchor = url.Substring(hsIndex);
                url = url.Substring(0, hsIndex);
            }

            var parts = url.Split(new[] { '?' });
            var current = new QueryString(parts.Length == 2 ? parts[1] : "");
            var modify = new QueryString(queryStringModification);

            foreach (var nv in modify.AllKeys)
            {
                current.Add(nv, modify[nv], true);
            }

            var result = string.Concat(
                parts[0],
                current.ToString(),
                anchor.NullEmpty() == null ? (curAnchor == null ? "" : "#" + curAnchor) : "#" + anchor
            );

            return result;
        }

        public virtual string RemoveQueryString(string url, string queryString)
        {
            var parts = url.SplitSafe("?");

            var current = new QueryString(parts.Length == 2 ? parts[1] : "");

            if (current.Count > 0 && queryString.HasValue())
            {
                current.Remove(queryString);
            }

            var result = string.Concat(parts[0], current.ToString());
            return result;
        }

        public virtual T QueryString<T>(string name)
        {
            string queryParam = null;

            if (_httpContext != null && _httpContext.Request.QueryString[name] != null)
                queryParam = _httpContext.Request.QueryString[name];

            if (!String.IsNullOrEmpty(queryParam))
                return queryParam.Convert<T>();

            return default(T);
        }

        internal static bool OptimizedCompilationsEnabled
        {
            get
            {
                if (!s_optimizedCompilationsEnabled.HasValue)
                {
                    var section = (CompilationSection)ConfigurationManager.GetSection("system.web/compilation");
                    s_optimizedCompilationsEnabled = section.OptimizeCompilations;
                }

                return s_optimizedCompilationsEnabled.Value;
            }
        }

        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!s_trustLevel.HasValue)
            {
                //set minimum
                s_trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximum
                foreach (AspNetHostingPermissionLevel trustLevel in
                        new[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal
                            })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        s_trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                }
            }
            return s_trustLevel.Value;
        }

        /// <summary>
        /// Prepends protocol and host to all (relative) urls in a html string
        /// </summary>
        /// <param name="html">The html string</param>
        /// <param name="request">Request object</param>
        /// <returns>The transformed result html</returns>
        /// <remarks>
        /// All html attributed named <c>src</c> and <c>href</c> are affected, also occurences of <c>url('path')</c> within embedded stylesheets.
        /// </remarks>
        public static string MakeAllUrlsAbsolute(string html, HttpRequestBase request)
        {
            Guard.ArgumentNotNull(() => request);

            if (request.Url == null)
            {
                return html;
            }

            return MakeAllUrlsAbsolute(html, request.Url.Scheme, request.Url.Authority);
        }

        /// <summary>
        /// Prepends protocol and host to all (relative) urls in a html string
        /// </summary>
        /// <param name="html">The html string</param>
        /// <param name="protocol">The protocol to prepend, e.g. <c>http</c></param>
        /// <param name="host">The host name to prepend, e.g. <c>www.mysite.com</c></param>
        /// <returns>The transformed result html</returns>
        /// <remarks>
        /// All html attributed named <c>src</c> and <c>href</c> are affected, also occurences of <c>url('path')</c> within embedded stylesheets.
        /// </remarks>
        public static string MakeAllUrlsAbsolute(string html, string protocol, string host)
        {
            Guard.ArgumentNotEmpty(() => html);
            Guard.ArgumentNotEmpty(() => protocol);
            Guard.ArgumentNotEmpty(() => host);

            string baseUrl = string.Format("{0}://{1}", protocol, host.TrimEnd('/'));

            MatchEvaluator evaluator = (match) =>
            {
                var url = match.Groups["url"].Value;
                return "{0}{1}".FormatCurrent(baseUrl, url.EnsureStartsWith("/"));
            };

            html = s_htmlPathPattern.Replace(html, evaluator);
            html = s_cssPathPattern.Replace(html, evaluator);

            return html;
        }

        /// <summary>
        /// Prepends protocol and host to the given (relative) url
        /// </summary>
        [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
        public static string GetAbsoluteUrl(string url, HttpRequestBase request)
        {
            Guard.ArgumentNotEmpty(() => url);
            Guard.ArgumentNotNull(() => request);

            if (request.Url == null)
            {
                return url;
            }

            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            if (url.StartsWith("~"))
            {
                url = VirtualPathUtility.ToAbsolute(url);
            }

            url = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, url);
            return url;
        }

        public static string GetPublicIPAddress()
        {
            string result = string.Empty;

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers["User-Agent"] = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                    try
                    {
                        byte[] arr = client.DownloadData("http://checkip.amazonaws.com/");
                        string response = Encoding.UTF8.GetString(arr);
                        result = response.Trim();
                    }
                    catch { }
                }
            }
            catch { }

            var checkers = new string[]
            {
                "https://ipinfo.io/ip",
                "https://api.ipify.org",
                "https://icanhazip.com",
                "https://wtfismyip.com/text",
                "http://bot.whatismyipaddress.com/"
            };

            if (string.IsNullOrEmpty(result))
            {
                using (var client = new WebClient())
                {
                    foreach (var checker in checkers)
                    {
                        try
                        {
                            result = client.DownloadString(checker).Replace("\n", "");
                            if (!string.IsNullOrEmpty(result))
                            {
                                break;
                            }
                        }
                        catch { }
                    }
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                try
                {
                    var url = "http://checkip.dyndns.org";
                    var req = WebRequest.Create(url);
                    using (var resp = req.GetResponse())
                    {
                        using (var sr = new StreamReader(resp.GetResponseStream()))
                        {
                            var response = sr.ReadToEnd().Trim();
                            var a = response.Split(':');
                            var a2 = a[1].Substring(1);
                            var a3 = a2.Split('<');
                            result = a3[0];
                        }
                    }
                }
                catch { }
            }

            return result;
        }

        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.</returns>
        /// <remarks>
        /// These are the file extensions considered to be static resources:
        /// .css
        ///	.gif
        /// .png
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        public virtual bool IsStaticResource()
        {
            var request = _httpContext.Request;
            string path = request.Path;
            string extension = VirtualPathUtility.GetExtension(path);

            if (extension == null) return false;

            switch (extension.ToLower())
            {
                case ".axd":
                case ".ashx":
                case ".bmp":
                case ".css":
                case ".gif":
                case ".htm":
                case ".html":
                case ".ico":
                case ".jpeg":
                case ".jpg":
                case ".js":
                case ".png":
                case ".rar":
                case ".zip":
                case ".woff":
                case ".eot":
                case ".svg":
                case ".otf":
                case ".ttf":
                case ".less":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the request is made by search engine (web crawler)
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Result</returns>
        public virtual bool IsSearchEngine(HttpContextBase context)
        {
            if (context == null)
                return false;

            bool result = false;
            try
            {
                if (context.Request.GetType().ToString().Contains("Fake"))
                    return false;

                result = context.Request.Browser.Crawler;
                if (!result && context.Request.UserAgent != null)
                {
                    //put any additional known crawlers in the Regex below for some custom validation
                    var regEx = new Regex("Twiceler|twiceler|BaiDuSpider|baduspider|Slurp|slurp|ask|Ask|Teoma|teoma|Yahoo|yahoo");
                    result = regEx.Match(context.Request.UserAgent).Success;
                }
            }
            catch
            {
                // ignored
            }
            return result;
        }

        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// </summary>
        public virtual bool IsRequestBeingRedirected
        {
            get
            {
                var response = _httpContext.Response;
                return response.IsRequestBeingRedirected;
            }
        }

        /// <summary>
        ///重启应用
        /// </summary>
        /// <param name="redirectUrl"></param>
        public virtual void RestartAppDomain(string redirectUrl = "")
        {
            LoggerWrapper.Logger.Info("重启应用", "RestartAppDomain Begin");

            HttpRuntime.UnloadAppDomain();
            // without this, MVC may fail resolving controllers for newly installed plugins after IIS restart
            Thread.Sleep(250);

            LoggerWrapper.Logger.Info("重启应用", "RestartAppDomain End");

            if (_httpContext == null) return;

            if (_httpContext.Request.RequestType == "GET")
            {
                if (!string.IsNullOrEmpty(redirectUrl) || _httpContext.Request.Url == null) return;

                redirectUrl = _httpContext.Request.Url.GetLeftPart(UriPartial.Path);
                _httpContext.Response.Redirect(redirectUrl, true /*endResponse*/);
            }
            else
            {
                _httpContext.Response.Write("The site is temporarily unavailable as a change in configuration requires a restart.");
                _httpContext.Response.End();
            }
        }
    }
}