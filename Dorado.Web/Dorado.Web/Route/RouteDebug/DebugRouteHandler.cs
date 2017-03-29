using System.Web;
using System.Web.Routing;

namespace Dorado.Web.Route.RouteDebug
{
    public class DebugRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new DebugHttpHandler
            {
                RequestContext = requestContext
            };
        }
    }
}