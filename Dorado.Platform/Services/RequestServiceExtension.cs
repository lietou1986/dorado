using Dorado.Configuration;
using Dorado.Core.Encrypt;
using Dorado.Extensions;
using Dorado.Platform.Extensions;
using Dorado.Services;
using System;
using System.Text;

namespace Dorado.Platform.Services
{
    public static class RequestServiceExtension
    {
        public static Result<T> RequestApi<T>(this RequestService requestService, int timeout = 5)
        {
            try
            {
                if (!requestService.GetParameters.ContainsKey("d"))
                    requestService.AddParams("d", AppSettingProvider.Get("DeviceId"));

                var tmpUrl = new StringBuilder();
                var timestamp = DateTime.Now.GetUnixTime().ToString();

                var index = 0;
                requestService.GetParameters.ForEach(n =>
                {
                    if (index > 0)
                        tmpUrl.Append("&");
                    var urlEncode = n.Value.UrlEncode();
                    if (urlEncode != null)
                        tmpUrl.Append(string.Format("{0}={1}", n.Key,
                            urlEncode));

                    index++;
                });

                tmpUrl.Append(string.Format("&{0}={1}", "key", AppSettingProvider.Get("API_KEY")));
                tmpUrl.Append(string.Format("&{0}={1}", "t", timestamp));

                tmpUrl.Append(string.Format("{0}", AppSettingProvider.Get("API_SECURIT")));

                var urlAction = requestService.Url.Replace(AppSettingProvider.Get("API_DOMAIN"), "/");
                urlAction = string.Format(urlAction.Contains("?") ? "{0}&{1}" : "{0}?{1}", urlAction, tmpUrl);

                var token = Cryption.MD5(urlAction.ToLower());

                requestService.AddParams("key", AppSettingProvider.Get("API_KEY"))
                    .AddParams("t", timestamp)
                    .AddParams("e", token.ToUpper());

                int apiTimeout;
                if (!int.TryParse(AppSettingProvider.Get("API_TIMEOUT"), out apiTimeout))
                    apiTimeout = timeout;
                return requestService.Request<T>(apiTimeout);
            }
            catch (Exception ex)
            {
                return new Result<T>(ResultStatus.Error, ex.Message);
            }
        }
    }
}