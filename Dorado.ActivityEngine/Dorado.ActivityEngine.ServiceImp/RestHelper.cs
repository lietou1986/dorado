using System.IO;
using System.Net;

namespace Dorado.ActivityEngine.ServiceImp
{
    internal class RestHelper
    {
        static RestHelper()
        {
            ServicePointManager.Expect100Continue = false;
        }

        public static string PostJson(string json, string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = (long)json.Length;
            using (Stream stream = req.GetRequestStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                }
            }
            string result;
            using (Stream stream2 = req.GetResponse().GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream2))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }
    }
}