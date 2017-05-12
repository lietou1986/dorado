/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：文件下载
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.ClientHost
{
    internal class FileLoader
    {
        private readonly ManualResetEvent _manualevent = new ManualResetEvent(false);
        private string _errMsg = string.Empty;
        private bool _sendFailed;

        /// <summary>
        ///     下载文件
        /// </summary>
        /// <param name = "syncTaskid">同步任务Id</param>
        /// <param name = "savePath">保存文件夹的绝对路径</param>
        /// <param name = "adds">需要下载的文件（相对路径）</param>
        /// <param name = "targetRoot">提供下载目标服务器根目录（绝对路径）</param>
        /// <param name = "targetIP">提供目标服务器IP</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>结果</returns>
        internal bool Download(int syncTaskid, string savePath, IList<string> adds, string targetRoot, string targetIP,
                               out string errorMsg)
        {
            errorMsg = _errMsg;
            if (adds != null && adds.Count != 0)
            {
                string downloadDir = savePath.Remove(savePath.Length - 1) + ".vws\\" + syncTaskid + "\\";

                var task = new TaskSenderEntity
                                            {
                                                TaskId = syncTaskid,
                                                TaskCmd = EnumTaskCmd.GETFILEBYTES,
                                                TargetRoot = targetRoot,
                                                AddList = string.Join(ClientConst.FileSeparator, adds.ToArray()),
                                            };
                try
                {
                    //Logger.Log("TaskEntity", LogLevel.Info, "HeartBeat Start!");
                    task.DomainType = 0;
                    task.OperatePathType = 0;
                    task.OperatePath = "";
                }
                catch (Exception)
                {
                }
                var client = new SocketFileClient { FileFolder = downloadDir };
                client.ReceivedFile += ReceivedFiles;
                if (client.RecieveFile(IPAddress.Parse(targetIP), task))
                {
                    _manualevent.WaitOne(1200000);
                    errorMsg = _errMsg;
                    return !_sendFailed;
                }

                errorMsg = "接收文件命令发送失败！";
                //_logger.Error("接收文件命令发送失败！");
                LoggerWrapper.Logger.Error("VWS.ClientHost", "接收文件命令发送失败！");
                return false;
            }
            return true;
        }

        private void ReceivedFiles(Dictionary<string, string> fileMd5, bool success, string errorMsg)
        {
            if (success)
            {
                foreach (var kv in fileMd5)
                {
                    string confirmMd5String = CommonHelper.MD5File(kv.Key);

                    if (confirmMd5String.Equals(kv.Value)) continue;
                    //_logger.ErrorFormat("文件接收失败：文件[{0}] Md5[{1}] 错误", kv.Key, kv.Value);
                    LoggerWrapper.Logger.Error("VWS.ClientHost", string.Format("文件接收失败：文件[{0}] Md5[{1}] 错误", kv.Key, kv.Value));
                    _errMsg = string.Format("文件接收失败：文件[{0}] Md5[{1}] 错误", kv.Key, kv.Value);
                    _sendFailed = true;
                    _manualevent.Set();
                    return;
                }
            }
            else
            {
                LoggerWrapper.Logger.Error("VWS.ClientHost", errorMsg);
                //_logger.ErrorFormat(errorMsg);
                _errMsg = errorMsg;
                _sendFailed = true;
                _manualevent.Set();
                return;
            }
            _manualevent.Set();
        }
    }
}