using Dorado.Core.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine
{
    public abstract class FastViewResult : ActionResult
    {
        public string Template
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public IFastViewEngine FastViewEngine
        {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (!string.IsNullOrEmpty(ContentType))
                context.HttpContext.Response.ContentType = ContentType;

            if (FastViewEngine is FastViewFileEngine)
                FastViewEngine.Process(CreateFastViewContext(context), context.HttpContext.Response.Output, Template);
            else
                FastViewEngine.Process(Template, context.HttpContext.Response.Output, CreateFastViewContext(context));
        }

        private static FastViewContext CreateFastViewContext(ControllerContext controllerContext)
        {
            FastViewContext context;
            object model = controllerContext.Controller.ViewData.Model;
            if (model != null)
            {
                if (model is DataArrayList)
                    context = new FastViewContext((DataArrayList)model);
                else if (model is DataArray)
                    context = new FastViewContext((DataArray)model);
                else if (model is IDictionary<string, object>)
                    context = new FastViewContext((IDictionary<string, object>)model);
                else if (model is IDictionary)
                    context = new FastViewContext((IDictionary)model);
                else
                    context = new FastViewContext();
            }
            else
                context = new FastViewContext();
            context.PageContext = new PageContext(controllerContext);
            context.HttpContext = controllerContext.HttpContext;
            context.HttpResponse = controllerContext.HttpContext.Response;
            context.ViewData = controllerContext.Controller.ViewData;
            context.TempData = controllerContext.Controller.TempData;
            context.RouteData = controllerContext.RouteData;
            context.Factory = FastView.ContextObjectFactory;
            return context;
        }
    }
}