using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Dorado.Web.Route.RouteDebug
{
    public class DebugHttpHandler : IHttpHandler
    {
        public RequestContext RequestContext
        {
            get;
            set;
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            string generatedUrlInfo = string.Empty;
            if (context.Request.QueryString.Count > 0)
            {
                RouteValueDictionary rvalues = new RouteValueDictionary();
                foreach (string key in context.Request.QueryString.Keys)
                {
                    rvalues.Add(key, context.Request.QueryString[key]);
                }
                VirtualPathData vpd = RouteTable.Routes.GetVirtualPath(RequestContext, rvalues);
                if (vpd != null)
                {
                    generatedUrlInfo = "<p><label>Generated URL</label>: ";
                    generatedUrlInfo = generatedUrlInfo + "<strong style=\"color: #00a;\">" + vpd.VirtualPath + "</strong>";
                    System.Web.Routing.Route vpdRoute = vpd.Route as System.Web.Routing.Route;
                    if (vpdRoute != null)
                    {
                        generatedUrlInfo = generatedUrlInfo + " using the route \"" + vpdRoute.Url + "\"</p>";
                    }
                }
            }
            const string htmlFormat = "<html>\r\n<head>\r\n    <title>Route Tester</title>\r\n    <style>\r\n        body, td, th {{font-family: verdana; font-size: small;}}\r\n        .message {{font-size: .9em;}}\r\n        caption {{font-weight: bold;}}\r\n        tr.header {{background-color: #ffc;}}\r\n        label {{font-weight: bold; font-size: 1.1em;}}\r\n        .false {{color: #c00;}}\r\n        .true {{color: #0c0;}}\r\n    </style>\r\n</head>\r\n<body>\r\n<h1>Route Tester</h1>\r\n<div id=\"main\">\r\n    <p class=\"message\">\r\n        Type in a url in the address bar to see which defined routes match it. \r\n        A {{*catchall}} route is added to the list of routes automatically in \r\n        case none of your routes match.\r\n    </p>\r\n    <p class=\"message\">\r\n        To generate URLs using routing, supply route values via the query string. example: <code>http://localhost:14230/?id=123</code>\r\n    </p>\r\n    <p><label>Matched Route</label>: {1}</p>\r\n    {5}\r\n    <div style=\"float: left;\">\r\n        <table border=\"1\" cellpadding=\"3\" cellspacing=\"0\" width=\"300\">\r\n            <caption>Route Data</caption>\r\n            <tr class=\"header\"><th>Key</th><th>Value</th></tr>\r\n            {0}\r\n        </table>\r\n    </div>\r\n    <div style=\"float: left; margin-left: 10px;\">\r\n        <table border=\"1\" cellpadding=\"3\" cellspacing=\"0\" width=\"300\">\r\n            <caption>Data Tokens</caption>\r\n            <tr class=\"header\"><th>Key</th><th>Value</th></tr>\r\n            {4}\r\n        </table>\r\n    </div>\r\n    <hr style=\"clear: both;\" />\r\n    <table border=\"1\" cellpadding=\"3\" cellspacing=\"0\">\r\n        <caption>All Routes</caption>\r\n        <tr class=\"header\">\r\n            <th>Matches Current Request</th>\r\n            <th>Url</th>\r\n            <th>Defaults</th>\r\n            <th>Constraints</th>\r\n            <th>DataTokens</th>\r\n        </tr>\r\n        {2}\r\n    </table>\r\n    <hr />\r\n    <h3>Current Request Info</h3>\r\n    <p>\r\n        AppRelativeCurrentExecutionFilePath is the portion of the request that Routing acts on.\r\n    </p>\r\n    <p><strong>AppRelativeCurrentExecutionFilePath</strong>: {3}</p>\r\n</div>\r\n</body>\r\n</html>";
            string routeDataRows = string.Empty;
            RouteData routeData = RequestContext.RouteData;
            RouteValueDictionary routeValues = routeData.Values;
            RouteBase matchedRouteBase = routeData.Route;
            string routes = string.Empty;
            using (IDisposable readLock = RouteTable.Routes.GetReadLock())
            {
                foreach (RouteBase routeBase in RouteTable.Routes)
                {
                    bool matchesCurrentRequest = routeBase.GetRouteData(RequestContext.HttpContext) != null;
                    string matchText = string.Format("<span class=\"{0}\">{0}</span>", matchesCurrentRequest);
                    string url = "n/a";
                    string defaults = "n/a";
                    string constraints = "n/a";
                    string dataTokens = "n/a";
                    System.Web.Routing.Route route = routeBase as System.Web.Routing.Route;
                    if (route != null)
                    {
                        url = route.Url;
                        defaults = FormatRouteValueDictionary(route.Defaults);
                        constraints = FormatRouteValueDictionary(route.Constraints);
                        dataTokens = FormatRouteValueDictionary(route.DataTokens);
                    }
                    routes += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", new object[]
					{
						matchText,
						url,
						defaults,
						constraints,
						dataTokens
					});
                }
            }
            string matchedRouteUrl = "n/a";
            string dataTokensRows = string.Empty;
            if (!(matchedRouteBase is DebugRoute))
            {
                routeDataRows = routeValues.Keys.Aggregate(routeDataRows, (current, key2) => current + string.Format("\t<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key2, routeValues[key2]));
                dataTokensRows = routeData.DataTokens.Keys.Aggregate(dataTokensRows, (current, key3) => current + string.Format("\t<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key3, routeData.DataTokens[key3]));
                System.Web.Routing.Route matchedRoute = matchedRouteBase as System.Web.Routing.Route;
                if (matchedRoute != null)
                {
                    matchedRouteUrl = matchedRoute.Url;
                }
            }
            else
            {
                matchedRouteUrl = "<strong class=\"false\">NO MATCH!</strong>";
            }
            context.Response.Write(string.Format(htmlFormat, new object[]
			{
				routeDataRows,
				matchedRouteUrl,
				routes,
				context.Request.AppRelativeCurrentExecutionFilePath,
				dataTokensRows,
				generatedUrlInfo
			}));
        }

        private static string FormatRouteValueDictionary(RouteValueDictionary values)
        {
            if (values == null || values.Count == 0)
            {
                return "(null)";
            }
            string display = values.Keys.Aggregate(string.Empty, (current, key) => current + string.Format("{0} = {1}, ", key, values[key]));
            if (display.EndsWith(", "))
            {
                display = display.Substring(0, display.Length - 2);
            }
            return display;
        }
    }
}