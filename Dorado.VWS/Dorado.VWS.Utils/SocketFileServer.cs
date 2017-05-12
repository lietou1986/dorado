/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：文件传递处理SocketServer
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using Dorado.Core;
using Dorado.Core.Logger;

namespace Dorado.VWS.Utils
{
    public class SocketFileServer
    {
        private static SocketFileServer _singleton;
        private readonly Socket _socketServer;

        private SocketFileServer()
        {
            try
            {
                _socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                var endPoint = new IPEndPoint(IPAddress.Any, 11113);

                _socketServer.Bind(endPoint);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        ///     单例模式
        /// </summary>
        /// <returns>SocketFileServer对象</returns>
        public static SocketFileServer GetInstance()
        {
            return _singleton ?? (_singleton = new SocketFileServer());
        }

        /// <summary>
        ///     开启
        /// </summary>
        /// <returns>结果</returns>
        public bool Start()
        {
            try
            {
                //开始监听并接收请求
                _socketServer.Listen(-1);
                _socketServer.BeginAccept(CallbackAccept, null);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        ///     关闭
        /// </summary>
        public void Close()
        {
            try
            {
                if (_socketServer.Connected) _socketServer.Shutdown(SocketShutdown.Both);
                _socketServer.Close();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        ///     收到连接请求，并开始接收数据
        /// </summary>
        /// <param name = "ar"></param>
        private void CallbackAccept(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = _socketServer.EndAccept(ar);
                //继续接收请求，达到同时处理多请求目的
                _socketServer.BeginAccept(CallbackAccept, null);

                //开始异步接收数据
                var state = new StateObject { WorkSocket = clientSocket };
                clientSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None,
                                          CallbackReceive, state);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        ///     接收数据，转化为任务后处理，并发回结果
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
                    string tmp = Encoding.UTF8.GetString(state.Buffer, 0, bytesRead);
                    //把接收到的数据加载到上下文对象中
                    state.Bytes.AddRange(state.Buffer.Take(bytesRead).ToArray());

                    if (tmp.EndsWith("<EOF>"))
                    {
                        //验证请求IP地址
                        string requestIP = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();
                        if (!Security.CheckRequestIp(requestIP))
                        {
                            return;
                        }

                        #region 收到任务，要处理

                        String content = Encoding.UTF8.GetString(state.Bytes.ToArray());
                        TaskSenderEntity task = GetTaskSenderEntity(content.Remove(content.Length - 5));

                        LoggerWrapper.Logger.Info("VWS", string.Format("Recieved Task: Id[{0}] Name[{1}] FilePath[{2}]", task.TaskId, task.TaskCmd,
                                            task.TargetRoot));

                        #endregion 收到任务，要处理

                        #region 任务结果发回去

                        // 接收到获取文件流的命令
                        if (EnumTaskCmd.GETFILEBYTES.Equals(task.TaskCmd))
                        {
                            // 检查文件是否都存在
                            IList<string> files = CommonHelper.ConvertByComma(task.AddList);

                            var nonExistFiles = files.Select(file => task.TargetRoot + file).Where(target => !File.Exists(target)).ToList();
                            SendCheckResult(handler, nonExistFiles.Count == 0, string.Join(",", nonExistFiles.ToArray()),
                                            files.Count);
                            // 文件都存在时，循环发送文件
                            if (nonExistFiles.Count == 0)
                            {
                                foreach (var file in files)
                                {
                                    SendFile(handler, task.TargetRoot, file);
                                }
                            }
                            // 关闭发送连接
                            CloseSendHandler(handler);
                        }
                        //接收到发送文件流的命令
                        else if (EnumTaskCmd.SENDFILEBYTES.Equals(task.TaskCmd))
                        {
                            handler.Send(BitConverter.GetBytes(1));
                            ReceiveFile(handler);
                        }
                        else
                        {
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }

                        #endregion 任务结果发回去
                    }
                    else
                    {
                        //任务接收不完全，继续异步接收
                        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                                             CallbackReceive, state);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        ///     接收单个文件
        /// </summary>
        private void ReceiveFile(Socket socket)
        {
            //依次获取文件路径长度+文件路径+MD5长度+MD5+文件长度
            var buffer = new byte[4];
            socket.Receive(buffer, 4, SocketFlags.None);
            int fileNameLen = BitConverter.ToInt32(buffer, 0);
            buffer = new byte[fileNameLen];
            socket.Receive(buffer, fileNameLen, SocketFlags.None);
            string fileFullPath = Encoding.UTF8.GetString(buffer);
            buffer = new byte[4];
            socket.Receive(buffer, 4, SocketFlags.None);
            int md5Len = BitConverter.ToInt32(buffer, 0);
            buffer = new byte[md5Len];
            socket.Receive(buffer, md5Len, SocketFlags.None);
            string md5 = Encoding.UTF8.GetString(buffer);
            buffer = new byte[8];
            socket.Receive(buffer, 8, SocketFlags.None);
            long fileLen = BitConverter.ToInt64(buffer, 0);

            // 创建目录
            string directoryPath = Path.GetDirectoryName(fileFullPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            //删除已存在的同名文件(忽略大小写)
            foreach (var file in new DirectoryInfo(directoryPath).GetFiles())
            {
                if (file.FullName.ToLower().Equals(fileFullPath.ToLower()))
                {
                    file.Delete();
                }
            }

            //设置state上下文
            var state = new StateObject
                            {
                                WorkSocket = socket,
                                RemainLength = fileLen,
                                FS = new FileStream(fileFullPath, FileMode.Create),
                                FileMD5 = md5,
                                FileFullPath = fileFullPath
                            };

            //开始异步接收文件
            socket.BeginReceive(state.Buffer, 0,
                                StateObject.BufferSize > state.RemainLength
                                    ? Convert.ToInt32(state.RemainLength)
                                    : StateObject.BufferSize, SocketFlags.None,
                                CallbackFileReceive, state);
        }

        /// <summary>
        ///     接收数据，转化为任务后处理，并发回结果
        /// </summary>
        /// <param name = "ar"></param>
        private void CallbackFileReceive(IAsyncResult ar)
        {
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
                        state.FS.Write(state.Buffer, 0, bytesRead);
                        state.RemainLength -= bytesRead;
                        //文件已读完时，关闭文件，响应_manualevent
                        if (state.RemainLength <= 0)
                        {
                            state.FS.Close();
                            //判断MD5
                            string md5 = CommonHelper.MD5File(state.FileFullPath);
                            handler.Send(md5.Equals(state.FileMD5) ? BitConverter.GetBytes(1) : BitConverter.GetBytes(0));

                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            return;
                        }
                        //文件未读完，则继续
                        handler.BeginReceive(state.Buffer, 0,
                                             StateObject.BufferSize > state.RemainLength
                                                 ? Convert.ToInt32(state.RemainLength)
                                                 : StateObject.BufferSize, SocketFlags.None,
                                             CallbackFileReceive, state);
                    }
                    else
                    {
                        //判断MD5
                        string md5 = CommonHelper.MD5File(state.FileFullPath);
                        handler.Send(md5.Equals(state.FileMD5) ? BitConverter.GetBytes(1) : BitConverter.GetBytes(0));

                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        return;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void SendCheckResult(Socket handler, bool success, string files, int count)
        {
            var checkResult = new List<byte>();
            if (success)
            {
                // 文件都存在，发送0，及文件个数
                checkResult.AddRange(BitConverter.GetBytes(0));
                checkResult.AddRange(BitConverter.GetBytes(count));
            }
            else
            {
                // 文件不存在, 发送1，且发送文件列表
                checkResult.AddRange(BitConverter.GetBytes(1));
                byte[] fileBytes = Encoding.UTF8.GetBytes(files);
                checkResult.AddRange(BitConverter.GetBytes(fileBytes.Length));
                checkResult.AddRange(fileBytes);
            }
            handler.Send(checkResult.ToArray());
        }

        private void SendFile(Socket handler, string fileFolder, string filePath)
        {
            string targetFile = fileFolder + filePath;
            byte[] filePathBytes = Encoding.UTF8.GetBytes(filePath);
            byte[] md5 = Encoding.UTF8.GetBytes(CommonHelper.MD5File(targetFile));

            byte[] filePathLen = BitConverter.GetBytes(filePathBytes.Length);
            byte[] md5Len = BitConverter.GetBytes(md5.Length);
            byte[] fileLen = BitConverter.GetBytes(new FileInfo(targetFile).Length);

            //把它们连起来
            //文件路径长度+文件路径+MD5长度+MD5+文件长度
            byte[] preBuffer = filePathLen.Concat(filePathBytes).Concat(md5Len).Concat(md5).Concat(fileLen).ToArray();

            //开始发送当前文件
            handler.SendFile(targetFile, preBuffer, null, TransmitFileOptions.UseDefaultWorkerThread);
        }

        /// <summary>
        ///     关闭连接的发送端
        /// </summary>
        /// <param name = "handler"></param>
        private void CloseSendHandler(Socket handler)
        {
            handler.Shutdown(SocketShutdown.Send);
            handler.Close();
        }

        /// <summary>
        ///     转化JSON到TaskSenderEntity
        /// </summary>
        /// <param name = "json"></param>
        /// <returns></returns>
        private TaskSenderEntity GetTaskSenderEntity(string json)
        {
            var jss = new JavaScriptSerializer { MaxJsonLength = 209715200 };
            //var objs = jss.Deserialize(json, typeof(TaskEntity));
            var objs = jss.Deserialize<TaskSenderEntity>(json);
            return objs;
        }
    }
}