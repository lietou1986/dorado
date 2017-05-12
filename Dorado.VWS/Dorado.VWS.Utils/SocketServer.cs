/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：命令处理SocketServer（文件传递除外）
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using Dorado.Core;
using Dorado.Core.Logger;

#endregion using

namespace Dorado.VWS.Utils
{
    public class SocketServer
    {
        #region Delegates

        public delegate Object ReceiveTaskHandler(TaskSenderEntity task);

        #endregion Delegates

        private static SocketServer _singleton;
        private readonly Socket _socketServer;
        public ReceiveTaskHandler ReceiveTask;

        private SocketServer()
        {
            try
            {
                _socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                var endPoint = new IPEndPoint(IPAddress.Any, 11112);

                _socketServer.Bind(endPoint);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        ///     单例模式
        /// </summary>
        /// <returns>SocketServer对象</returns>
        public static SocketServer GetInstance()
        {
            return _singleton ?? (_singleton = new SocketServer());
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
                LoggerWrapper.Logger.Error("VWS", ex);

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
                LoggerWrapper.Logger.Error("VWS", ex);
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
                LoggerWrapper.Logger.Error("VWS", ex);
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
                        Object obj = null;

                        //验证请求IP地址
                        string requestIP = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();
                        LoggerWrapper.Logger.Info("VWS", string.Format("requestIP.Address{0}", requestIP));
                        if (Security.CheckRequestIp(requestIP))
                        {
                            #region 收到任务，要处理

                            //把收到的数据转化为TaskSender对象
                            String content = Encoding.UTF8.GetString(state.Bytes.ToArray());
                            TaskSenderEntity task = GetTaskSenderEntity(content.Remove(content.Length - 5));

                            //回调委托
                            if (ReceiveTask != null)
                            {
                                obj = ReceiveTask(task);
                            }
                            LoggerWrapper.Logger.Info("VWS", string.Format("Recieved Task: Id[{0}] Name[{1}]", task.TaskId, task.TaskCmd));

                            #endregion 收到任务，要处理
                        }
                        else
                        {
                            TaskResultEntity result = new TaskResultEntity();
                            result.Success = false;
                            result.ErrorMsg = "非法的IP";
                            obj = result;
                        }

                        #region 任务结果发回去

                        if (obj != null)
                        {
                            Send(handler, obj);
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
                LoggerWrapper.Logger.Error("VWS", ex);
            }
        }

        /// <summary>
        ///     发送结果
        /// </summary>
        /// <param name = "handler"></param>
        /// <param name = "taskResult"></param>
        private void Send(Socket handler, Object taskResult)
        {
            try
            {
                //把返回对象转化为JSON格式
                var sb = new StringBuilder();
                var jss = new JavaScriptSerializer { MaxJsonLength = 209715200 };
                jss.Serialize(taskResult, sb);
                sb.Append("<EOF>");

                byte[] byteData = Encoding.UTF8.GetBytes(sb.ToString());
                //开始异步返回发送
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                                  CallBackSend, handler);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
            }
        }

        /// <summary>
        ///     发送结果的回调方法
        /// </summary>
        /// <param name = "ar"></param>
        private void CallBackSend(IAsyncResult ar)
        {
            try
            {
                var handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                LoggerWrapper.Logger.Info("VWS", string.Format("Sent {0} bytes to client.", bytesSent));
                //发送完毕，关闭当前连接的发送
                handler.Shutdown(SocketShutdown.Send);
                handler.Close();
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
            }
        }

        /// <summary>
        ///     转化JSON到TaskSenderEntity
        /// </summary>
        /// <param name = "json"></param>
        /// <returns></returns>
        private TaskSenderEntity GetTaskSenderEntity(string json)
        {
            var jss = new JavaScriptSerializer { MaxJsonLength = 209715200 };
            var objs = jss.Deserialize<TaskSenderEntity>(json);
            return objs;
        }
    }
}