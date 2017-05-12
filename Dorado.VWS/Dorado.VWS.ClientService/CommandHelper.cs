/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 9:57:35
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：命令行执行帮助类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Diagnostics;

#endregion using

namespace Dorado.VWS.ClientHost
{
    internal class CommandHelper
    {
        /// <summary>
        ///     命令行执行文件
        /// </summary>
        /// <param name = "filePath">文件全路径</param>
        internal static void RunShellCommand(string filePath)
        {
            var process = new Process
                                  {
                                      StartInfo =
                                          {
                                              CreateNoWindow = true,
                                              WindowStyle = ProcessWindowStyle.Hidden,
                                              FileName = filePath,
                                              UseShellExecute = true
                                          }
                                  };

            process.Start();
        }

        /// <summary>
        ///     命令行执行
        /// </summary>
        /// <param name = "command">命令</param>
        /// <param name = "arguments">参数</param>
        internal static void RunShellCommand(String command, String arguments)
        {
            var process = new Process
                                  {
                                      StartInfo =
                                          {
                                              CreateNoWindow = true,
                                              WindowStyle = ProcessWindowStyle.Hidden,
                                              FileName = command,
                                              Arguments = arguments,
                                              UseShellExecute = false,
                                              RedirectStandardOutput = true,
                                              RedirectStandardError = true
                                          }
                                  };
            process.Start();

            process.WaitForExit();
        }
    }
}