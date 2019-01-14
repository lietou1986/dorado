using Dorado.Core.Data;
using Dorado.Extensions;
using Dorado.Ioc;
using Dorado.Web.Exceptions;
using Dorado.Web.Filters;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace Dorado.Web.Extensions
{
    public static class ControllerExtensions
    {
        internal static ConnContainer GetConnContainer(this ControllerBase controller)
        {
            object containerObj;
            if (!controller.ViewData.TryGetValue(ConnContainer.ContainerName, out containerObj))
            {
                containerObj = new ConnContainer();
                controller.ViewData.Add(ConnContainer.ContainerName, containerObj);
            }
            return (ConnContainer)containerObj;
        }

        public static Conn GetConn(this ControllerBase controller, string connName = "Default")
        {
            ConnContainer container = controller.GetConnContainer();
            if (!container.ContainsKey(connName))
                throw new ConnNotFountException(connName, controller);
            object srv = container[connName];
            if (srv == null)
                throw new ConnUnvailableException(connName, controller);
            return (Conn)srv;
        }

        internal static ServiceContainer GetContainer(this ControllerBase controller)
        {
            object containerObj = null;
            if (!controller.TempData.TryGetValue(ServiceContainer.Consts.ServiceContainerName, out containerObj))
            {
                containerObj = new ServiceContainer();
                controller.TempData.Add(ServiceContainer.Consts.ServiceContainerName, containerObj);
            }
            return (ServiceContainer)containerObj;
        }
        /// <summary>
        /// 获取根据Controller或Action中指定的服务类型的实例。
        /// </summary>
        /// <typeparam name="T">服务的类型</typeparam>
        /// <param name="controller">控制器</param>
        /// <returns>服务的实例</returns>
        public static T GetService<T>(this Controller controller)
            where T : class
        {
            ServiceContainer container = controller.GetContainer();
            if (container.ContainsKey(typeof(T)))
            {
                object srv = container[typeof(T)];
                if (srv == null)
                {
                    throw new ServiceUnvailableException(typeof(T));
                }
                return (T)srv;
            }
            else
            {
                throw new ServiceNotFountException(typeof(T));
            }
        }

        /// <summary>
        /// 取得页面get数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller">The controller.</param>
        /// <param name="paraName">Name of the para.</param>
        /// <returns></returns>
        public static T Get<T>(this Controller controller, string paraName)
        {
            NameValueCollection nvc = controller.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : default(T);
        }

        public static string[] Get(this Controller controller, string paraName)
        {
            NameValueCollection nvc = controller.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc.GetValues(paraName) : new string[] { };
        }

        /// <summary>
        /// 取得页面post数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller">The controller.</param>
        /// <param name="paraName">Name of the para.</param>
        /// <returns></returns>
        public static T Post<T>(this Controller controller, string paraName)
        {
            NameValueCollection nvc = controller.Request.Form;

            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : default(T);
        }

        public static string[] Post(this Controller controller, string paraName)
        {
            NameValueCollection nvc = controller.Request.Form;
            return nvc.IsHas(paraName) ? nvc.GetValues(paraName) : new string[] { };
        }

        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this ControllerBase controller)
        {
            return RenderPartialViewToString(controller, null, null);
        }

        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="viewName">View name</param>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this ControllerBase controller, string viewName)
        {
            return RenderPartialViewToString(controller, viewName, null);
        }

        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this ControllerBase controller, object model)
        {
            return RenderPartialViewToString(controller, null, model);
        }

        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this ControllerBase controller, string viewName, object model)
        {
            if (viewName.IsEmpty())
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName.EmptyNull());

                ThrowIfViewNotFound(viewResult, viewName);

                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Render view to string
        /// </summary>
        /// <returns>Result</returns>
        public static string RenderViewToString(this ControllerBase controller)
        {
            return RenderViewToString(controller, null, null, null);
        }

        /// <summary>
        /// Render view to string
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderViewToString(this ControllerBase controller, object model)
        {
            return RenderViewToString(controller, null, null, model);
        }

        /// <summary>
        /// Render view to string
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="viewName">View name</param>
        /// <returns>Result</returns>
        public static string RenderViewToString(this ControllerBase controller, string viewName)
        {
            return RenderViewToString(controller, viewName, null, null);
        }

        /// <summary>
        /// Render view to string
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="viewName">View name</param>
        /// <param name="masterName"></param>
        /// <returns>Result</returns>
        public static string RenderViewToString(this ControllerBase controller, string viewName, string masterName)
        {
            return RenderViewToString(controller, viewName, masterName, null);
        }

        /// <summary>
        /// Render view to string
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="viewName">View name</param>
        /// <param name="masterName">Master name</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderViewToString(this ControllerBase controller, string viewName, string masterName, object model)
        {
            if (viewName.IsEmpty())
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName.EmptyNull(), masterName.EmptyNull());

                ThrowIfViewNotFound(viewResult, viewName);

                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        private static void ThrowIfViewNotFound(ViewEngineResult viewResult, string viewName)
        {
            // if view not found, throw an exception with searched locations
            if (viewResult.View == null)
            {
                var locations = new StringBuilder();
                locations.AppendLine();

                foreach (string location in viewResult.SearchedLocations)
                {
                    locations.AppendLine(location);
                }

                throw new InvalidOperationException(String.Format("The view '{0}' or its master was not found, searched locations: {1}", viewName, locations));
            }
        }

        public static ActionResult RedirectLocal(this Controller controller, string redirectUrl, Func<ActionResult> invalidUrlBehavior)
        {
            if (!String.IsNullOrWhiteSpace(redirectUrl) && controller.Request.IsLocalUrl(redirectUrl))
            {
                return new RedirectResult(redirectUrl);
            }
            return invalidUrlBehavior != null ? invalidUrlBehavior() : null;
        }

        public static ActionResult RedirectLocal(this Controller controller, string redirectUrl)
        {
            return RedirectLocal(controller, redirectUrl, (string)null);
        }

        public static ActionResult RedirectLocal(this Controller controller, string redirectUrl, string defaultUrl)
        {
            if (controller.Request.IsLocalUrl(redirectUrl))
            {
                return new RedirectResult(redirectUrl);
            }

            return new RedirectResult(defaultUrl ?? "~/");
        }

        #region jsonp

        public static JsonpViewResult JsonpView(this Controller controller)
        {
            return controller.JsonpView(null, null, null);
        }

        public static JsonpViewResult JsonpView(this Controller controller, object model)
        {
            return controller.JsonpView(null, null, model);
        }

        public static JsonpViewResult JsonpView(this Controller controller, string viewName)
        {
            return controller.JsonpView(viewName, null, null);
        }

        public static JsonpViewResult JsonpView(this Controller controller, string viewName, string masterName)
        {
            return controller.JsonpView(viewName, masterName, null);
        }

        public static JsonpViewResult JsonpView(this Controller controller, string viewName, object model)
        {
            return controller.JsonpView(viewName, null, model);
        }

        public static JsonpViewResult JsonpView(this Controller controller, string viewName, string masterName, object model)
        {
            if (model != null)
                controller.ViewData.Model = model;
            return new JsonpViewResult
            {
                ViewName = viewName,
                MasterName = masterName,
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }

        public static JsonpViewResult JsonpView(this Controller controller, IView view)
        {
            return controller.JsonpView(view, null);
        }

        public static JsonpViewResult JsonpView(this Controller controller, IView view, object model)
        {
            if (model != null)
                controller.ViewData.Model = model;
            return new JsonpViewResult
            {
                View = view,
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }

        public static JsonResult Jsonp(this Controller controller, object data)
        {
            return new FastJsonResult
            {
                Data = data
            };
        }

        #endregion jsonp
    }
}