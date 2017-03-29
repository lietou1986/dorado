using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Dorado.Web.FastViewEngine
{
    public class PageContext
    {
        private readonly HttpContextBase _httpContext;
        private readonly ViewDataDictionary _viewData;

        public HtmlHelper Html
        {
            get;
            set;
        }

        public UrlHelper Url
        {
            get;
            set;
        }

        public PageContext()
        {
        }

        public PageContext(ViewContext context)
            : this()
        {
            _httpContext = context.HttpContext;
            _viewData = context.ViewData;
        }

        public PageContext(ControllerContext context)
            : this()
        {
            Url = new UrlHelper(context.RequestContext);
            _httpContext = context.HttpContext;
            _viewData = context.Controller.ViewData;
        }

        public void SetContentType(string contentType)
        {
            _httpContext.Response.ContentType = contentType;
        }

        public void SetHeader(string name, string value)
        {
            _httpContext.Response.AppendHeader(name, value);
        }

        public string GetQuerystring(string query)
        {
            return _httpContext.Request.QueryString[query] ?? string.Empty;
        }

        public string GetParam(string paramName)
        {
            return _httpContext.Request[paramName] ?? string.Empty;
        }

        public string GetPost(string name)
        {
            return _httpContext.Request.Form[name] ?? string.Empty;
        }

        public string GetHeader(string name)
        {
            return _httpContext.Request.Headers[name] ?? string.Empty;
        }

        public object GetViewData(string name)
        {
            return _viewData[name] ?? string.Empty;
        }

        public void SetViewData(string name, string data)
        {
            _viewData[name] = data;
        }

        public string HtmlEncode(string html)
        {
            return _httpContext.Server.HtmlEncode(html);
        }

        public string HtmlDecode(string html)
        {
            return _httpContext.Server.HtmlDecode(html);
        }

        public string RenderJs(params string[] jsName)
        {
            StringWriter writer = new StringWriter();
            Html.RenderJs(writer, jsName);
            return writer.ToString();
        }

        public string RenderJs()
        {
            return Html.RenderJs();
        }

        public string RenderCss()
        {
            return Html.RenderCss();
        }

        public string ActionUrl(string area, string action, string controller)
        {
            if (string.IsNullOrEmpty(area))
            {
                return Url.Action(action, controller, new
                {
                    Area = area
                });
            }
            return Url.Action(action, controller);
        }

        public string RenderCss(params string[] cssNames)
        {
            StringWriter writer = new StringWriter();
            Html.RenderCss(writer, cssNames);
            return writer.ToString();
        }

        public string Partial(string viewName)
        {
            return Html.Partial(viewName).ToHtmlString();
        }

        public string Partial(string viewName, object model)
        {
            return Html.Partial(viewName, model).ToHtmlString();
        }
    }
}