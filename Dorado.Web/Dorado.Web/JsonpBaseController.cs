using System.Text;
using System.Web.Mvc;

namespace Dorado.Web
{
    public class JsonpBaseController : Controller
    {
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new FastJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        public JsonpViewResult JsonpView()
        {
            return JsonpView(null, null, null);
        }

        public JsonpViewResult JsonpView(object model)
        {
            return JsonpView(null, null, model);
        }

        public JsonpViewResult JsonpView(string viewName)
        {
            return JsonpView(viewName, null, null);
        }

        public JsonpViewResult JsonpView(string viewName, string masterName)
        {
            return JsonpView(viewName, masterName, null);
        }

        public JsonpViewResult JsonpView(string viewName, object model)
        {
            return JsonpView(viewName, null, model);
        }

        public JsonpViewResult JsonpView(string viewName, string masterName, object model)
        {
            if (model != null)
                base.ViewData.Model = model;
            return new JsonpViewResult
            {
                ViewName = viewName,
                MasterName = masterName,
                ViewData = base.ViewData,
                TempData = base.TempData
            };
        }

        public JsonpViewResult JsonpView(IView view)
        {
            return JsonpView(view, null);
        }

        public JsonpViewResult JsonpView(IView view, object model)
        {
            if (model != null)
                base.ViewData.Model = model;
            return new JsonpViewResult
            {
                View = view,
                ViewData = base.ViewData,
                TempData = base.TempData
            };
        }
    }
}