using System.Web.Routing;

namespace Dorado.Web.Route.RouteDebug
{
    public static class RouteDebugger
    {
        public static void RewriteRoutesForTesting(RouteCollection routes)
        {
            using (routes.GetReadLock())
            {
                bool foundDebugRoute = false;
                foreach (RouteBase routeBase in routes)
                {
                    System.Web.Routing.Route route = routeBase as System.Web.Routing.Route;
                    if (route != null)
                    {
                        route.RouteHandler = new DebugRouteHandler();
                    }
                    if (route == DebugRoute.Singleton)
                    {
                        foundDebugRoute = true;
                    }
                }
                if (!foundDebugRoute)
                {
                    routes.Add(DebugRoute.Singleton);
                }
            }
        }
    }
}