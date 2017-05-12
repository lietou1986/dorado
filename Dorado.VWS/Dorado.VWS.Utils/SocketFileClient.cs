/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：发送任务及接收文件的SocketClient
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Dorado.Core;
using Dorado.Core.Logger;

namespace Dorado.VWS.Utils
{
    public class SocketFileClient
    {
        #region Delegates

        /// <summary>
        ///     接收到文件后的委托类
        /// </summary>
        /// <param name = "fileMd5">文件路径及md5</param>
        /// <param name = "success">接收结果</param>
        /// <param name = "errorMsg">错误信息</param>
        public delegate void ReceivedFileHandler(Dictionary<string, string> fileMd5, bool success, string errorMsg);

        #endregion Delegates

        private readonly Dictionary<string, string> _fileMd5 = new Dictionary<string, string>();

        private readonly ManualResetEvent _manualevent = new ManualResetEvent(true);

        /// <summary>
        ///     收到文件后的委托定义
        /// </summary>
        public ReceivedFileHandler ReceivedFile;

        private string _fileFolder;
        private string _fileName;
        private FileStream _fileStream;
        private string _md5;
        private Socket _socketClient;

        /// <summary>
        ///     要保存到的根地址
        /// </summary>
        public string FileFolder
        {
            set { _fileFolder = value; }
        }

        /// <summary>
        ///     发送任务，并异步接收文件
        /// </summary>
        /// <param name = "ip">目标IP</param>
        /// <param name = "task">任务</param>
        /// <returns></returns>
        public bool RecieveFile(IPAddress ip, TaskSenderEntity task)
        {
            try
            {
                //连接SocketService
                _socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(ip, 11113);
                _socketClient.Connect(endPoint);

                //把任务转化为JSON串
                var sb = new StringBuilder();
                var jss = new JavaScriptSerializer { MaxJsonLength = 209715200 };
                jss.Serialize(task, sb);
                sb.Append("<EOF>");
                byte[] byteData = Encoding.UTF8.GetBytes(sb.ToString());

                //异步发送
                _socketClient.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, CallbackSend, null);
                return true;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
                return false;
            }
        }

        /// <summary>
        ///     发送文件到目标
        /// </summary>
        /// <param name = "ip">目标IP</param>
        /// <param name = "originFilePath">要发送文件路径</param>
        /// <param name = "targetFilePath">目标文件路径</param>
        /// <returns>结果</returns>
        public bool SendFile(IPAddress ip, string targetFilePath, string originFilePath)
        {
            try
            {
                //文件不存在，返回失败
                if (!File.Exists(originFilePath))
                {
                    return false;
                }

                //连接SocketService
                _socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(ip, 11113);
                _socketClient.Connect(endPoint);

                var taskSender = new TaskSenderEntity { TaskCmd = EnumTaskCmd.SENDFILEBYTES };
                //把任务转化为JSON串
                var sb = new StringBuilder();
                var jss = new JavaScriptSerializer { MaxJsonLength = 209715200 };
                jss.Serialize(taskSender, sb);
                sb.Append("<EOF>");
                byte[] byteData = Encoding.UTF8.GetBytes(sb.ToString());

                //发送
                _socketClient.Send(byteData);
                //接收
                var buffer = new byte[4];
                _socketClient.Receive(buffer, 4, SocketFlags.None);
                int callback = BitConverter.ToInt32(buffer, 0);
                if (callback == 1)
                {
                    byte[] filePathBytes = Encoding.UTF8.GetBytes(targetFilePath);
                    byte[] md5Bytes = Encoding.UTF8.GetBytes(CommonHelper.MD5File(originFilePath));

                    byte[] filePathLen = BitConverter.GetBytes(targetFilePath.Length);
                    byte[] md5Len = BitConverter.GetBytes(md5Bytes.Length);
                    byte[] fileLen = BitConverter.GetBytes(new FileInfo(originFilePath).Length);

                    //把它们连起来
                    //文件路径长度+文件路径+MD5长度+MD5+文件长度
                    byte[] preBuffer =
                        filePathLen.Concat(filePathBytes).Concat(md5Len).Concat(md5Bytes).Concat(fileLen).ToArray();

                    //开始发送当前文件
                    _socketClient.SendFile(originFilePath, preBuffer, null, TransmitFileOptions.UseDefaultWorkerThread);

                    _socketClient.Receive(buffer, 4, SocketFlags.None);
                    callback = BitConverter.ToInt32(buffer, 0);

                    if (callback == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
                return false;
            }
        }

        /// <summary>
        ///     发送文件到目标
        /// </summary>
        /// <param name = "ip">目标IP</param>
        /// <param name = "targetFilePath">目标文件路径</param>
        /// <param name="fileBytes"></param>
        /// <returns>结果</returns>
        public bool SendFile(IPAddress ip, string targetFilePath, byte[] fileBytes)
        {
            try
            {
                //连接SocketService
                _socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(ip, 11113);
                _socketClient.Connect(endPoint);

                var taskSender = new TaskSenderEntity { TaskCmd = EnumTaskCmd.SENDFILEBYTES };
                //把任务转化为JSON串
                var sb = new StringBuilder();
                var jss = new JavaScriptSerializer { MaxJsonLength = 209715200 };
                jss.Serialize(taskSender, sb);
                sb.Append("<EOF>");
                byte[] byteData = Encoding.UTF8.GetBytes(sb.ToString());

                //发送
                _socketClient.Send(byteData);
                //接收
                var buffer = new byte[4];
                _socketClient.Receive(buffer, 4, SocketFlags.None);
                int callback = BitConverter.ToInt32(buffer, 0);
                if (callback == 1)
                {
                    byte[] filePathBytes = Encoding.UTF8.GetBytes(targetFilePath);
                    byte[] md5Bytes = Encoding.UTF8.GetBytes(CommonHelper.MD5Buffer(fileBytes));

                    byte[] filePathLen = BitConverter.GetBytes(targetFilePath.Length);
                    byte[] md5Len = BitConverter.GetBytes(md5Bytes.Length);
                    byte[] fileLen = BitConverter.GetBytes(long.Parse(fileBytes.Length.ToString()));

                    //把它们连起来
                    //文件路径长度+文件路径+MD5长度+MD5+文件长度
                    byte[] preBuffer =
                        filePathLen.Concat(filePathBytes).Concat(md5Len).Concat(md5Bytes).Concat(fileLen).ToArray();

                    //开始发送当前文件
                    _socketClient.Send(preBuffer);
                    _socketClient.Send(fileBytes);

                    _socketClient.Receive(buffer, 4, SocketFlags.None);
                    callback = BitConverter.ToInt32(buffer, 0);

                    if (callback == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
                return false;
            }
        }

        /// <summary>
        ///     消息发送的回调
        /// </summary>
        /// <param name = "ar"></param>
        private void CallbackSend(IAsyncResult ar)
        {
            bool recieveFlag = false;
            string errorMsg = string.Empty;
            try
            {
                _socketClient.EndSend(ar);

                //获取接收到的文件个数
                int count = ReceiveCheckCount(out errorMsg);

                if (count == 0)
                {
                    //文件接收失败，关闭连接
                    _socketClient.Shutdown(SocketShutdown.Receive);
                    _socketClient.Close();
                    LoggerWrapper.Logger.Error("VWS", "文件接收失败");
                }
                else
                {
                    //逐个接收文件
                    for (int nIndex = 0; nIndex < count; nIndex++)
                    {
                        _manualevent.Reset();
                        ReceiveFile();
                        _manualevent.WaitOne(300000);
                    }

                    //文件接收完毕，关闭连接
                    _socketClient.Shutdown(SocketShutdown.Receive);
                    _socketClient.Close();
                    recieveFlag = true;
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
                recieveFlag = false;
                errorMsg = ex.ToString();
            }
            finally
            {
                //回调委托
                if (ReceivedFile != null)
                {
                    ReceivedFile(_fileMd5, recieveFlag, errorMsg);
                }
            }
        }

        /// <summary>
        ///     接收单个文件
        /// </summary>
        private void ReceiveFile()
        {
            //依次获取文件路径长度+文件路径+MD5长度+MD5+文件长度
            var buffer = new byte[4];
            _socketClient.Receive(buffer, 4, SocketFlags.None);
            int fileNameLen = BitConverter.ToInt32(buffer, 0);
            buffer = new byte[fileNameLen];
            _socketClient.Receive(buffer, fileNameLen, SocketFlags.None);
            _fileName = Encoding.UTF8.GetString(buffer);
            buffer = new byte[4];
            _socketClient.Receive(buffer, 4, SocketFlags.None);
            int md5Len = BitConverter.ToInt32(buffer, 0);
            buffer = new byte[md5Len];
            _socketClient.Receive(buffer, md5Len, SocketFlags.None);
            _md5 = Encoding.UTF8.GetString(buffer);
            buffer = new byte[8];
            _socketClient.Receive(buffer, 8, SocketFlags.None);
            long fileLen = BitConverter.ToInt64(buffer, 0);

            string filePath = _fileFolder + _fileName;
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            _fileStream = new FileStream(filePath, FileMode.Create);
            var state = new StateObject { WorkSocket = _socketClient, RemainLength = fileLen };

            //开始异步接收文件
            _socketClient.BeginReceive(state.Buffer, 0,
                                       StateObject.BufferSize > state.RemainLength
                                           ? Convert.ToInt32(state.RemainLength)
                                           : StateObject.BufferSize, SocketFlags.None,
                                       CallbackReceive, state);
        }

        /// <summary>
        ///     接收文件回调
        /// </summary>
        /// <param name = "ar"></param>
        private void CallbackReceive(IAsyncResult ar)
        {
            try
            {
                var state = (StateObject)ar.AsyncState;
                Socket handler = state.WorkSocket;

                // 当前接收到的byte大小
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // 读到一部分，就写入文件
                    _fileStream.Write(state.Buffer, 0, bytesRead);
                    state.RemainLength -= bytesRead;
                    //文件已读完时，关闭文件，响应_manualevent
                    if (state.RemainLength <= 0)
                    {
                        _fileStream.Close();
                        _fileMd5.Add(_fileFolder + _fileName, _md5);
                        _manualevent.Set();
                        return;
                    }
                    //文件未读完，则继续
                    _socketClient.BeginReceive(state.Buffer, 0,
                                               StateObject.BufferSize > state.RemainLength
                                                   ? Convert.ToInt32(state.RemainLength)
                                                   : StateObject.BufferSize, SocketFlags.None,
                                               CallbackReceive, state);
                }
                else
                {
                    //文件读到0字节，说明是空文件
                    _fileStream.Close();
                    _fileMd5.Add(_fileFolder + _fileName, _md5);
                    _manualevent.Set();
                    return;
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
                _manualevent.Set();
            }
        }

        /// <summary>
        ///     检查接收到的结果
        /// </summary>
        /// <returns></returns>
        private int ReceiveCheckCount(out string errMsg)
        {
            errMsg = string.Empty;
            int count = 0;
            // 接收Check结果，0是成功，1失败
            var buffer = new byte[4];
            _socketClient.Receive(buffer, 4, SocketFlags.None);
            int checkResult = BitConverter.ToInt32(buffer, 0);
            if (checkResult == 0)
            {
                // 获取文件个数
                _socketClient.Receive(buffer, 4, SocketFlags.None);
                count = BitConverter.ToInt32(buffer, 0);
            }
            else
            {
                //文件接收失败
                _socketClient.Receive(buffer, 4, SocketFlags.None);
                int errorLen = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[errorLen];
                _socketClient.Receive(buffer, errorLen, SocketFlags.None);
                string errorMsg = Encoding.UTF8.GetString(buffer);
                LoggerWrapper.Logger.Error("VWS", string.Format("文件不存在：{0}", errorMsg));
                errMsg = "文件不存在：" + errorMsg;
            }
            return count;
        }
    }
}