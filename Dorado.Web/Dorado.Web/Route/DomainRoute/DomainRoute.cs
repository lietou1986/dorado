using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dorado.Web.Route.DomainRoute
{
    public class DomainRoute : System.Web.Routing.Route
    {
        private Regex _domainRegex;
        private Regex _pathRegex;

        public DomainRoute(string domain, string url, RouteValueDictionary defaults)
            : base(url, defaults, new MvcRouteHandler())
        {
            Domain = domain;
        }

        public DomainRoute(string domain, string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
            Domain = domain;
        }

        public DomainRoute(string domain, string url, object defaults)
            : base(url, new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            Domain = domain;
        }

        public DomainRoute(string domain, string url, object defaults, IRouteHandler routeHandler)
            : base(url, new RouteValueDictionary(defaults), routeHandler)
        {
            Domain = domain;
        }

        public string Domain { get; set; }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // 构造 regex
            _domainRegex = CreateRegex(Domain);
            _pathRegex = CreateRegex(Url);

            // 请求信息
            var requestDomain = httpContext.Request.Headers["host"];
            if (!string.IsNullOrEmpty(requestDomain))
            {
                if (requestDomain.IndexOf(":") > 0)
                {
                    requestDomain = requestDomain.Substring(0, requestDomain.IndexOf(":"));
                }
            }
            else
            {
                requestDomain = httpContext.Request.Url.Host;
            }
            var requestPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) +
                              httpContext.Request.PathInfo;

            // 匹配域名和路由
            var domainMatch = _domainRegex.Match(requestDomain);
            var pathMatch = _pathRegex.Match(requestPath);

            // 路由数据
            RouteData data = null;
            if (domainMatch.Success && pathMatch.Success)
            {
                data = new RouteData(this, RouteHandler);

                // 添加默认选项
                if (Defaults != null)
                {
                    foreach (var item in Defaults)
                    {
                        data.Values[item.Key] = item.Value;
                    }
                }

                // 匹配域名路由
                for (var i = 1; i < domainMatch.Groups.Count; i++)
                {
                    var group = domainMatch.Groups[i];
                    if (group.Success)
                    {
                        var key = _domainRegex.GroupNameFromNumber(i);

                        if (!string.IsNullOrEmpty(key) && !char.IsNumber(key, 0))
                        {
                            if (!string.IsNullOrEmpty(group.Value))
                            {
                                data.Values[key] = group.Value;
                            }
                        }
                    }
                }

                // 匹配域名路径
                for (var i = 1; i < pathMatch.Groups.Count; i++)
                {
                    var group = pathMatch.Groups[i];
                    if (group.Success)
                    {
                        var key = _pathRegex.GroupNameFromNumber(i);

                        if (!string.IsNullOrEmpty(key) && !char.IsNumber(key, 0))
                        {
                            if (!string.IsNullOrEmpty(group.Value))
                            {
                                data.Values[key] = group.Value;
                            }
                        }
                    }
                }
            }

            return data;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return base.GetVirtualPath(requestContext, RemoveDomainTokens(values));
        }

        public DomainData GetDomainData(RequestContext requestContext,
            RouteValueDictionary values)
        {
            // 获得主机名
            var hostname = Domain;
            foreach (var pair in values)
            {
                hostname = hostname.Replace("{" + pair.Key + "}", pair.Value.ToString());
            }

            // Return 域名数据
            return new DomainData
            {
                Protocol = "http",
                HostName = hostname,
                Fragment = ""
            };
        }

        private Regex CreateRegex(string source)
        {
            // 替换
            source = source.Replace("/", @"\/?");
            source = source.Replace(".", @"\.?");
            source = source.Replace("-", @"\-?");
            source = source.Replace("{", @"(?<");
            source = source.Replace("}", @">([a-zA-Z0-9_]*))");

            return new Regex("^" + source + "$");
        }

        private RouteValueDictionary RemoveDomainTokens(RouteValueDictionary values)
        {
            var tokenRegex =
                new Regex(
                    @"({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?");
            var tokenMatch = tokenRegex.Match(Domain);
            for (var i = 0; i < tokenMatch.Groups.Count; i++)
            {
                var group = tokenMatch.Groups[i];
                if (group.Success)
                {
                    var key = group.Value.Replace("{", "").Replace("}", "");
                    if (values.ContainsKey(key))
                        values.Remove(key);
                }
            }

            return values;
        }
    }
}