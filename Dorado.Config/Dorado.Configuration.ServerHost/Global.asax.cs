using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Dorado.Configuration.ServerHost
{
    public class Global : System.Web.HttpApplication
    {
        #region protected members

        protected void Application_Start(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            LoggerWrapper.Logger.Info("Web Starting...");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string strCurrentPath = Request.Path.ToLower();

            if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["debug"]))
            {
                var info = string.Format("Begin Request: {0}", strCurrentPath);

                try
                {
                    if (strCurrentPath.ToLower().IndexOf("configversionhandler.ashx") >= 0)
                    {
                        XmlSerializer xser = new XmlSerializer(typeof(RemoteConfigSectionCollection));
                        RemoteConfigSectionCollection rcc = (RemoteConfigSectionCollection)xser.Deserialize(Request.InputStream);
                        Request.InputStream.Seek(0, SeekOrigin.Begin);
                        info += string.Format("\r\n\tApplication:{0} Machine:{1}", rcc.Application, rcc.Machine);
                    }
                    else if ((strCurrentPath.ToLower().IndexOf("resourcemanagerhandler.ashx") >= 0) || (strCurrentPath.ToLower().IndexOf("configmanagerhandler.ashx") >= 0))
                    {
                        XmlSerializer xser = new XmlSerializer(typeof(RemoteConfigManagerDto));
                        RemoteConfigManagerDto rcm = (RemoteConfigManagerDto)xser.Deserialize(Request.InputStream);
                        Request.InputStream.Seek(0, SeekOrigin.Begin);
                        info += string.Format("\r\n\tCommand:{0} OperatorID:{1} Application:{2} Machine:{3}", rcm.Operation.Command, rcm.Operation.OperatorId, rcm.RemoteConfigSections.Application, rcm.RemoteConfigSections.Machine);
                    }
                }
                catch (Exception ex)
                {
                    {
                        Request.InputStream.Seek(0, SeekOrigin.Begin);
                        byte[] bytes = new byte[Request.InputStream.Length + 1];
                        Request.InputStream.Read(bytes, 0, (int)(Request.InputStream.Length));
                        string streamContent = new UTF8Encoding().GetString(bytes);
                        Request.InputStream.Seek(0, SeekOrigin.Begin);

                        LoggerWrapper.Logger.Error(streamContent, ex);
                    }
                }

                LoggerWrapper.Logger.Info(info);
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (!bool.Parse(System.Configuration.ConfigurationManager.AppSettings["debug"])) return;
            string strCurrentPath = Request.Path;
            LoggerWrapper.Logger.Info(string.Format("End Request: {0}", strCurrentPath));
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                LoggerWrapper.Logger.Error("Application error", new Exception(string.Format("ErrorPath:{0}", Request.Path), ex));
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
            LoggerWrapper.Logger.Info("Web ending...");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LoggerWrapper.Logger.Error("Unhandled exception on thread", ex);
            }
        }

        #endregion protected members
    }
}