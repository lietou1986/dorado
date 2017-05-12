/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 时间： 2012/1/4 16:21:32
 * 作者：
 * 版本            时间                  作者                 描述
 * v 1.0    2012/1/4 16:21:32               创建
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System;
using System.Text.RegularExpressions;
using Dorado.Core;
using Dorado.Core.Logger;

namespace Dorado.VWS.Services
{
    public class SecurityExt
    {
        public static int RemoveAllCache()
        {
            try
            {
                Regex reg = new Regex("allowIPs_.*|allowservice_.*", RegexOptions.IgnoreCase);
                return WebCache.ClearAll(reg);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS.Site", ex.ToString());
            }
            return 0;
        }
    }
}