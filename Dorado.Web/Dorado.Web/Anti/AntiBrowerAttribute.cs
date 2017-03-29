using Dorado.Configuration;
using Dorado.Web.Configuration.Anti;
using Dorado.Web.Exceptions;
using System;
using System.Web.Mvc;

namespace Dorado.Web.Anti
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AntiBrowerAttribute : FilterAttribute, IAuthorizationFilter
    {
        public bool IsCheckBrower
        {
            get;

            set;
        }

        public AntiBrowerAttribute()
        {
            IsCheckBrower = true;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (IsCheckBrower)
            {
                CheckBrowers(filterContext.HttpContext.Request.UserAgent);
            }
        }

        public static void CheckBrowers(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return;
            }
            BrowerAndSearchEngineConfiguration browerAndSearchEngineConfiguration = BaseConfig<BrowerAndSearchEngineConfiguration>.Instance;
            foreach (BrowerSecurityItems key in browerAndSearchEngineConfiguration.BrowerSecurityItems)
            {
                if (!string.IsNullOrEmpty(key.Value) && userAgent.IndexOf(key.Value, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return;
            }
            foreach (BrowerBlackItems key2 in browerAndSearchEngineConfiguration.BrowerBlackItems)
            {
                if (!string.IsNullOrEmpty(key2.Value) && userAgent.IndexOf(key2.Value, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    throw new HttpBrowerInvalidReqeust(key2.Name, userAgent);
                }
            }
        }
    }
}