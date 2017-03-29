using System;
using System.Web;

namespace Dorado.Web.Anti
{
    public class LinkSecurityHttpModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Application_BeginRequest);
        }

        public void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            HttpContext context = application.Context;
            if (context.Request.UserAgent != null)
                AntiBrowerAttribute.CheckBrowers(context.Request.UserAgent);
            if (context.Request.UrlReferrer != null)
                AntiSearchEngineAttribute.CheckSearchEngines(context.Request.UrlReferrer.Host);
        }
    }
}