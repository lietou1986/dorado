using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Messaging;

namespace Dorado.Utils
{
    /// <summary>
    /// Description of MsmqUtility.
    /// </summary>
    public class MsmqUtility : IDisposable
    {
        private MessageQueue Queue { get; set; }

        private readonly string _queueAddress;

        public MsmqUtility(string queueAddress)
        {
            _queueAddress = queueAddress;
        }

        private void Open()
        {
            Queue = Queue ?? new MessageQueue(_queueAddress);
        }

        private void Close()
        {
            if (Queue != null)
            {
                Queue.Dispose();
                Queue = null;
            }
        }

        /// <summary>
        /// 顺序发送消息
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool Send(string body)
        {
            MessageQueueTransaction trans = null;
            try
            {
                if (Queue == null) Open();
                if (Queue != null)
                {
                    trans = new MessageQueueTransaction();
                    trans.Begin();
                    Message msg = new Message();
                    ActiveXMessageFormatter formatter = new ActiveXMessageFormatter();
                    formatter.Write(msg, body);
                    Queue.Send(msg, trans);
                    trans.Commit();
                    return true;
                }
                LoggerWrapper.Logger.Trace("发送队列信息异常", "队列地址错误");
                return false;
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Abort();
                LoggerWrapper.Logger.Error("发送队列信息异常", ex);
                return false;
            }
        }

        /// <summary>
        /// 无序发送消息
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool SendNoTrans(string body)
        {
            try
            {
                if (Queue == null) Open();
                if (Queue != null)
                {
                    Message msg = new Message();
                    ActiveXMessageFormatter formatter = new ActiveXMessageFormatter();
                    formatter.Write(msg, body);
                    Queue.Send(msg);
                    return true;
                }
                LoggerWrapper.Logger.Trace("发送队列信息异常", "队列地址错误");
                return false;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("发送队列信息异常", ex.ToString(), ex);
                return false;
            }
        }

        public string Get(TimeSpan timeOut)
        {
            try
            {
                if (Queue == null) Open();
                if (Queue != null)
                {
                    Message msg = Queue.Receive(timeOut);
                    ActiveXMessageFormatter formatter = new ActiveXMessageFormatter();
                    if (msg != null)
                    {
                        return formatter.Read(msg).ToString();
                    }
                }
                else
                {
                    LoggerWrapper.Logger.Error("读取队列信息异常", "队列地址错误");
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("读取队列信息异常", ex);
                return string.Empty;
            }
        }

        public string Get()
        {
            try
            {
                if (Queue == null) Open();
                if (Queue != null)
                {
                    Message msg = Queue.Receive();
                    ActiveXMessageFormatter formatter = new ActiveXMessageFormatter();
                    if (msg != null)
                    {
                        return formatter.Read(msg).ToString();
                    }
                }
                else
                {
                    LoggerWrapper.Logger.Error("读取队列信息异常", "队列地址错误");
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("读取队列信息异常", ex);
                return string.Empty;
            }
        }

        public bool Clear()
        {
            try
            {
                if (Queue == null) Open();
                if (Queue != null)
                {
                    Queue.Purge();
                    return true;
                }

                LoggerWrapper.Logger.Error("清空队列信息异常", "队列地址错误");

                return false;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("清空队列信息异常", ex);
                return false;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}