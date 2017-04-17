using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Dorado.Services
{
    public static class RequestServiceExtension
    {
        private static string GetResponseBody(HttpWebResponse response, Encoding encoding)
        {
            string responseBody;
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
            {
                using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            return responseBody;
        }

        public static List<Cookie> GetAllCookies(this RequestService requestService, CookieContainer cc)
        {
            if (cc == null)
            {
                return new List<Cookie>();
            }

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            return (from object pathList in table.Values select (SortedList)pathList.GetType().InvokeMember("m_list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { }) into lstCookieCol from CookieCollection colCookies in lstCookieCol.Values from Cookie c in colCookies select c).ToList();
        }

        public static CookieContainer GetCookie(this RequestService requestService, int timeout = 5)
        {
            try
            {
                requestService.JoinParameter();

                CookieContainer cookieContainer = new CookieContainer();
                HttpWebResponse response = null;
                var req = (HttpWebRequest)WebRequest.Create(requestService.UrlBuffer.ToString());
                req.UserAgent = Environment.OSVersion.ToString();
                req.Method = requestService.HttpMethod == HttpMethod.Get ? "GET" : "POST";
                req.CookieContainer = cookieContainer;
                if (req.Method == "POST")
                {
                    //添加POST的参数
                    var args =
                        requestService.PostParameters.Keys.Select(
                            key => string.Format("{0}={1}", key, requestService.PostParameters[key].UrlEncode()));
                    var pArgs = string.Join("&", args.ToArray());
                    var b = Encoding.UTF8.GetBytes(pArgs);
                    req.ContentType = MimeType.WebForm;
                    req.ContentLength = b.Length;
                    using (var reqs = req.GetRequestStream())
                    {
                        reqs.Write(b, 0, b.Length);
                        reqs.Close();
                    }
                }
                var e = new AutoResetEvent(false);
                var httpResult = string.Empty;
                string error = null;
                req.BeginGetResponse(delegate (IAsyncResult ar)
                {
                    try
                    {
                        response = (HttpWebResponse)req.EndGetResponse(ar);

                        httpResult = GetResponseBody(response, requestService.Encoder);
                    }
                    catch (Exception ex)
                    {
                        error = string.Format("request url:[{0}] error", requestService.UrlBuffer);
                        LoggerWrapper.Logger.Error(error, ex);
                    }
                    finally
                    {
                        req.Abort();
                        if (response != null) response.Close();
                        e.Set();
                    }
                }, null);
                e.WaitOne(TimeSpan.FromSeconds(timeout));

                if (error != null) throw new CoreException(error);

                if (string.IsNullOrEmpty(httpResult))
                    throw new CoreException(string.Format("request url:[{0}] timeout", requestService.UrlBuffer));

                response.Cookies = req.CookieContainer.GetCookies(req.RequestUri);

                return cookieContainer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static OperateResult<T> Request<T>(this RequestService requestService, int timeout = 5)
        {
            try
            {
                requestService.JoinParameter();

                HttpWebResponse response = null;
                var req = (HttpWebRequest)WebRequest.Create(requestService.UrlBuffer.ToString());
                req.UserAgent = Environment.OSVersion.ToString();
                if (requestService.CookieContainer != null) req.CookieContainer = requestService.CookieContainer;
                req.Method = requestService.HttpMethod == HttpMethod.Get ? "GET" : "POST";
                if (req.Method == "POST")
                {
                    //添加POST的参数
                    var args =
                        requestService.PostParameters.Keys.Select(
                            key => string.Format("{0}={1}", key, requestService.PostParameters[key].UrlEncode()));
                    var pArgs = string.Join("&", args.ToArray());
                    var b = Encoding.UTF8.GetBytes(pArgs);
                    req.ContentType = MimeType.WebForm;
                    req.ContentLength = b.Length;
                    using (var reqs = req.GetRequestStream())
                    {
                        reqs.Write(b, 0, b.Length);
                        reqs.Close();
                    }
                }
                var e = new AutoResetEvent(false);
                var httpResult = string.Empty;
                string error = null;
                req.BeginGetResponse(delegate (IAsyncResult ar)
                {
                    try
                    {
                        response = (HttpWebResponse)req.EndGetResponse(ar);

                        httpResult = GetResponseBody(response, requestService.Encoder);
                    }
                    catch (Exception ex)
                    {
                        error = string.Format("request url:[{0}] error", requestService.UrlBuffer);
                        LoggerWrapper.Logger.Error(error, ex);
                    }
                    finally
                    {
                        req.Abort();
                        if (response != null) response.Close();
                        e.Set();
                    }
                }, null);
                e.WaitOne(TimeSpan.FromSeconds(timeout));

                if (error != null) throw new CoreException(error);

                if (string.IsNullOrEmpty(httpResult))
                    throw new CoreException(string.Format("request url:[{0}] timeout", requestService.UrlBuffer));

                var result = requestService.Convert<T>(httpResult);

                return new OperateResult<T>(OperateStatus.Success, "", result);
            }
            catch (Exception ex)
            {
                return new OperateResult<T>(OperateStatus.Failure, ex.Message);
            }
        }

        public static OperateResult<T> Request<T, M>(this RequestService requestService, M jsonBody, int timeout = 5) where M : class
        {
            return Request<T>(requestService, MimeType.Json, requestService.JsonSerializer.Serialize(jsonBody), timeout);
        }

        public static OperateResult<T> Request<T>(this RequestService requestService, string contentType, string data, int timeout = 5)
        {
            try
            {
                requestService.JoinParameter();

                HttpWebResponse response = null;
                var req = (HttpWebRequest)WebRequest.Create(requestService.UrlBuffer.ToString());
                req.UserAgent = Environment.OSVersion.ToString();
                req.Method = "POST";
                if (requestService.CookieContainer != null) req.CookieContainer = requestService.CookieContainer;
                var b = Encoding.UTF8.GetBytes(data);
                req.ContentType = contentType;
                req.ContentLength = b.Length;
                using (var reqs = req.GetRequestStream())
                {
                    reqs.Write(b, 0, b.Length);
                    reqs.Close();
                }
                var e = new AutoResetEvent(false);
                var httpResult = string.Empty;
                string error = null;
                req.BeginGetResponse(delegate (IAsyncResult ar)
                {
                    try
                    {
                        response = (HttpWebResponse)req.EndGetResponse(ar);
                        httpResult = GetResponseBody(response, requestService.Encoder);
                    }
                    catch (Exception ex)
                    {
                        error = string.Format("request api:[{0}] error", requestService.UrlBuffer);
                        LoggerWrapper.Logger.Error(error, ex);
                    }
                    finally
                    {
                        req.Abort();
                        if (response != null) response.Close();
                        e.Set();
                    }
                }, null);
                e.WaitOne(TimeSpan.FromSeconds(timeout));

                if (error != null) throw new CoreException(error);

                if (string.IsNullOrEmpty(httpResult))
                    throw new CoreException(string.Format("request api:[{0}] timeout", requestService.UrlBuffer));

                var result = requestService.Convert<T>(httpResult);

                return new OperateResult<T>(OperateStatus.Success, "", result);
            }
            catch (Exception ex)
            {
                return new OperateResult<T>(OperateStatus.Failure, ex.Message);
            }
        }
    }
}