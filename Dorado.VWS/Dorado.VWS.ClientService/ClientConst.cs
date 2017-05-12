/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：客户端常量类
 *  -------------------------------------------------------------------------*/

#region using

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

#endregion using

namespace Dorado.VWS.ClientHost
{
    internal class ClientConst
    {
        /// <summary>
        ///     当前进程名
        /// </summary>
        internal const string ExeName = "Dorado.VWS.ClientHost.exe";

        /// <summary>
        ///     YUI插件名
        /// </summary>
        internal const string YuiCompressorName = "yuicompressor-2.4.6.jar";

        /// <summary>
        ///     更新文件名
        /// </summary>
        internal const string UpdateName = "update.bat";

        /// <summary>
        ///     压缩时临时文件后缀
        /// </summary>
        internal const string CompressTempPostfix = ".vws-temp";

        /// <summary>
        ///     任务记录文件名
        /// </summary>
        internal const string VwsTaskFile = "vwstask.txt";

        /// <summary>
        ///     临时备份文件名
        /// </summary>
        internal const string VwsBackupFile = "vwsbk.txt";

        /// <summary>
        ///     文件列表分隔符
        /// </summary>
        internal const string FileSeparator = ",";

        /// <summary>
        ///     客户端IP
        /// </summary>
        internal static readonly string LocalIP;

        /// <summary>
        ///     客户端机器名
        /// </summary>
        internal static readonly string HostName;

        /// <summary>
        ///     客户端版本号
        /// </summary>
        internal static readonly string ClientVersion;

        /// <summary>
        ///     Html文件后缀
        /// </summary>
        internal static readonly List<string> HtmlPostfixs = new List<string> { "*.html", "*.htm", "*.shtml", "*.aspx" };

        /// <summary>
        ///     JS,CSS文件后缀
        /// </summary>
        internal static readonly List<string> JsCssPostfixs = new List<string> { "*.css", "*.js" };

        /// <summary>
        ///     配制路径前两部分的正则表达式
        /// </summary>
        internal static readonly Regex TwoPrefixRegex = new Regex(@"^\w+\\\w+\\");

        static ClientConst()
        {
            //设置IP
            string hostname = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostname);

            foreach (IPAddress item in ipEntry.AddressList)
            {
                //if (!item.IsIPv6LinkLocal)
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (string.IsNullOrEmpty(LocalIP) || string.IsNullOrWhiteSpace(LocalIP))
                        LocalIP = item.ToString().Trim();
                    else
                        LocalIP += "|" + item.ToString().Trim();
                }
            }

            //设置hostname
            HostName = ipEntry.HostName;

            //设置ClientVersion
            ClientVersion = AssemblyHelper.GetProductVersion();
        }
    }
}