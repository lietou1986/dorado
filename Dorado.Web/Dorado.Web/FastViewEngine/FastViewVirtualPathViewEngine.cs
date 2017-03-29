using Dorado.Web.Extensions;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine
{
    public class FastViewVirtualPathViewEngine : VirtualPathProviderViewEngine
    {
        private readonly Dictionary<ViewEngineResultCacheKey, ViewEngineResult> _viewEngineResults = new Dictionary<ViewEngineResultCacheKey, ViewEngineResult>();
        private readonly object _syncHelper = new object();

        public string BaseViewPath
        {
            get;
            set;
        }

        public IFastViewEngine FastViewEngine
        {
            get;
            set;
        }

        public FastViewVirtualPathViewEngine()
            : this("Views")
        {
        }

        public FastViewVirtualPathViewEngine(string baseViewPath)
            : this(baseViewPath, FastViewEngineFactory.FileEngine)
        {
        }

        public FastViewVirtualPathViewEngine(IFastViewEngine fastViewEngine)
            : this("Views", fastViewEngine)
        {
        }

        public FastViewVirtualPathViewEngine(string baseViewPath, IFastViewEngine fastViewEngine)
        {
            FastViewEngine = fastViewEngine;
            BaseViewPath = baseViewPath;
            ViewLocationFormats = new[]
			{
				"~/" + baseViewPath + "/{1}/{0}.shtml",
				"~/" + baseViewPath + "/Shared/{0}.shtml",
				"~/" + baseViewPath + "/{1}/{0}.html",
				"~/" + baseViewPath + "/Shared/{0}.html"
			};
            PartialViewLocationFormats = ViewLocationFormats;
            AreaViewLocationFormats = new[]
			{
				"~/Areas/{2}/" + baseViewPath + "/{1}/{0}.shtml",
				"~/Areas/{2}/" + baseViewPath + "/Shared/{0}.shtml",
				"~/Areas/{2}/" + baseViewPath + "/{1}/{0}.html",
				"~/Areas/{2}/" + baseViewPath + "/Shared/{0}.html"
			};
            MasterLocationFormats = new string[0];
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return FindView(controllerContext, partialViewName, null, useCache);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            string areaName = controllerContext.RouteData.GetAreaName();
            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            ViewEngineResultCacheKey key = new ViewEngineResultCacheKey(areaName, controllerName, viewName);
            ViewEngineResult result;
            if (!useCache)
            {
                result = new ViewEngineResult(ViewLocationFormats);
                lock (_syncHelper)
                {
                    _viewEngineResults[key] = result;
                }
                return result;
            }
            lock (_syncHelper)
            {
                if (_viewEngineResults.TryGetValue(key, out result))
                {
                    return result;
                }
            }
            lock (_syncHelper)
            {
                if (_viewEngineResults.TryGetValue(key, out result))
                {
                    return result;
                }

                result = new ViewEngineResult(ViewLocationFormats);
                _viewEngineResults[key] = result;
                return result;
            }
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return new FastView
            {
                Template = partialPath,
                FastViewEngine = FastViewEngineFactory.FileEngine
            };
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return new FastView
            {
                Template = viewPath,
                FastViewEngine = FastViewEngineFactory.FileEngine
            };
        }
    }
}