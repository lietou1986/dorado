using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Vancl.IC.VWS.ClientService.Model;
using System.Web.Script.Serialization;
using log4net;

namespace Vancl.IC.VWS.ClientService
{
    public class HandlerHelper
    {
        private static ILog _logger = LogManager.GetLogger(typeof(HandlerHelper));

        public static int SignHandler(string ip, string version, string hostname, int iisstatus)
        {
            string url = string.Format("http://demovws2.vancl.com/handler/signinhandler.ashx?ip={0}&version={1}&hostname={2}&iisstatus={3}", ip, version, hostname, iisstatus);
            int interval = 60;
            int.TryParse(GetReponse(url), out interval);
            return interval;
        }

        public static IList<TaskEntity> TaskHandler(string ip)
        {
            string url = string.Format("http://demovws2.vancl.com/handler/taskhandler.ashx?ip={0}", ip);
            IList<TaskEntity> tasks = new List<TaskEntity>();

            string response = GetReponse(url);
            if (!string.IsNullOrEmpty(response))
            {
                var objs = new JavaScriptSerializer().Deserialize(response, typeof(List<TaskEntity>));
                return objs as List<TaskEntity>; 
            }
            return null;
        }

        public static bool TaskResultHandler(string ip, int taskid, bool success, string msg)
        {
            string url = string.Format("http://demovws2.vancl.com/handler/taskresulthandler.ashx?ip={0}&taskid={1}&result={2}&msg={3}", ip, taskid, success ? 0 : 1, msg ?? string.Empty);
            string response = GetReponse(url);
            return "1".Equals(response);
        }

        private static string GetReponse(string url)
        {
            string responseFromServer = string.Empty;
            try
            {
                _logger.DebugFormat("Request URL : {0}", url);
                WebRequest request = WebRequest.Create(url);
                
                // Get the response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    _logger.DebugFormat("Response Callback");

                    // Get the stream containing content returned by the server.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            // Read the content.
                            responseFromServer = reader.ReadToEnd();

                            // Cleanup the streams and the response.
                            reader.Close();
                        }
                        dataStream.Close();
                    }
                    response.Close();
                }

                _logger.DebugFormat("Response : {0}", responseFromServer);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return responseFromServer;
        }
    }
}
