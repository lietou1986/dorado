using System;

namespace Dorado.Core.Queue
{
    public interface IQueue<T> : IDisposable
    {
        /// <summary>
        /// 消息处理事件
        /// </summary>
        event ProcessMessageHandler ProcessMessage;

        /// <summary>
        /// 超时或者没有消息时处理事件
        /// </summary>
        event NoMessageOrTimeoutHandler ProcessNoMessageOrTimeout;

        /// <summary>
        /// 消息类型无效的处理事件
        /// </summary>
        event InvalidTypeHandler ProcessInvalidType;

        /// <summary>
        /// 消息入队，返回是否发送成功
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool Push(T msg);

        /// <summary>
        /// 消息出对
        /// </summary>
        /// <returns></returns>
        T Pop();

        /// <summary>
        /// 开始异步消息处理
        /// </summary>
        void StartListener();

        /// <summary>
        /// 停止异步消息处理
        /// </summary>
        void StopListener();
    }

    public class MessageArgs : EventArgs
    {
        public object ObjectToProcess
        {
            get;
            set;
        }
    }

    public delegate void ProcessMessageHandler(object sender, MessageArgs args);

    public delegate void NoMessageOrTimeoutHandler(object sender, EventArgs args);

    public delegate void InvalidTypeHandler(object sender, EventArgs args);
}