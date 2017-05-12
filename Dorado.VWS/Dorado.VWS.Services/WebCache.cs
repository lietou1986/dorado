/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/11/10 10:50:33
 * 版本号：v1.0
 * 本类主要用途描述：Web缓存
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;

namespace Dorado.VWS.Services
{
    public class WebCache
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="cacheKey">键值</param>
        /// <param name="data">数据</param>
        /// <param name="timeOut">最后一次访问后失效时间,单位：分</param>
        public static void Add(string cacheKey, object data, int timeOut)
        {
            HttpContext.Current.Cache.Insert(cacheKey, data, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(timeOut));
        }

        /// <summary>
        /// 删除缓存数据
        /// </summary>
        /// <param name="cacheKey">键值</param>
        public static void Remove(string cacheKey)
        {
            HttpContext.Current.Cache.Remove(cacheKey);
        }

        /// <summary>
        /// 删除所有缓存数据 add by heyongdong
        /// </summary>
        public static int ClearAll()
        {
            int i = 0;
            IDictionaryEnumerator CacheEnum = HttpContext.Current.Cache.GetEnumerator();

            while (CacheEnum.MoveNext())
            {
                i++;
                HttpContext.Current.Cache.Remove(CacheEnum.Key.ToString());
            }
            return i;
        }

        /// <summary>
        /// 删除所有缓存数据 add by heyongdong
        /// </summary>
        public static int ClearAll(Regex regex)
        {
            int i = 0;
            IDictionaryEnumerator CacheEnum = HttpContext.Current.Cache.GetEnumerator();

            while (CacheEnum.MoveNext())
            {
                if (regex.IsMatch(CacheEnum.Key.ToString()))
                {
                    i++;
                    HttpContext.Current.Cache.Remove(CacheEnum.Key.ToString());
                }
            }
            return i;
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="cacheKey">键值</param>
        /// <returns>找到则返回 object数据,否则为null </returns>
        public static object Get(string cacheKey)
        {
            return HttpContext.Current.Cache.Get(cacheKey);
        }
    }
}