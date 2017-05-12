/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/6 9:50:33
 * 版本号：v1.0
 * 本类主要用途描述：清除缓存类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Dorado.Core;
using Dorado.Core.Logger;

#endregion using

namespace Dorado.VWS.Services
{
    public class ClearCache
    {
        #region 推送图片清除缓存

        public static void Clear(List<string> clearCacheData)
        {
            HttpWebRequest httpRequest = null;
            Stream requestStream = null;
            HttpWebResponse httpRespone = null;

            try
            {
                //设置并发请求数
                System.Net.ServicePointManager.DefaultConnectionLimit = 20;

                var clearCacheDataUrl = new StringBuilder();
                for (int i = 0; i < clearCacheData.Count; i++)
                {
                    string tmp = clearCacheData[i].Replace("\\", "/");
                    if (i == 0)
                    {
                        clearCacheDataUrl.AppendLine("url=" + tmp);
                    }
                    else
                    {
                        clearCacheDataUrl.AppendLine(tmp);
                    }
                }
                Encoding encoding = Encoding.UTF8;
                byte[] bytesToPost = encoding.GetBytes(clearCacheDataUrl.ToString());

                LoggerWrapper.Logger.Info("VWS.Site", string.Format("将清理这些URL的缓存：" + clearCacheDataUrl.ToString()));

                #region 创建HttpWebRequest对象

                httpRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["imgCache"]);
                //Logger.Debug("清理缓存：已经创建WebRequest请求");
                LoggerWrapper.Logger.Info("VWS.Site", "清理缓存：已经创建WebRequest请求");

                #endregion 创建HttpWebRequest对象

                #region 初始化HtppWebRequest对象

                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.Method = "POST";
                LoggerWrapper.Logger.Info("VWS.Site", "清理缓存：已经设置代理");

                #endregion 初始化HtppWebRequest对象

                #region 附加Post给服务器的数据到HttpWebRequest对象

                httpRequest.ContentLength = bytesToPost.Length;
                requestStream = httpRequest.GetRequestStream();
                requestStream.Write(bytesToPost, 0, bytesToPost.Length);
                requestStream.Close();
                requestStream = null;
                LoggerWrapper.Logger.Info("VWS.Site", "清理缓存：已经POST数据");

                #endregion 附加Post给服务器的数据到HttpWebRequest对象

                httpRespone = (HttpWebResponse)httpRequest.GetResponse();
                LoggerWrapper.Logger.Info("VWS.Site", string.Format("清理缓存成功:{0}", clearCacheDataUrl.ToString()));
                httpRespone.Close();
                httpRespone = null;
                httpRequest = null;
                LoggerWrapper.Logger.Info("VWS.Site", "清理缓存：已经关闭响应");
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS.Site", "清理缓存失败！\n" + ex.ToString());
            }
            finally
            {
                try
                {
                    if (requestStream != null)
                    {
                        requestStream.Close();
                    }
                }
                catch
                {
                }
                try
                {
                    if (httpRespone != null)
                    {
                        httpRespone.Close();
                    }
                }
                catch
                {
                }
            }
        }

        #endregion 推送图片清除缓存
    }
}