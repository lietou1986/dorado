using Dorado.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine
{
    public class FastView : IView, IViewDataContainer
    {
        public static ContextObjectFactory ContextObjectFactory = new ContextObjectFactory();

        public IFastViewEngine FastViewEngine
        {
            get;
            set;
        }

        public string Template
        {
            get;
            set;
        }

        public ViewDataDictionary ViewData
        {
            get;
            set;
        }

        public virtual void Render(ViewContext viewContext, TextWriter writer)
        {
            if (string.IsNullOrEmpty(Template))
                throw new ArgumentNullException("Template", "模板不能为空");
            FastViewEngine.Process(CreateFastViewContext(viewContext), writer, Template.Substring(1));
        }

        private FastViewContext CreateFastViewContext(ViewContext viewContext)
        {
            FastViewContext context;
            object model = viewContext.ViewData.Model;
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

            ViewData = viewContext.ViewData;
            PageContext page = new PageContext(viewContext);
            page.Html = new HtmlHelper(viewContext, this);
            page.Url = new UrlHelper(viewContext.RequestContext);
            context.PageContext = page;
            context.HttpContext = viewContext.HttpContext;
            context.HttpResponse = viewContext.HttpContext.Response;
            context.ViewData = viewContext.Controller.ViewData;
            context.TempData = viewContext.Controller.TempData;
            context.RouteData = viewContext.RouteData;
            context.Factory = FastView.ContextObjectFactory;
            return context;
        }
    }
}