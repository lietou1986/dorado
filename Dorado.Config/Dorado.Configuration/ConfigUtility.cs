using Dorado.Configuration.EnvironmentRouter;
using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Xml;

namespace Dorado.Configuration
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public sealed class ConfigUtility
    {
        #region Extend Methods

        /// <summary>
        /// Get the static Parse(string) method on the type supplied
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A delegate to the type's Parse(string) if it has one</returns>
        public static MethodInfo GetParseMethod(Type type)
        {
            const string parseMethod = "Parse";
            if (type == typeof(string))
            {
                return typeof(ConfigUtility).GetMethod(parseMethod, BindingFlags.Public | BindingFlags.Static);
            }
            var parseMethodInfo = type.GetMethod(parseMethod,
                                                 BindingFlags.Public | BindingFlags.Static, null,
                                                 new Type[] { typeof(string) }, null);

            return parseMethodInfo;
        }

        /// <summary>
        /// Gets the constructor info for T(string) if exists.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructorInfo(Type type)
        {
            foreach (ConstructorInfo ci in type.GetConstructors())
            {
                var ciTypes = ci.GetGenericArguments();
                var matchFound = (ciTypes.Length == 1 && ciTypes[0] == typeof(string)); //e.g. T(string)
                if (matchFound)
                {
                    return ci;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the value returned by the 'T.Parse(string)' method if exists otherwise 'new T(string)'.
        /// e.g. if T was a TimeSpan it will return TimeSpan.Parse(textValue).
        /// If there is no Parse Method it will attempt to create a new instance of the destined type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="textValue">The default value.</param>
        /// <returns>T.Parse(string) or new T(string) value</returns>
        public static T ParseTextValue<T>(string textValue)
        {
            var parseMethod = GetParseMethod(typeof(T));
            if (parseMethod == null)
            {
                var ci = GetConstructorInfo(typeof(T));
                if (ci == null)
                {
                    throw new TypeLoadException(string.Format("Error creating type {0} from text '{1}", typeof(T).Name, textValue));
                }
                var newT = ci.Invoke(null, new object[] { textValue });
                return (T)newT;
            }
            var value = parseMethod.Invoke(null, new object[] { textValue });
            return (T)value;
        }

        #endregion Extend Methods

        /// <summary>
        /// 判断是否为web应用程序
        /// </summary>
        public static bool IsWebApplication
        {
            get
            {
                //使用根web.config文件
                string AppVirtualPath = System.Web.HttpRuntime.AppDomainAppVirtualPath;
                if (!string.IsNullOrEmpty(AppVirtualPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static string baseDomain;

        public static string BaseDomain
        {
            get
            {
                if (baseDomain == null)
                {
                    baseDomain = WebConfigurationManager.AppSettings["baseDomain"];
                    if (baseDomain == null)
                        baseDomain = "vancl.com";
                }
                return baseDomain;
            }
        }

        /// <summary>
        /// 建立基于BaseDomain的绝对URL
        /// </summary>
        /// <param name="host">不包含域的主机名</param>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <returns></returns>
        public static string BuildUrl(string host, string relativeUrl)
        {
            if (string.IsNullOrEmpty(host))
                return string.Concat("http://", BaseDomain, relativeUrl);
            else
                return string.Concat("http://", host, ".", BaseDomain, relativeUrl);
        }

        /// <summary>
        /// 获取远程配置的环境
        /// </summary>
        public static NetworkEnvironment CurrentEnvironment
        {
            get
            {
                IEnvironmentRouter router = IPRouter.Instance;  //可以添加个策略容器
                return router.GetCurrentEnvironment();
            }
        }

        public static bool IsProd
        {
            get
            {
                return CurrentEnvironment == NetworkEnvironment.Prod;
            }
        }

        public static bool IsTest
        {
            get
            {
                return CurrentEnvironment == NetworkEnvironment.Test;
            }
        }

        public static bool IsDev
        {
            get
            {
                return CurrentEnvironment == NetworkEnvironment.Dev;
            }
        }

        public static bool IsLabs
        {
            get
            {
                return CurrentEnvironment == NetworkEnvironment.Labs;
            }
        }

        private static string executablePath;

        public static string ExecutablePath
        {
            get
            {
                if (executablePath == null)
                {
                    if (IsWebApplication)
                    {
                        executablePath = System.AppDomain.CurrentDomain.BaseDirectory;
                    }
                    else
                    {
                        System.Reflection.Assembly ass = System.Reflection.Assembly.GetEntryAssembly();
                        if (ass != null)
                            executablePath = ass.Location;
                        else
                            executablePath = System.AppDomain.CurrentDomain.BaseDirectory;
                    }
                }
                return executablePath;
            }
        }

        private static string applicationName;

        public static string ApplicationName
        {
            get
            {
                if (applicationName == null)
                {
                    applicationName = System.Web.Configuration.WebConfigurationManager.AppSettings["applicationName"];
                    if (string.IsNullOrEmpty(applicationName))
                        applicationName = CustomApplicationName;
                    if (string.IsNullOrEmpty(applicationName))
                        applicationName = EncodedExecutablePath;
                }
                return applicationName;
            }
        }

        private static string rootConfigFolder;

        public static string RootConfigFolder
        {
            get
            {
                if (rootConfigFolder == null)
                {
                    rootConfigFolder = WebConfigurationManager.AppSettings["configRoot"];
                    if (rootConfigFolder == null)
                        rootConfigFolder = DefaultRootConfigFolder;
                }
                return rootConfigFolder;
            }
        }

        private static string rootLogFolder;

        public static string RootLogFolder
        {
            get
            {
                if (rootLogFolder == null)
                {
                    rootLogFolder = System.Web.Configuration.WebConfigurationManager.AppSettings["logRoot"];
                    if (rootLogFolder == null)
                        rootLogFolder = DefaultRootLogFolder;
                }
                return rootLogFolder;
            }
        }

        //配置文件本地存放地址（从远程配置下载的配置文件也会存放在此目录）
        public const string DefaultRootConfigFolder = "c:\\Dorado.configs";

        public const string DefaultRootLogFolder = "c:\\Dorado.logfiles";

        public static string DefaultApplicationConfigFolder = Path.GetFullPath(Path.Combine(RootConfigFolder, ApplicationName));
        public static string DefaultApplicationLogFolder = Path.GetFullPath(Path.Combine(RootLogFolder, ApplicationName));

        public static string Combine(string folder, string file)
        {
            return Path.GetFullPath(Path.Combine(folder, file));
        }

        private static bool IsLetterOrNumber(char i)
        {
            return ((i >= 'A' && i <= 'Z') || (i >= 'a' && i <= 'z') || (i >= '0' && i <= '9'));
        }

        private static string EncodeToPath(string path)
        {
            char[] chs = path.ToCharArray();
            for (int i = 0; i < path.Length; i++)
            {
                if (!IsLetterOrNumber(chs[i]))
                {
                    chs[i] = '_';
                }
            }
            return new string(chs);
        }

        private static string encodedExecutablePath;

        public static string EncodedExecutablePath
        {
            get
            {
                encodedExecutablePath = "General";
                if (encodedExecutablePath == null)
                    encodedExecutablePath = EncodeToPath(ExecutablePath);
                return encodedExecutablePath;
            }
        }

        public static string CustomApplicationName
        {
            get
            {
                IEnumerable<Assembly> assemblys = AppDomain.CurrentDomain.GetAssemblies().Where(n => !n.FullName.StartsWith("System") && !n.FullName.StartsWith("System") && !n.FullName.StartsWith("mscorlib") && !n.FullName.StartsWith("vshost32") && !n.FullName.StartsWith("Microsoft"));
                foreach (Assembly assembly in assemblys)
                {
                    try
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.GetInterface("IConfiguration") != null && !type.IsAbstract)
                            {
                                IConfiguration configuration = assembly.CreateInstance(type.ToString(), true) as IConfiguration;
                                if (configuration != null)
                                    return configuration.ApplicationName;
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException rtle)
                    {
                        //捕获未加载的程序集
                        LoggerWrapper.Logger.Error("加载IConfiguration时出错 " + rtle.LoaderExceptions.FirstOrDefault());
                        continue;
                    }
                }
                return string.Empty;
            }
        }

        public static int GetIntValue(XmlReader reader, string name, int defaultValue)
        {
            int val;
            if (!int.TryParse(reader.GetAttribute(name), out val))
                val = defaultValue;
            return val;
        }

        public static bool GetBoolValue(XmlReader reader, string name, bool defaultValue)
        {
            bool val;
            if (!bool.TryParse(reader.GetAttribute(name), out val))
                val = defaultValue;
            return val;
        }

        public static string GetStringValue(XmlReader reader, string name, string defaultValue)
        {
            string val;
            val = reader.GetAttribute(name);
            if (val == null)
                val = defaultValue;
            return val;
        }

        public static void DumpStack(TextWriter writer)
        {
            StackTrace trace = new StackTrace();
            foreach (StackFrame frame in trace.GetFrames())
            {
                string file = frame.GetFileName();
                int line = frame.GetFileLineNumber();
                int column = frame.GetFileColumnNumber();
                string methodName = frame.GetMethod().Name;
                string clsName = frame.GetMethod().DeclaringType.FullName;
                writer.WriteLine(clsName + "." + methodName + "," + file + ":" + line);
            }
        }

        public static string ConvertEnvironment(string environment)
        {
            string result = string.Empty;
            switch (environment.ToLower())
            {
                case "production":
                    result = "app_prd";
                    break;

                case "development":
                    result = "app_dev";
                    break;

                case "testing":
                    result = "app_test";
                    break;

                case "demo":
                    result = "app_demo";
                    break;

                default:
                    result = "app_test";
                    break;
            }
            return result;
        }
    }
}