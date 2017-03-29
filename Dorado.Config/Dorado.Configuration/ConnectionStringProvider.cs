using System;
using System.Collections.Specialized;
using Dorado.Core;
using Dorado.Core.Logger;

namespace Dorado.Configuration
{
    public class ConnectionStringProvider
    {
        #region Singleton

        private static readonly ConnectionStringProvider instance = new ConnectionStringProvider();

        private ConnectionStringProvider()
        {
        }

        static ConnectionStringProvider()
        {
        }

        public static ConnectionStringProvider Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion Singleton

        private static readonly string ConnectionStrings = "ConnectionStrings";

        private static readonly char SplitChar = '|';

        public static string Get(string name, params string[] sections)
        {
            try
            {
                if (sections.Length == 0)
                {
                    string settings = System.Web.Configuration.WebConfigurationManager.AppSettings[ConnectionStrings];
                    if (string.IsNullOrEmpty(settings))
                        return ConnectionStringCollection.Instance[name] ?? string.Empty;
                    sections = settings.Trim().Split(new char[] { SplitChar }, StringSplitOptions.RemoveEmptyEntries);
                }
                ConnectionStringCollection temp;
                NameValueCollection connectionStrings = new NameValueCollection();
                foreach (string sectionName in sections)
                {
                    temp = ConnectionStringCollection.Using(sectionName);
                    if (temp != null)
                        connectionStrings.Add(temp.Collection);
                }
                string[] values = connectionStrings.GetValues(name);
                return values == null ? string.Empty : values[0];
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(string.Format("{0}异常", ConnectionStrings) + ex);
                return string.Empty;
            }
        }
    }
}