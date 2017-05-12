/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/3 10:54:15
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：文件压缩相关类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Dorado.Core;
using Dorado.Core.Logger;

#endregion using

namespace Dorado.VWS.ClientHost
{
    internal class FileCompress
    {
        /// <summary>
        ///     文件监听
        /// </summary>
        private readonly FileSystemWatcher _fsWatcher = new FileSystemWatcher();

        private readonly object _lockObj = new object();
        private readonly Dictionary<string, DateTime> _tmpCompressFiles = new Dictionary<string, DateTime>();

        /// <summary>
        ///     压缩插件路径
        /// </summary>
        private readonly string _yuiCompressPath = AssemblyHelper.GetFileLocation(ClientConst.YuiCompressorName);

        /// <summary>
        ///     文件压缩
        /// </summary>
        /// <param name = "path">压缩文件路径</param>
        /// <param name = "filter">筛选类型</param>
        internal FileCompress(string path, string filter)
        {
            _fsWatcher.Path = path;
            _fsWatcher.Filter = filter;
            _fsWatcher.NotifyFilter = NotifyFilters.Size;
            _fsWatcher.IncludeSubdirectories = true;

            _fsWatcher.Changed += CompressFiles;
            _fsWatcher.Created += CompressFiles;
            _fsWatcher.EnableRaisingEvents = false;
        }

        /// <summary>
        ///     开始压缩
        /// </summary>
        internal void Start()
        {
            _fsWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        ///     停止压缩
        /// </summary>
        internal void Stop()
        {
            _fsWatcher.EnableRaisingEvents = false;
        }

        /// <summary>
        ///     文件压缩逻辑处理
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void CompressFiles(object sender, FileSystemEventArgs e)
        {
            LoggerWrapper.Logger.Error("VWS.ClientHost", e.FullPath + " Type:" + e.ChangeType + "  Event: Created");
            lock (_lockObj)
            {
                if (_tmpCompressFiles.ContainsKey(e.FullPath) &&
                    (DateTime.Now - _tmpCompressFiles[e.FullPath]).TotalSeconds < 10)
                {
                    // 当文件十秒种内被压缩过，则本次不处理
                    return;
                }

                if (_tmpCompressFiles.ContainsKey(e.FullPath))
                {
                    _tmpCompressFiles.Remove(e.FullPath);
                }

                try
                {
                    string filelower = e.FullPath.ToLower();
                    if (ClientConst.HtmlPostfixs.Contains(_fsWatcher.Filter))
                    {
                        Thread.Sleep(500);
                        //读取文件编码
                        string fileContent = File.ReadAllText(e.FullPath);
                        var reg = new Regex("<meta.+?charset=(.+?)\"");
                        Match match = reg.Match(fileContent);
                        Encoding coding = Encoding.GetEncoding("utf-8");
                        if (match.Success)
                        {
                            string charset = match.Groups[1].Value;
                            coding = Encoding.GetEncoding(charset);
                            fileContent = File.ReadAllText(e.FullPath, coding);
                        }

                        File.WriteAllText(e.FullPath + ClientConst.CompressTempPostfix,
                                          HtmlCompress(fileContent).ToString(), coding);
                        File.Copy(e.FullPath + ClientConst.CompressTempPostfix, e.FullPath, true);
                        File.Delete(e.FullPath + ClientConst.CompressTempPostfix);
                        //_logger.InfoFormat("Html Compress : {0}", e.FullPath);
                        LoggerWrapper.Logger.Error("VWS.ClientHost", string.Format("Html Compress : {0}", e.FullPath));
                        _tmpCompressFiles.Add(e.FullPath, DateTime.Now);
                    }
                    else if (ClientConst.JsCssPostfixs.Contains(_fsWatcher.Filter) && !filelower.EndsWith("-min.js") &&
                             !filelower.EndsWith("-min.css"))
                    {
                        Thread.Sleep(500);
                        CommandHelper.RunShellCommand("java",
                                                      string.Format(" -jar {0} {1} -o {2} --charset utf-8",
                                                                    _yuiCompressPath,
                                                                    e.FullPath,
                                                                    e.FullPath + ClientConst.CompressTempPostfix));
                        File.Copy(e.FullPath + ClientConst.CompressTempPostfix, e.FullPath, true);
                        File.Delete(e.FullPath + ClientConst.CompressTempPostfix);
                        //_logger.InfoFormat("JsCss Compress : {0}", e.FullPath);
                        LoggerWrapper.Logger.Error("VWS.ClientHost", string.Format("JsCss Compress : {0}", e.FullPath));
                        _tmpCompressFiles.Add(e.FullPath, DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("VWS.ClientHost", ex.ToString());
                    //_logger.Error(ex);
                }
            }
        }

        /// <summary>
        ///     Html格式压缩
        /// </summary>
        /// <param name = "text">输入</param>
        /// <returns>输出</returns>
        private StringBuilder HtmlCompress(string text)
        {
            var str = new StringBuilder();
            var s = new List<char> { '\f', '\n', '\r', '\t', '\v' };
            Func<int, object> P = c => null;
            Func<int, object> Ptag = c => null; //标签处理机
            Func<int, object> Pcomment = c => null; //注释

            #region - 总处理机 -

            Func<int, object> state = P = i =>
                                              {
                                                  char c = text[i];
                                                  if (c == '<') //碰到<交个Ptag处理机
                                                  {
                                                      // 注释掉“注释处理机”
                                                      //if (i + 4 < text.Length)
                                                      //{
                                                      //    if (text.Substring(i + 1, 3) == "!--") //交个注释处理机
                                                      //    {
                                                      //        return Pcomment;
                                                      //    }
                                                      //}
                                                      str.Append(c);
                                                      return Ptag;
                                                  }
                                                  if (s.Contains(c))
                                                  {
                                                      return P;
                                                  }
                                                  if (c == ' ')
                                                  {
                                                      if (i + 1 < text.Length)
                                                      {
                                                          if (
                                                              new List<char> { ' ', '<', '\f', '\n', '\r', '\t', '\v' }.
                                                                  Contains(text[i + 1]) == false)
                                                          {
                                                              str.Append(c);
                                                          }
                                                      }
                                                      return P;
                                                  }
                                                  str.Append(c);
                                                  return P;
                                              };

            #endregion - 总处理机 -

            #region - Tag处理机 -

            Ptag = i =>
                       {
                           char c = text[i];
                           if (c == '>') //交还给p
                           {
                               str.Append(c);
                               return P;
                           }
                           //else if (s.Contains(c) == true) { return Ptag; }
                           if (s.Contains(c) && c != '\r' && c != '\n')
                           {
                               return Ptag;
                           }
                           if (c == ' ')
                           {
                               if (i + 1 < text.Length)
                               {
                                   if (new List<char> { ' ', '/', '=', '>' }.Contains(text[i + 1]) == false)
                                   {
                                       str.Append(c);
                                   }
                               }
                               return Ptag;
                           }
                           str.Append(c);
                           return Ptag;
                       };

            #endregion - Tag处理机 -

            #region - 注释处理机 -

            Pcomment = i =>
                           {
                               char c = text[i];
                               if (c == '>' && text.Substring(i - 2, 3) == "-->")
                               {
                                   return P;
                               }
                               return Pcomment;
                           };

            #endregion - 注释处理机 -

            for (int index = 0; index < text.Length; index++)
            {
                state = (Func<int, object>)state(index);
            }

            return str;
        }
    }
}