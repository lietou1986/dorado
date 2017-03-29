using Dorado.Extensions;
using System.Web.Mvc;

namespace Dorado.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string CommandName(this UrlHelper helper, string action, string controller, string area)
        {
            if (string.IsNullOrEmpty(area))
            {
                return controller + "_" + action;
            }
            return string.Concat(area, "_", controller, "_", action);
        }

        public static string Referrer(this UrlHelper urlHelper, string fallbackUrl = "")
        {
            var request = urlHelper.RequestContext.HttpContext.Request;
            if (request.UrlReferrer != null && request.UrlReferrer.ToString().HasValue())
            {
                return request.UrlReferrer.ToString();
            }

            return fallbackUrl;
        }
    }
}