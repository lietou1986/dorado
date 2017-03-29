using Dorado.Extensions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dorado.Web.Extensions
{
    public static class RouteExtensions
    {
        public static string GetAreaName(this RouteData routeData)
        {
            object obj2;
            if (routeData.DataTokens.TryGetValue("area", out obj2))
            {
                return (obj2 as string);
            }
            return routeData.Route.GetAreaName();
        }

        public static string GetAreaName(this RouteBase route)
        {
            var area = route as IRouteWithArea;
            if (area != null)
            {
                return area.Area;
            }
            var route2 = route as System.Web.Routing.Route;
            if ((route2 != null) && (route2.DataTokens != null))
            {
                return (route2.DataTokens["area"] as string);
            }
            return null;
        }

        /// <summary>
        /// Generates an identifier for the given route in the form "[{area}.]{controller}.{action}"
        /// </summary>
        public static string GenerateRouteIdentifier(this RouteData routeData)
        {
            string area = routeData.GetAreaName();
            string controller = routeData.GetRequiredString("controller");
            string action = routeData.GetRequiredString("action");

            return "{0}{1}.{2}".FormatInvariant(area.HasValue() ? area + "." : "", controller, action);
        }
    }
}