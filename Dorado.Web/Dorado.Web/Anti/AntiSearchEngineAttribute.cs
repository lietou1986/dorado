using Dorado.Configuration;
using Dorado.Web.Configuration.Anti;
using Dorado.Web.Exceptions;
using System;
using System.Web.Mvc;

namespace Dorado.Web.Anti
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AntiSearchEngineAttribute : FilterAttribute, IAuthorizationFilter
    {
        public bool IsCheckSearchEngine
        {
            get;
            set;
        }

        public AntiSearchEngineAttribute()
        {
            IsCheckSearchEngine = true;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (IsCheckSearchEngine && filterContext.HttpContext.Request.UrlReferrer != null)
                CheckSearchEngines(filterContext.HttpContext.Request.UrlReferrer.Host);
        }

        public static void CheckSearchEngines(string host)
        {
            if (string.IsNullOrEmpty(host))
                return;
            BrowerAndSearchEngineConfiguration browerAndSearchEngineConfiguration = BaseConfig<BrowerAndSearchEngineConfiguration>.Instance;
            foreach (SearchEngineSecurityItems key in browerAndSearchEngineConfiguration.SearchEngineSecurityItems)
            {
                if (!string.IsNullOrEmpty(key.Value) && host.IndexOf(key.Value, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return;
            }
            foreach (SearchEngineBlackItems key2 in browerAndSearchEngineConfiguration.SearchEngineBlackItems)
            {
                if (!string.IsNullOrEmpty(key2.Value) && host.IndexOf(key2.Value, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    throw new HttpSearchEngineInvalidReqeust(key2.Name, host);
            }
        }
    }
}