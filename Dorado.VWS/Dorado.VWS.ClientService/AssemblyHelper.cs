/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：Assembly帮助类
 *  -------------------------------------------------------------------------*/

#region using

using System.Diagnostics;
using System.IO;
using System.Reflection;
using Dorado.Core;
using Dorado.Core.Logger;

#endregion using

namespace Dorado.VWS.ClientHost
{
    internal class AssemblyHelper
    {
        /// <summary>
        ///     获取产品版本号
        /// </summary>
        /// <returns>版本号</returns>
        internal static string GetProductVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductVersion;
        }

        /// <summary>
        ///     从Assembly中提取文件并保存
        /// </summary>
        /// <param name = "resFileName">要提取的文件</param>
        /// <param name = "targetPath">目标文件</param>
        internal static bool GetFileFromAssembly(string resFileName, string targetPath)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resName = string.Format("{0}.{1}", typeof(AssemblyHelper).Namespace, resFileName);
            using (
                Stream stream =
                    assembly.GetManifestResourceStream(resName))
            {
                StreamReader sr;
                if (stream != null)
                {
                    sr = new StreamReader(stream);
                }
                else
                {
                    //Assembly中不存在，则返回false
                    LoggerWrapper.Logger.Error("VWS.ClientHost", string.Format("Can not get resources stream by \"{0}\" from assambly {{{1}}}",
                                                resName, assembly));
                    return false;
                }

                string formatStr = sr.ReadToEnd();
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }

                using (var sw = new StreamWriter(targetPath))
                {
                    sw.WriteLine(formatStr);
                }
            }

            return true;
        }

        /// <summary>
        ///     获取当前程序域中的文件绝对路径
        /// </summary>
        /// <param name = "filename">文件名</param>
        /// <returns>绝对路径</returns>
        internal static string GetFileLocation(string filename)
        {
            return Assembly.GetExecutingAssembly().Location.Replace(ClientConst.ExeName, filename);
        }
    }
}