/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：发送任务及接收返回的SocketClient
 *  -------------------------------------------------------------------------*/

using System;
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
    public class SocketClient
    {
        #region Delegates

        /// <summary>
        ///     接收TaskResultEntity的委托类
        /// </summary>
        /// <param name = "result"></param>
        public delegate void ReceiveTaskResultHandler(TaskResultEntity result);

        #endregion Delegates

        private readonly ManualResetEvent _manualevent = new ManualResetEvent(false);

        /// <summary>
        ///     收到任务返回后的委托定义
        /// </summary>
        public ReceiveTaskResultHandler ReceiveTaskResult;

        private Socket _socketClient;
        private TaskResultEntity _taskResult;
        private TaskResultForLinuxEntity _taskResultForLinux;

        /// <summary>
        ///     发送任务，并异步接收
        /// </summary>
        /// <param name = "ip">目标IP</param>
        /// <param name = "task">任务</param>
        public void AsyncSend(IPAddress ip, TaskSenderEntity task)
        {
            try
            {
                _taskResult = null;
                //连接SocketService
                _socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(ip, 11112);
                _socketClient.Connect(endPoint);

                //把任务转化为JSON串
                var sb = new StringBuilder();
                var jss = new JavaScriptSerializer { MaxJsonLength = 209715200 };
                jss.Serialize(task, sb);
                sb.Append("<EOF>");
                byte[] byteData = Encoding.UTF8.GetBytes(sb.ToString());

                //发送任务
                _socketClient.Send(byteData);

                //开始异步接收
                var state = new StateObject { WorkSocket = _socketClient };
                _socketClient.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None,
                                           CallbackReceive, state);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
            }
        }

        /// <summary>
        ///     发送任务，并同步接收
        /// </summary>
        /// <param name = "ip">目标IP</param>
        /// <param name = "task">任务</param>
        public TaskResultForLinuxEntity SyncSendForLinux(IPAddress ip, TaskSenderForLinuxEntity task)
        {
            try
            {
                _taskResultForLinux = null;
                //连接SocketService
                _socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(ip, 11112);
                _socketClient.Connect(endPoint);

                //把任务转化为JSON串
                var sb = new StringBuilder();
                var jss = new JavaScriptSerializer { MaxJsonLength = 209715200 };
                jss.Serialize(task, sb);
                sb.Append("<EOF>");
                byte[] byteData = Encoding.UTF8.GetBytes(sb.ToString());

                //发送任务
                _socketClient.Send(byteData);

                //开始异步接收
                var state = new StateObject { WorkSocket = _socketClient };
                _socketClient.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None,
                                           CallbackReceive, state);

                //等待线程执行完毕，收到返回
                _manualevent.WaitOne(1000000);
                _manualevent.Reset();
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
            }

            return _taskResultForLinux;
        }

        /// <summary>
        ///     发送任务，并同步接收
        /// </summary>
        /// <param name = "ip">目标IP</param>
        /// <param name = "task">任务</param>
        public TaskResultEntity SyncSend(IPAddress ip, TaskSenderEntity task)
        {
            try
            {
                _taskResult = null;
                //连接SocketService
                _socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(ip, 11112);
                _socketClient.Connect(endPoint);

                //把任务转化为JSON串
                var sb = new StringBuilder();
                var jss = new JavaScriptSerializer { MaxJsonLength = 209715200 };
                jss.Serialize(task, sb);
                sb.Append("<EOF>");
                byte[] byteData = Encoding.UTF8.GetBytes(sb.ToString());

                //发送任务
                _socketClient.Send(byteData);

                //开始异步接收
                var state = new StateObject { WorkSocket = _socketClient };
                _socketClient.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None,
                                           CallbackReceive, state);

                //等待线程执行完毕，收到返回
                _manualevent.WaitOne(1000000);
                _manualevent.Reset();
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
            }

            return _taskResult;
        }

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
                    string tmp = Encoding.UTF8.GetString(
                        state.Buffer, 0, bytesRead);
                    //把接收到的数据加载到上下文对象中
                    state.Bytes.AddRange(state.Buffer.Take(bytesRead).ToArray());

                    if (tmp.EndsWith("<EOF>"))
                    {
                        //任务返回完毕，关闭socket
                        _socketClient.Shutdown(SocketShutdown.Receive);
                        _socketClient.Close();

                        #region 收到任务返回，要处理

                        //转化为TaskResultEntity
                        String content = Encoding.UTF8.GetString(state.Bytes.ToArray());
                        _taskResult = GetTaskResultEntity(content.Remove(content.Length - 5));
                        //通知等待结束
                        _manualevent.Set();
                        //回调委托
                        if (ReceiveTaskResult != null)
                        {
                            ReceiveTaskResult(_taskResult);
                        }

                        #endregion 收到任务返回，要处理
                    }
                    else
                    {
                        //返回未接收完，继续接收
                        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                                             CallbackReceive, state);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
                _manualevent.Set();
            }
        }

        /// <summary>
        ///     转化JSON到TaskResultEntity
        /// </summary>
        /// <param name = "json"></param>
        /// <returns></returns>
        private TaskResultEntity GetTaskResultEntity(string json)
        {
            try
            {
                var jss = new JavaScriptSerializer { MaxJsonLength = 209715200 };
                var objs = jss.Deserialize<TaskResultEntity>(json);
                return objs;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
                return null;
            }
        }
    }
}