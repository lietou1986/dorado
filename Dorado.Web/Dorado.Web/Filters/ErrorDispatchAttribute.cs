using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Extensions;
using System;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ErrorDispatchAttribute : FilterAttribute, IExceptionFilter
    {
        public const int ErrorStatusCode = 312;
        public const string AjaxHeader = "x-Dorado-ajax";
        public const string AjaxError = "x-Dorado-ajax-error";

        public string View
        {
            get;
            set;
        }

        public Type ExceptionType
        {
            get;
            set;
        }

        public ErrorDispatchAttribute()
        {
        }

        public ErrorDispatchAttribute(Type type)
            : this()
        {
            ExceptionType = type;
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
            {
                LoggerWrapper.Logger.Error("Action中发生异常", filterContext.Exception);
                return;
            }
            filterContext.ExceptionHandled = true;
            string ajaxHeader = filterContext.RequestContext.HttpContext.Request.Headers["x-Dorado-ajax"];
            if (!ajaxHeader.IsNaN())
            {
                filterContext.HttpContext.Response.AppendHeader("x-Dorado-ajax-error", HttpUtility.UrlEncode(filterContext.Exception.Message));
            }
            if (!View.IsNaN())
            {
                filterContext.HttpContext.Response.StatusCode = 312;
                ViewResult view = new ViewResult
                {
                    ViewName = View
                };
                view.ViewData.Model = filterContext.Exception;
                filterContext.Result = view;
                return;
            }
            filterContext.Result = new ContentResult
            {
                Content = filterContext.Exception.Message,
                ContentType = "text/html"
            };
        }
    }
}