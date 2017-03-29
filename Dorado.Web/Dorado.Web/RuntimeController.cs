using Dorado.Web.Fileset;
using Dorado.Web.Filters;
using RazorEngine;
using RazorEngine.Templating;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web
{
    public class RuntimeController : Controller
    {
        [Compress]
        public ActionResult MergedFileContent(string fileName, string extension, [Bind(Prefix = "v")] string version)
        {
            if (string.IsNullOrEmpty(fileName))
                return base.Content(string.Empty);
            return new MergedFileResult(fileName);
        }

        public ActionResult AjaxProxy(string url, string method, string param)
        {
            url += (string.IsNullOrEmpty(param) ? string.Empty : ("?" + param));
            method = (string.IsNullOrEmpty(method) ? "get" : method);
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.CookieContainer = new CookieContainer(base.Request.Cookies.Count);
            foreach (HttpCookie cookie in base.Request.Cookies)
            {
                httpRequest.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }
            httpRequest.Method = method;
            httpRequest.GetResponse();
            return Content(string.Empty);
        }

        public ActionResult MinifierStatus()
        {
            var template = System.Text.Encoding.UTF8.GetString(Resource.Templates_AjaxMinifierStatus);
            return Content(Engine.Razor.RunCompile(template, "AjaxMinifierStatus", null, new { Errors = MicrosoftAjaxMinifer.Errors, Level = MicrosoftAjaxMinifer.WarningLevel, CssSetting = MicrosoftAjaxMinifer.CssSetting, JavascriptSetting = MicrosoftAjaxMinifer.JavascriptSetting, EnableMinifier = StaticFilesetManager.EnableMinify }));
        }

        public ActionResult Navigations()
        {
            var template = System.Text.Encoding.UTF8.GetString(Resource.Templates_Navigation);
            return Content(Engine.Razor.RunCompile(template, "Navigation", null, NavigationProvider.GetAllNavigation()));
        }
    }
}