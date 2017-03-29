using System.Web.Mvc;
using System.Web.Routing;

namespace Dorado.Web
{
    public static class AreaHelpers
    {
        public static string GetAreaName(RouteBase route)
        {
            IRouteWithArea routeWithArea = route as IRouteWithArea;
            if (routeWithArea != null)
                return routeWithArea.Area;
            System.Web.Routing.Route castRoute = route as System.Web.Routing.Route;
            if (castRoute != null && castRoute.DataTokens != null)
                return castRoute.DataTokens["area"] as string;
            return null;
        }

        public static string GetAreaName(RouteData routeData)
        {
            object area;
            if (routeData.DataTokens.TryGetValue("area", out area))
                return area as string;
            return GetAreaName(routeData.Route);
        }
    }
}