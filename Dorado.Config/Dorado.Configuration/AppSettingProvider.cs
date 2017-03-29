using System;
using System.Collections.Specialized;
using Dorado.Core;
using Dorado.Core.Logger;

namespace Dorado.Configuration
{
    public class AppSettingProvider
    {
        #region Singleton

        private static readonly AppSettingProvider instance = new AppSettingProvider();

        private AppSettingProvider()
        {
        }

        static AppSettingProvider()
        {
        }

        public static AppSettingProvider Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion Singleton

        private static readonly string AppSettings = "AppSettings";

        private static readonly char SplitChar = '|';

        public static T Get<T>(string key, params string[] sections)
        {
            string val = Get(key, sections);
            if ("{null}".EndsWith(val))
            {
                return default(T);
            }
            return ConfigUtility.ParseTextValue<T>(val);
        }

        public static T Get<T>(string key, T defaultValue, params string[] sections)
        {
            string val = Get(key, sections);
            if (val.Length == 0)
                return defaultValue;

            if ("{null}".EndsWith(val))
            {
                return default(T);
            }
            return ConfigUtility.ParseTextValue<T>(val);
        }

        public static string Get(string key, params string[] sections)
        {
            try
            {
                if (sections.Length == 0)
                {
                    string settings = System.Web.Configuration.WebConfigurationManager.AppSettings[AppSettings];
                    if (string.IsNullOrEmpty(settings))
                        return AppSettingCollection.Instance[key] ?? string.Empty;
                    sections = settings.Trim().Split(new char[] { SplitChar }, StringSplitOptions.RemoveEmptyEntries);
                }
                AppSettingCollection temp;
                NameValueCollection appSettings = new NameValueCollection();
                foreach (string sectionName in sections)
                {
                    temp = AppSettingCollection.Using(sectionName);
                    if (temp != null)
                        appSettings.Add(temp.Collection);
                }
                string[] values = appSettings.GetValues(key);
                return values == null ? string.Empty : values[0];
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(string.Format("{0}异常->", AppSettings) + ex);
                return string.Empty;
            }
        }

        public static int GetInt(string key, params string[] sections)
        {
            string value = Get(key, sections);
            int result;
            int.TryParse(value, out result);
            return result;
        }

        public static bool GetBool(string key, params string[] sections)
        {
            string value = Get(key, sections);
            bool result;
            bool.TryParse(value, out result);
            return result;
        }

        public string this[string key, params string[] sections]
        {
            get
            {
                return Get(key, sections);
            }
        }
    }
}