using System.Net;
using System.Text;
using System.Web;

namespace Dorado.VWS.Utils
{
    public class WebHttpHelper
    {
        /// <summary>
        /// 获取页面内容
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string GetWebContent(string Url)
        {
            string htmlContent = "";

            try
            {
                HttpWebRequest wreq = WebRequest.Create(Url) as HttpWebRequest;
                using (HttpWebResponse wresp = (HttpWebResponse)wreq.GetResponse())
                {
                    using (System.IO.Stream responseStream = wresp.GetResponseStream())
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.UTF8))
                        {
                            htmlContent = reader.ReadToEnd();
                        }
                    }
                }
                return htmlContent;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static string GetUrl(HttpRequest Request, string pageName)
        {
            string defaultPage = string.Format(@"{0}://{1}:{2}/{3}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port, pageName);
            return defaultPage;
        }
    }
}