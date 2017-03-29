using RazorEngine;
using RazorEngine.Templating;
using System.Web.Mvc;

namespace Dorado.Web
{
    public class WebExtensionAreaRegistration : AreaRegistration
    {
        public const string MergedFileRoutePartten = "runtime/merged/@Model.filename.@Model.version.@Model.extension";

        public override string AreaName
        {
            get
            {
                return "WebExtension";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            string routePartten = Engine.Razor.RunCompile(MergedFileRoutePartten, "MergedFileRoutePartten", null, new
            {
                filename = "filename",
                version = "version",
                extension = "extension"
            });
            context.MapRoute("WebExtension_staticfile", routePartten, new
            {
                action = "MergedFileContent",
                controller = "Runtime"
            });
            context.MapRoute("WebExtension_minifier_status", "runtime/minifier/status", new
            {
                action = "MinifierStatus",
                controller = "Runtime"
            });
            context.MapRoute("WebExtension_runtime_navigation", "runtime/navigation/all", new
            {
                action = "Navigations",
                controller = "Runtime"
            });
            context.MapRoute("WebExtension_default", AreaName + "/{controller}/{action}/{id}", new
            {
                action = "Index",
                id = UrlParameter.Optional
            });
        }
    }
}