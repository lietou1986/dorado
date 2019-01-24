using Dorado.Core.ComponentModel;
using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Dorado.Platform
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class CommonHelper
    {
        /// <summary>
        /// 将字符串转成二进制 “10011100000000011100011111111101”
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StringToBinary(string s)
        {
            byte[] data = Encoding.Unicode.GetBytes(s);
            return ByteToBinaryString(data);
        }

        /// <summary>
        /// 将二进制 “10011100000000011100011111111101” 转成 字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string BinaryToString(string s)
        {
            byte[] data = BinaryStringToByte(s);
            return Encoding.Unicode.GetString(data, 0, data.Length);
        }

        public static string ByteToBinaryString(byte[] data)
        {
            StringBuilder result = new StringBuilder(data.Length * 8);

            foreach (byte b in data)
            {
                result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return result.ToString();
        }

        public static byte[] BinaryStringToByte(string s)
        {
            CaptureCollection cs = Regex.Match(s, @"([01]{8})+").Groups[1].Captures;
            byte[] data = new byte[cs.Count];
            for (int i = 0; i < cs.Count; i++)
            {
                data[i] = Convert.ToByte(cs[i].Value, 2);
            }
            return data;
        }

        /// <summary>
        /// byte截取
        /// </summary>
        /// <param name="src"></param>
        /// <param name="begin"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] SubBytes(byte[] src, int begin, int count)
        {
            byte[] bs = new byte[count];
            for (int i = begin; i < begin + count; i++) bs[i - begin] = src[i];
            return bs;
        }

        public static ExpandoObject ToExpando(object value)
        {
            Guard.ArgumentNotNull(() => value);

            var anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(value);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
            {
                expando.Add(item);
            }
            return (ExpandoObject)expando;
        }

        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }

        /// <summary>
        /// Returns an random interger number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = 2147483647)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <param name="findAppRoot">Specifies if the app root should be resolved when mapped directory does not exist</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        /// <remarks>
        /// This method is able to resolve the web application root
        /// even when it's called during design-time (e.g. from EF design-time tools).
        /// </remarks>
        public static string MapPath(string path, bool findAppRoot = true)
        {
            Guard.ArgumentNotNull(() => path);

            if (HostingEnvironment.IsHosted)
            {
                // hosted
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                // not hosted. For example, running in unit tests or EF tooling
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');

                var testPath = Path.Combine(baseDirectory, path);

                if (findAppRoot /* && !Directory.Exists(testPath)*/)
                {
                    // most likely we're in unit tests or design-mode (EF migration scaffolding)...
                    // find solution root directory first
                    var dir = FindSolutionRoot(baseDirectory);

                    // concat the web root
                    if (dir != null)
                    {
                        baseDirectory = Path.Combine(dir.FullName, "Presentation\\Dorado.Platform.Web");
                        testPath = Path.Combine(baseDirectory, path);
                    }
                }

                return testPath;
            }
        }

        public static bool IsDevEnvironment
        {
            get
            {
                if (!HostingEnvironment.IsHosted)
                    return true;

                if (HostingEnvironment.IsDevelopmentEnvironment)
                    return true;

                if (System.Diagnostics.Debugger.IsAttached)
                    return true;

                // if there's a 'Dorado.Platform.NET.sln' in one of the parent folders,
                // then we're likely in a dev environment
                if (FindSolutionRoot(HostingEnvironment.MapPath("~/")) != null)
                    return true;

                return false;
            }
        }

        private static DirectoryInfo FindSolutionRoot(string currentDir)
        {
            var dir = Directory.GetParent(currentDir);
            while (true)
            {
                if (dir == null || IsSolutionRoot(dir))
                    break;

                dir = dir.Parent;
            }

            return dir;
        }

        private static bool IsSolutionRoot(DirectoryInfo dir)
        {
            return File.Exists(Path.Combine(dir.FullName, "Dorado.Platform.sln"));
        }

        public static IDictionary<string, object> ObjectToDictionary(object obj)
        {
            Guard.ArgumentNotNull(() => obj);

            return FastProperty.ObjectToDictionary(
                obj,
                key => key.Replace("_", "-"));
        }

        /// <summary>
        /// Gets a setting from the application's <c>web.config</c> <c>appSettings</c> node
        /// </summary>
        /// <typeparam name="T">The type to convert the setting value to</typeparam>
        /// <param name="key">The key of the setting</param>
        /// <param name="defValue">The default value to return if the setting does not exist</param>
        /// <returns>The casted setting value</returns>
        public static T GetAppSetting<T>(string key, T defValue = default(T))
        {
            Guard.ArgumentNotEmpty(() => key);

            var setting = ConfigurationManager.AppSettings[key];

            if (setting == null)
            {
                return defValue;
            }

            return setting.Convert<T>();
        }

        private static bool TryAction<T>(Func<T> func, out T output)
        {
            Guard.ArgumentNotNull(() => func);

            try
            {
                output = func();
                return true;
            }
            catch
            {
                output = default(T);
                return false;
            }
        }
    }
}