using System;
using System.Collections;

//using log4net.Core;
//using log4net.Config;
//using log4net;
//using Dorado.Core;using Dorado.Core.Logger;
using System.Collections.Generic;
using System.Collections.Specialized;

//using Dorado.Cryptography;
//
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Dorado.ESB.Common.Utility
{
    /// <summary>Helper Class</summary>
    public static class Helper
    {
        #region ÅÐ¶ÏÊÇ·ñµÇÂ¼

        //public static int IsLogin()
        //{
        //    HttpCookie myUserInfoCookie = Dorado.Application.Web.Context.RequestContext.Instance.GetCookieFromContext(Dorado.Application.Web.Utilities.Cookies.CookieKeys.USER_INFO_COOKIE_KEY);
        //    if (myUserInfoCookie != null && !StringUtility.IsNullOrEmpty(myUserInfoCookie.Value))
        //    {
        //        MyUserInfo userInfo = new MyUserInfo(HttpUtility.UrlDecode(myUserInfoCookie.Value).Replace(' ', '+'), true);
        //        return userInfo.UserID;
        //    }
        //    else
        //    {
        //        return -1;
        //    }
        //}

        //public static string GetEncryptedMyUserInfoFromCookie()
        //{
        //    string encrytedUserInfo = null;
        //    HttpCookie myUserInfoCookie = Dorado.Application.Web.Context.RequestContext.Instance.GetCookieFromContext(Dorado.Application.Web.Utilities.Cookies.CookieKeys.USER_INFO_COOKIE_KEY);
        //    if (myUserInfoCookie != null && !StringUtility.IsNullOrEmpty(myUserInfoCookie.Value))
        //    {
        //        encrytedUserInfo = myUserInfoCookie.Value;
        //        //MyUserInfo userInfo = new MyUserInfo(HttpUtility.UrlDecode(myUserInfoCookie.Value).Replace(' ', '+'), true);
        //        //return userInfo.UserID;
        //        //userInfo.UserIDEncrypted
        //    }
        //    return encrytedUserInfo;
        //}

        #endregion ÅÐ¶ÏÊÇ·ñµÇÂ¼

        /// <summary>Convert an object to an integer type, supplying a default value if parsing fails.</summary>
        /// <param name="val">Value to convert</param>
        /// <param name="defaultVal">Default value to supply if parsing fails.</param>
        /// <returns>Integer value</returns>
        public static int ToInt(object val, object defaultVal)
        {
            string str = StringHelper.ToString(val);

            int x;

            if (int.TryParse(str, out x))
            {
                return x;
            }
            else
            {
                return (int)defaultVal;
            }
        }

        #region Individual item Converters

        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string ToString(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return data.ToString();
            }
        }

        /// <summary>
        /// Toes the int.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static int? ToInt(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(data);
            }
        }

        /// <summary>
        /// Toes the int32.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Int32? ToInt32(object data)
        {
            return ToInt(data);
        }

        /// <summary>
        /// Toes the int16.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Int16? ToInt16(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToInt16(data);
            }
        }

        /// <summary>
        /// Toes the int64.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Int64? ToInt64(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToInt64(data);
            }
        }

        /// <summary>
        /// Toes the double.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Double? ToDouble(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToDouble(data);
            }
        }

        /// <summary>
        /// Toes the decimal.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Decimal? ToDecimal(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToDecimal(data);
            }
        }

        /// <summary>
        /// Convert an object to a decimal type, supplying a default value if parsing fails.
        /// </summary>
        /// <param name="val">Value to convert</param>
        /// <param name="defaultVal">Default value to supply if parsing fails</param>
        /// <returns>Decimal value</returns>
        public static Decimal ToDecimal(object val, object defaultVal)
        {
            string str = StringHelper.ToString(val);
            Decimal x;
            if (decimal.TryParse(str, out x))
            {
                return x;
            }
            else
            {
                return Convert.ToDecimal(defaultVal);
            }
        }

        /// <summary>
        /// Toes the date time.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToDateTime(data);
            }
        }

        /// <summary>
        /// Toes the boolean.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Boolean? ToBoolean(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToBoolean(data);
            }
        }

        /// <summary>
        /// Toes the Single.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Single? ToSingle(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToSingle(data);
            }
        }

        /// <summary>
        /// Toes the Byte.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Byte? ToByte(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToByte(data);
            }
        }

        /// <summary>
        /// Toes the GUID.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Guid? ToGuid(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return new Guid(data.ToString());
            }
        }

        /// <summary>
        /// Toes the URI.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Uri ToUri(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return null;
            }
            else
            {
                return new Uri(data.ToString());
            }
        }

        #endregion Individual item Converters

        #region ContextHelper

        public delegate T Converter<T>(string value);

        //review - rajiv try the commented regex.
        public static T ExtractQueryStringValue<T>(string queryString, string parameterName, Converter<T> converter)
        {
            string querystringFormat = @"^(.*?(\?(.*?))?)?$"; //@"(?<name>[^=&]+)=(?<value>[^&]+)&";
            Regex regex = new Regex(querystringFormat);
            string[] parameters;
            if (regex.IsMatch(queryString))
            {
                Match rxMatch = regex.Match(queryString);
                if (!string.IsNullOrEmpty(rxMatch.Groups[3].Value))
                {
                    parameters = rxMatch.Groups[3].Value.Split('&');
                    foreach (string parameter in parameters)
                    {
                        if (parameter.StartsWith(string.Format("{0}=", parameterName)))
                        {
                            string value = parameter.Replace(string.Format("{0}=", parameterName), string.Empty);
                            return converter(value);
                        }
                    }
                    return default(T);
                }
                else
                    return default(T);
            }
            else
                return default(T);
        }

        #endregion ContextHelper

        #region ParseQueryString

        /// <summary>
        /// The method would return a boolean with an ref parameter of type Dcitionary
        /// </summary>
        /// <param name="Querystring">querystring</param>
        /// <param name="Parameters">Dictionary</param>
        /// <returns></returns>
        public static bool TryExtractParameters(string Querystring, ref IDictionary<string, object> Parameters, out string Message)
        {
            Message = string.Empty;
            Check.ValidateQueryString(Querystring);
            NameValueCollection nameValues = HttpUtility.ParseQueryString(Querystring);

            foreach (string str in nameValues.AllKeys)
            {
                if (!Parameters.ContainsKey(str))
                {
                    Message = string.Format("The parameter {0} was not found in the input string.", str);
                    return false;
                }
                else
                    Parameters[str] = nameValues[str];
            }

            return true;
        }

        #endregion ParseQueryString

        public static string ReplaceLocalhostToIPAdress(string urlStr)
        {
            string loolback = "Microsoft Loopback Adapter";
            string hostIP = String.Empty;
            NetworkInterface networkInterfaces = NetworkInterface.GetAllNetworkInterfaces().SingleOrDefault(c => c.Description == loolback);
            if (networkInterfaces != null)
            {
                hostIP = networkInterfaces.GetIPProperties().UnicastAddresses.SingleOrDefault(c => c.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Address.ToString();
                IEnumerable<NetworkInterface> arrNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces().Where(c => c.OperationalStatus == OperationalStatus.Up && c.NetworkInterfaceType != NetworkInterfaceType.Loopback && c.Description.ToString() != loolback && c.NetworkInterfaceType != NetworkInterfaceType.Tunnel);
                foreach (NetworkInterface objNetworkInterface in arrNetworkInterfaces)
                {
                    UnicastIPAddressInformationCollection unicastIPAddressInformationCollection = objNetworkInterface.GetIPProperties().UnicastAddresses;
                    foreach (UnicastIPAddressInformation ip in unicastIPAddressInformationCollection)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && hostIP.Substring(0, hostIP.LastIndexOf('.')) == ip.Address.ToString().Substring(0, ip.Address.ToString().LastIndexOf('.')))
                        {
                            hostIP = ip.Address.ToString();
                            break;
                        }
                    }
                }
            }
            else
            {
                System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                foreach (IPAddress address in addressList)
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        hostIP = address.ToString();
                        break;
                    }
                }
            }

            if (hostIP != String.Empty && urlStr.Contains("localhost"))
                urlStr = urlStr.Replace("localhost", hostIP);

            return urlStr;
        }

        public static bool CheckIsChinaUser(int userId)
        {
            if (userId < 1300000000 || userId > 1400000000)
                return false;
            else
                return true;
        }
    }

    /// <summary>String Helper Class</summary>
    public static class StringHelper
    {
        /// <summary>ToString implementation - null remains null, otherwise the object's ToString result</summary>
        /// <param name="obj">Object to convert</param>
        /// <returns>Null for null argument, otherwise the object's ToString result.</returns>
        public static string ToString(object obj)
        {
            return obj == null ? null : obj + "";
        }

        /// <summary>Converts an object into its ToString implementation, or an empty string if it is null.</summary>
        /// <param name="obj">object to convert</param>
        /// <returns>Empty string for null argument, otherwise the object's ToString result.</returns>
        public static string ToEmptyString(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        /// <summary>Delimits a list of objects with a specified delimeter.</summary>
        /// <param name="delim">delimiter</param>
        /// <param name="values">List of objects</param>
        /// <returns>delimited string</returns>
        public static string Delimit(string delim, IList values)
        {
            Check.NotNull(delim, "delimeter");
            Check.NotNull(values, "values");

            StringBuilder builder = new StringBuilder(75 * values.Count);

            int i = 0;
            foreach (object obj in values)
            {
                if (obj != null)
                {
                    builder.Append(obj);
                }

                if (i < values.Count - 1)
                {
                    builder.Append(delim);
                }

                i++;
            }

            return builder.ToString();
        }
    }

    /// <summary>Check - Validation and verification support</summary>
    public static class Check
    {
        #region NotNull

        /// <summary>Not Null Test - Object cannot be null</summary>
        /// <param name="obj">Object to test</param>
        /// <param name="objectName">Name of object to test</param>
        public static void NotNull(object obj, string objectName)
        {
            NotNull(obj, objectName, null);
        }

        /// <summary>Not Null Test - Object cannot be null</summary>
        /// <param name="obj">Object to test</param>
        /// <param name="objectName">Name of object to test</param>
        /// <param name="message">Message to raise as exception if object is empty</param>
        public static void NotNull(object obj, string objectName, string message)
        {
            if (obj == null)
            {
                throw new NullReferenceException(string.Format("The object, {0}, can not be null. {1}", objectName, message));
            }
        }

        #endregion NotNull

        #region NotEmpty

        /// <summary>Not Empty Test - byte array cannot be empty</summary>
        /// <param name="array">Byte array to test</param>
        /// <param name="paramName">Parameter Name to test</param>
        public static void NotEmpty(byte[] array, string paramName)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException("The byte array, " + paramName + ", can't be empty.", paramName);
            }
        }

        /// <summary>Not Empty Test - Guid cannot be empty</summary>
        /// <param name="guid">Guid to test</param>
        /// <param name="paramName">Parameter Name to test</param>
        public static void NotEmpty(Guid guid, string paramName)
        {
            if (guid == Guid.Empty)
            {
                throw new ArgumentException("The guid, " + paramName + ", can't have an empty value.", paramName);
            }
        }

        /// <summary>Not Empty Test - Integer cannot be empty</summary>
        /// <param name="x">nullable integer to test</param>
        /// <param name="paramName">Parameter Name to test</param>
        public static void NotEmpty(int? x, string paramName)
        {
            if (!x.HasValue)
            {
                throw new ArgumentException("The int, " + paramName + ", can't have an empty value.", paramName);
            }
        }

        /// <summary>Not Empty Test - Unnamed string cannot be empty</summary>
        /// <param name="str">string to test</param>
        public static void NotEmpty(string str)
        {
            NotEmpty(str, "[unnamed]");
        }

        /// <summary>Not Empty Test - String cannot be empty</summary>
        /// <param name="str">string to test</param>
        /// <param name="paramName">Parameter Name to test</param>
        public static void NotEmpty(string str, string paramName)
        {
            NotEmpty(str, paramName, null);
        }

        /// <summary>Not Empty Test - String cannot be empty</summary>
        /// <param name="str">string to test</param>
        /// <param name="paramName">Parameter Name to test</param>
        /// <param name="message">Message to raise as exception if string is empty</param>
        public static void NotEmpty(string str, string paramName, string message)
        {
            if (str == null || str.Trim().Length <= 0)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Not Empty Test - collection cannot be empty
        /// </summary>
        /// <param name="col"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        public static void NotEmpty(ICollection col, string paramName, string message)
        {
            if (col == null || col.Count == 0)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        #endregion NotEmpty

        #region boolean

        /// <summary>Not True Test - boolean value must be false</summary>
        /// <param name="condition">Boolean value to test</param>
        /// <param name="conditionName">name of Boolean value to test</param>
        public static void NotTrue(bool condition, string conditionName)
        {
            True(!condition, conditionName);
        }

        /// <summary>Not True Test - boolean value must be false</summary>
        /// <param name="condition">Boolean value to test</param>
        /// <param name="conditionName">name of Boolean value to test</param>
        /// <param name="message">Message to raise as exception if condition fails</param>
        public static void NotTrue(bool condition, string conditionName, string message)
        {
            True(!condition, conditionName, message);
        }

        /// <summary>True Test - boolean value must be true</summary>
        /// <param name="condition">Boolean value to test</param>
        /// <param name="conditionName">name of Boolean value to test</param>
        public static void True(bool condition, string conditionName)
        {
            True(condition, conditionName, null);
        }

        /// <summary>True Test - boolean value must be true</summary>
        /// <param name="condition">Boolean value to test</param>
        /// <param name="conditionName">name of Boolean value to test</param>
        /// <param name="message">Message to raise as exception if condition fails</param>
        public static void True(bool condition, string conditionName, string message)
        {
            if (!condition)
            {
                throw new ApplicationException(string.Format("The condition, {0}, is not true. {1}", conditionName, message));
            }
        }

        #endregion boolean

        internal static void ValidateQueryString(string queryString)
        {
            string querystringFormat = @"^(.*?(\?(.*?))?)?$";
            Regex regex = new Regex(querystringFormat);
            if (!regex.IsMatch(queryString))
                throw new ApplicationException(string.Format("The querystring {0} specified is not valid", queryString));
        }
    }
}