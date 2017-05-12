#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Dorado.VWS.Model;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    ///     用户下载文件的Handler
    /// </summary>
    public class DownloadFile : IHttpHandler
    {
        private const string DownloadDir = "/tmp/";
        private AutoResetEvent _are;
        private bool _downloadSucess;

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            switch (context.Request.QueryString["action"])
            {
                case "remote":
                    {
                        string filePath = context.Request.QueryString["filePath"];
                        int domainID = Convert.ToInt32(context.Request.QueryString["domainID"]);
                        bool isHistory = context.Request.QueryString["isHistory"] == "true";

                        ServerEntity server = new ServerProvider().GetSourceServerByDomainId(domainID);

                        string targetRoot = server.Root.TrimEnd('\\') + (isHistory ? ".vws\\" : "\\");
                        var task = new TaskSenderEntity
                                       {
                                           TaskCmd = EnumTaskCmd.GETFILEBYTES,
                                           CustomCmd = EnumTaskCmd.GETFILEBYTES,
                                           TargetRoot = targetRoot,
                                           AddList = isHistory ? filePath : filePath.Remove(0, targetRoot.Length)
                                       };

                        var client = new SocketFileClient();
                        client.ReceivedFile += ReceivedFile;
                        client.FileFolder = context.Server.MapPath("~/tmp/");
                        client.RecieveFile(IPAddress.Parse(server.IP), task);
                        _are = new AutoResetEvent(false);
                        _are.WaitOne();
                        context.Response.Write(string.Format("{{\"ret\": {0}, \"filepath\":\"{1}\"}}",
                                                             _downloadSucess ? "true" : "false",
                                                             task.AddList.Replace("\\", "\\\\")));
                    }
                    break;

                case "local":
                    {
                        try
                        {
                            string fileName = context.Request.QueryString["fileName"];
                            string phyFile = context.Server.MapPath(DownloadDir + fileName);
                            var info = new FileInfo(phyFile);
                            if (!info.Exists)
                            {
                                context.Response.StatusCode = 404;
                                return;
                            }

                            fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                            if (context.Request.UserAgent.ToLower().Contains("msie"))
                                fileName = DownloadFileNameEncode.Encode(fileName);

                            context.Response.ContentType = "application/octet-stream";
                            context.Response.AddHeader("Cache-control", "no-cache");
                            context.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                            context.Response.TransmitFile(phyFile);
                        }
                        catch
                        {
                            context.Response.StatusCode = 500;
                        }
                    }
                    break;

                case "server":
                    {
                        string filePath = context.Request.QueryString["filePath"];
                        int serverID = Convert.ToInt32(context.Request.QueryString["domainID"]);
                        bool isHistory = context.Request.QueryString["isHistory"] == "true";

                        ServerEntity server = new ServerProvider().GetServerById(serverID);

                        string targetRoot = server.Root.TrimEnd('\\') + (isHistory ? ".vws\\" : "\\");
                        var task = new TaskSenderEntity
                        {
                            TaskCmd = EnumTaskCmd.GETFILEBYTES,
                            CustomCmd = EnumTaskCmd.GETFILEBYTES,
                            TargetRoot = targetRoot,
                            AddList = isHistory ? filePath : filePath.Remove(0, targetRoot.Length)
                        };

                        var client = new SocketFileClient();
                        client.ReceivedFile += ReceivedFile;
                        client.FileFolder = context.Server.MapPath("~/tmp/");
                        client.RecieveFile(IPAddress.Parse(server.IP), task);
                        _are = new AutoResetEvent(false);
                        _are.WaitOne();
                        context.Response.Write(string.Format("{{\"ret\": {0}, \"filepath\":\"{1}\"}}",
                                                             _downloadSucess ? "true" : "false",
                                                             task.AddList.Replace("\\", "\\\\")));
                    }
                    break;
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion IHttpHandler Members

        private void ReceivedFile(Dictionary<string, string> fileMd5, bool success, string errorMsg)
        {
            if (success)
            {
                foreach (var kv in fileMd5)
                {
                    var confirmMd5String = CommonHelper.MD5File(kv.Key);

                    if (confirmMd5String.Equals(kv.Value)) continue;

                    _downloadSucess = false;
                    _are.Set();
                    return;
                }
            }
            else
            {
                _downloadSucess = false;
                _are.Set();
                return;
            }

            _downloadSucess = true;
            _are.Set();
        }
    }

    public static class DownloadFileNameEncode
    {
        /// <summary>
        ///     为字符串中的非英文字符编码
        /// </summary>
        /// <param name = "s"></param>
        /// <returns></returns>
        public static string Encode(string s)
        {
            char[] chars = s.ToCharArray();
            var builder = new StringBuilder();
            foreach (char t in chars)
            {
                var needToEncode = NeedToEncode(t);
                if (needToEncode)
                {
                    string encodedString = ToHexString(t);
                    builder.Append(encodedString);
                }
                else
                {
                    builder.Append(t);
                }
            }

            return builder.ToString();
        }

        ///<summary>
        ///    指定 一个字符是否应该被编码
        ///</summary>
        ///<param name = "chr"></param>
        ///<returns></returns>
        private static bool NeedToEncode(char chr)
        {
            const string reservedChars = "$-_.+!*'(),@=&";

            if (chr > 127)
                return true;
            if (char.IsLetterOrDigit(chr) || reservedChars.IndexOf(chr) >= 0)
                return false;

            return true;
        }

        /// <summary>
        ///     为非英文字符串编码
        /// </summary>
        /// <param name = "chr"></param>
        /// <returns></returns>
        private static string ToHexString(char chr)
        {
            var utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(chr.ToString());
            var builder = new StringBuilder();
            foreach (byte t in encodedBytes)
            {
                builder.AppendFormat("%{0}", Convert.ToString(t, 16));
            }
            return builder.ToString();
        }
    }
}