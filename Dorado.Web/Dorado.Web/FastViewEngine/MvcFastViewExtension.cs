using Dorado.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine
{
    public static class MvcFastViewExtension
    {
        public static ActionResult Template(this Controller controller, string template)
        {
            return controller.Template(template, null);
        }

        public static ActionResult Template(this Controller controller, string template, object model)
        {
            return controller.Template(template, model, "text/html");
        }

        public static ActionResult Template(this Controller controller, string template, object model, string contentType)
        {
            if (string.IsNullOrEmpty(template))
                throw new ArgumentNullException("template", "template参数不能为空");
            if (model != null)
                controller.ViewData.Model = model;
            return new FastViewMemoryResult
            {
                Template = template,
                ContentType = contentType ?? "text/html"
            };
        }

        public static ActionResult FileTemplate(this Controller controller, string templatePath)
        {
            return controller.FileTemplate(templatePath, null);
        }

        public static ActionResult FileTemplate(this Controller controller, string templatePath, object model)
        {
            return controller.FileTemplate(templatePath, model, "text/html");
        }

        public static ActionResult FileTemplate(this Controller controller, string templatePath, object model, string contentType)
        {
            if (string.IsNullOrEmpty(templatePath))
                throw new ArgumentNullException("templatePath", "指定模板路径不能为空");
            if (model != null)
                controller.ViewData.Model = model;
            return new FastViewFileResult()
            {
                Template = templatePath,
                ContentType = contentType ?? "text/html"
            };
        }

        public static ActionResult ViewTemplate(this Controller controller, string viewName = "")
        {
            return controller.ViewTemplate(null, viewName);
        }

        public static ActionResult ViewTemplate(this Controller controller, object model, string viewName = "")
        {
            return controller.ViewTemplate(model, viewName, "text/html");
        }

        public static ActionResult ViewTemplate(this Controller controller, object model, string viewName, string contentType)
        {
            IViewEngine viewEngine = ViewEngines.Engines.FirstOrDefault(n => n.GetType() == typeof(FastViewVirtualPathViewEngine));
            if (viewEngine == null)
                throw new InvalidOperationException("没有找到视图引擎FastViewVirtualPathViewEngine，请确认该引擎是否已注册");

            if (string.IsNullOrEmpty(viewName))
                viewName = controller.RouteData.GetRequiredString("action");

            if (model != null)
                controller.ViewData.Model = model;

            ViewEngineResult result = ViewEngines.Engines.First(n => n.GetType() == typeof(FastViewVirtualPathViewEngine)).FindView(controller.ControllerContext, viewName, null, false);
            if (result.View == null)
            {
                StringBuilder sb = new StringBuilder();
                result.SearchedLocations.ForEach(n =>
                {
                    sb.Append(n);
                    sb.Append("\r\n");
                });

                throw new InvalidOperationException(string.Format("The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:\r\n {1}", viewName, sb.ToString()));
            }
            string templatePath = (result.View as FastView).Template;
            return new FastViewFileResult()
            {
                Template = HttpContext.Current.Server.MapPath(templatePath),
                ContentType = contentType ?? "text/html"
            };
        }
    }
}