namespace Dorado.Web.Route.RouteDebug
{
    public class DebugRoute : System.Web.Routing.Route
    {
        private static readonly DebugRoute singleton = new DebugRoute();

        public static DebugRoute Singleton
        {
            get
            {
                return singleton;
            }
        }

        private DebugRoute()
            : base("{*catchall}", new DebugRouteHandler())
        {
        }
    }
}