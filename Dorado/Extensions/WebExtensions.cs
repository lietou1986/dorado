using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Dorado.Extensions
{
    public static class WebExtensions
    {
        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ChangeType<T>(this object value)
        {
            return (T)ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object ChangeType(this object value, Type type)
        {
            try
            {
                if (value == null && type.IsGenericType)
                    return Activator.CreateInstance(type);
                if (value == null)
                    return default(Type);
                if (type == value.GetType())
                    return value;
                if (type.IsEnum)
                {
                    if (value is string)
                        return Enum.Parse(type, value as string);
                    return Enum.ToObject(type, value);
                }
                if (type == typeof(Boolean))
                {
                    return value.Bool();
                }
                if (type == typeof(DateTime))
                {
                    return value.Date();
                }
                if (!type.IsInterface && type.IsGenericType)
                {
                    Type innerType = type.GetGenericArguments()[0];
                    object innerValue = value.ChangeType(innerType);
                    return Activator.CreateInstance(type, new[] { innerValue });
                }
                if (value is string && type == typeof(Guid))
                    return new Guid(value as string);
                if (value is string && type == typeof(Version))
                    return new Version(value as string);
                if (!(value is IConvertible))
                    return value;
                return Convert.ChangeType(value, type);
            }
            catch (Exception)
            {
                return default(Type);
            }
        }

        public static T GetPara<T>(this NameValueCollection nvc, string paraName)
        {
            return nvc.IsHas(paraName) ? ChangeType<T>(nvc[paraName]) : default(T);
        }

        public static string[] GetParaValues(this NameValueCollection nvc, string paraName)
        {
            return nvc.IsHas(paraName) ? nvc.GetValues(paraName) : new string[] { };
        }

        public static bool IsHas(this NameValueCollection nvc, string paraName)
        {
            return nvc.AllKeys.Contains(paraName, new NvcCompare<string>());
        }

        public static bool IsHas(this HttpFileCollection nvc, string paraName)
        {
            return nvc.AllKeys.Contains(paraName, new NvcCompare<string>());
        }

        /// <summary>
        /// url解码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UrlDecode(this string input)
        {
            return string.IsNullOrEmpty(input) ? string.Empty : HttpUtility.UrlDecode(input);
        }

        /// <summary>
        /// 取得页面get数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="paraName">Name of the para.</param>
        /// <returns></returns>
        public static T Get<T>(this HttpContext context, string paraName)
        {
            NameValueCollection nvc = context.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : default(T);
        }

        public static string[] Get(this HttpContext context, string paraName)
        {
            NameValueCollection nvc = context.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc.GetValues(paraName) : new string[] { };
        }

        /// <summary>
        /// 取得页面post数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="paraName">Name of the para.</param>
        /// <returns></returns>
        public static T Post<T>(this HttpContext context, string paraName)
        {
            NameValueCollection nvc = context.Request.Form;

            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : default(T);
        }

        public static string[] Post(this HttpContext context, string paraName)
        {
            NameValueCollection nvc = context.Request.Form;
            return nvc.IsHas(paraName) ? nvc.GetValues(paraName) : new string[] { };
        }

        /// <summary>
        /// 取得页面get数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="paraName">Name of the para.</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Get<T>(this HttpContext context, string paraName, T defaultValue)
        {
            NameValueCollection nvc = context.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : defaultValue;
        }

        /// <summary>
        /// 取得页面post数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="paraName">Name of the para.</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Post<T>(this HttpContext context, string paraName, T defaultValue)
        {
            NameValueCollection nvc = context.Request.Form;

            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : defaultValue;
        }

        /// <summary>
        /// 取得页面get数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="paraName">Name of the para.</param>
        /// <returns></returns>
        public static T Get<T>(this HttpContextBase context, string paraName)
        {
            NameValueCollection nvc = context.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : default(T);
        }

        /// <summary>
        /// 取得页面get数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="paraName">Name of the para.</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Get<T>(this HttpContextBase context, string paraName, T defaultValue)
        {
            NameValueCollection nvc = context.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : defaultValue;
        }

        public static string[] Get(this HttpContextBase context, string paraName)
        {
            NameValueCollection nvc = context.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc.GetValues(paraName) : new string[] { };
        }

        /// <summary>
        /// 取得页面post数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="paraName">Name of the para.</param>
        /// <returns></returns>
        public static T Post<T>(this HttpContextBase context, string paraName)
        {
            NameValueCollection nvc = context.Request.Form;

            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : default(T);
        }

        /// <summary>
        /// 取得页面post数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="paraName">Name of the para.</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Post<T>(this HttpContextBase context, string paraName, T defaultValue)
        {
            NameValueCollection nvc = context.Request.Form;

            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : defaultValue;
        }

        public static string[] Post(this HttpContextBase context, string paraName)
        {
            NameValueCollection nvc = context.Request.Form;
            return nvc.IsHas(paraName) ? nvc.GetValues(paraName) : new string[] { };
        }

        /// <summary>
        /// 取得页面get数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">The page.</param>
        /// <param name="paraName">Name of the para.</param>
        /// <returns></returns>
        public static T Get<T>(this Page page, string paraName)
        {
            NameValueCollection nvc = page.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : default(T);
        }

        /// <summary>
        /// 取得页面get数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">The page.</param>
        /// <param name="paraName">Name of the para.</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Get<T>(this Page page, string paraName, T defaultValue)
        {
            NameValueCollection nvc = page.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : defaultValue;
        }

        public static string[] Get(this Page page, string paraName)
        {
            NameValueCollection nvc = page.Request.QueryString;
            return nvc.IsHas(paraName) ? nvc.GetValues(paraName) : new string[] { };
        }

        /// <summary>
        /// 取得页面post数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">The page.</param>
        /// <param name="paraName">Name of the para.</param>
        /// <returns></returns>
        public static T Post<T>(this Page page, string paraName)
        {
            NameValueCollection nvc = page.Request.Form;

            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : default(T);
        }

        /// <summary>
        /// 取得页面post数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">The page.</param>
        /// <param name="paraName">Name of the para.</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Post<T>(this Page page, string paraName, T defaultValue)
        {
            NameValueCollection nvc = page.Request.Form;

            return nvc.IsHas(paraName) ? nvc[paraName].ChangeType<T>() : defaultValue;
        }

        public static string[] Post(this Page page, string paraName)
        {
            NameValueCollection nvc = page.Request.Form;
            return nvc.IsHas(paraName) ? nvc.GetValues(paraName) : new string[] { };
        }

        public static void GetPara(object para)
        {
            GetPara(para, HttpContext.Current.Request.QueryString);
        }

        public static void GetPara(object para, NameValueCollection list)
        {
            FieldInfo[] info = para.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo fld in info)
            {
                string tmp = list[(fld.Name == "page" || fld.Name == "key" ? fld.Name : fld.Name.Substring(0, 1))];
                if (tmp == null)
                {
                    if (fld.Name == "page")
                        fld.SetValue(para, 1);
                    else if (fld.FieldType == typeof(string))
                        fld.SetValue(para, string.Empty);
                }
                else
                {
                    switch (Type.GetTypeCode(fld.FieldType))
                    {
                        case TypeCode.Boolean:
                            fld.SetValue(para, DataTypeExtensions.ToBool(tmp));
                            break;

                        case TypeCode.String:
                            fld.SetValue(para, tmp.Trim());
                            break;

                        case TypeCode.Char:
                            fld.SetValue(para, DataTypeExtensions.ToChar(tmp));
                            break;

                        case TypeCode.Byte:
                            fld.SetValue(para, DataTypeExtensions.ToByte(tmp));
                            break;

                        case TypeCode.SByte:
                            fld.SetValue(para, DataTypeExtensions.ToSByte(tmp));
                            break;

                        case TypeCode.Int16:
                            fld.SetValue(para, DataTypeExtensions.ToShort(tmp));
                            break;

                        case TypeCode.UInt16:
                            fld.SetValue(para, DataTypeExtensions.ToUShort(tmp));
                            break;

                        case TypeCode.Int32:
                            fld.SetValue(para, DataTypeExtensions.ToInt(tmp));
                            break;

                        case TypeCode.UInt32:
                            fld.SetValue(para, DataTypeExtensions.ToUInt(tmp));
                            break;

                        case TypeCode.Int64:
                            fld.SetValue(para, DataTypeExtensions.ToLong(tmp));
                            break;

                        case TypeCode.UInt64:
                            fld.SetValue(para, DataTypeExtensions.ToULong(tmp));
                            break;

                        case TypeCode.Decimal:
                            fld.SetValue(para, DataTypeExtensions.ToDecimal(tmp));
                            break;

                        case TypeCode.Double:
                            fld.SetValue(para, DataTypeExtensions.ToDouble(tmp));
                            break;

                        case TypeCode.Single:
                            fld.SetValue(para, DataTypeExtensions.ToFloat(tmp));
                            break;

                        case TypeCode.DateTime:
                            fld.SetValue(para, DataTypeExtensions.ToDateTime(tmp));
                            break;

                        default:
                            fld.SetValue(para, tmp);
                            break;
                    }
                }
            }
        }

        public static string UrlHead()
        {
            string[] list = HttpContext.Current.Request.Url.Segments;
            string url = list[list.Length - 1];
            int pos = url.IndexOf('.');
            if (pos > -1) url = url.Substring(0, pos);
            string tmp = string.Empty;
            for (int i = 0; i < list.Length - 1; i++)
            {
                tmp = list[i] + tmp;
            }
            return tmp + url;
        }

        public static string UrlTail()
        {
            string url = HttpContext.Current.Request.Url.ToString();
            int pos = url.IndexOf('-');
            if (pos > -1)
            {
                return url.Substring(pos);
            }
            return ".htm";
        }

        public static string UrlTail(object para)
        {
            return UrlTail(para, null);
        }

        public static string UrlTail(object para, string key)
        {
            string Tail = ".htm";
            if (para == null) return Tail;
            Type type = para.GetType();
            FieldInfo[] info = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            StringBuilder sb = new StringBuilder();
            foreach (FieldInfo fld in info)
            {
                if (fld.Name == key) continue;
                object tmp = fld.GetValue(para);
                switch (Type.GetTypeCode(fld.FieldType))
                {
                    case TypeCode.String:
                        if (tmp != null && !tmp.Equals(string.Empty))
                        {
                            if (fld.Name == "key")
                                Tail += "?" + HttpContext.Current.Server.UrlEncode(tmp.ToString());
                            else
                                sb.Append("-" + fld.Name.Substring(0, 1) + HttpContext.Current.Server.UrlEncode(tmp.ToString()).Replace("+", "%2B").Replace("-", "%2D"));
                        }
                        break;

                    case TypeCode.Boolean:
                        if (tmp.Equals(true))
                            sb.Append("-" + fld.Name.Substring(0, 1) + "1");
                        break;

                    default:
                        if (fld.Name == "page")
                        {
                            if (DataTypeExtensions.ToInt(tmp) > 1) sb.Append("-" + tmp.ToString());
                        }
                        else if (!tmp.ToString().Equals("0"))
                        {
                            sb.Append("-" + fld.Name.Substring(0, 1) + tmp.ToString());
                        }
                        break;
                }
            }
            return sb.ToString() + Tail;
        }

        public static string UrlQuery(object para, string key)
        {
            if (para == null) return String.Empty;
            Type type = para.GetType();
            FieldInfo[] info = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            StringBuilder sb = new StringBuilder();
            foreach (FieldInfo fld in info)
            {
                if (fld.Name == key) continue;
                object tmp = fld.GetValue(para);
                switch (fld.FieldType.ToString())
                {
                    case "System.String":
                        if (tmp != null)
                        {
                            if (sb.Length > 0) sb.Append("&");
                            sb.Append(fld.Name + "=" + HttpContext.Current.Server.UrlEncode(tmp.ToString()));
                        }
                        break;

                    case "System.Int32":
                    case "System.Int16":
                    case "System.Int64":
                    case "System.Byte":
                        if (!tmp.Equals(0))
                        {
                            if (sb.Length > 0) sb.Append("&");
                            sb.Append(fld.Name + "=" + tmp.ToString());
                        }
                        break;

                    case "System.Float":
                    case "System.Double":
                        if (!tmp.Equals(0))
                        {
                            if (sb.Length > 0) sb.Append("&");
                            sb.Append(fld.Name + "=" + tmp.ToString());
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        public static string GetHttpInput(this HttpContextBase httpContext)
        {
            Stream input = httpContext.Request.InputStream;
            int inputLength = Convert.ToInt32(input.Length);

            byte[] bytes = new byte[inputLength];
            input.Read(bytes, 0, inputLength);

            return Encoding.UTF8.GetString(bytes);
        }

        public static string GetHttpInput(this HttpContext httpContext)
        {
            Stream input = httpContext.Request.InputStream;
            int inputLength = Convert.ToInt32(input.Length);

            byte[] bytes = new byte[inputLength];
            input.Read(bytes, 0, inputLength);

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 获取用户IP地址
        /// </summary>
        /// <returns></returns>
        public static string ClientIp
        {
            get
            {
                HttpRequest request = HttpContext.Current.Request;
                string clientIp = request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null
                                      ? request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                                      : request.ServerVariables["REMOTE_ADDR"] != null ? request.ServerVariables["REMOTE_ADDR"] : request.UserHostAddress;

                return clientIp.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }
        }
    }

    public class NvcCompare<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            if (x == null && y == null)
                return false;
            return x is String && y is String ? x.ToString().ToLower() == y.ToString().ToLower() : x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}