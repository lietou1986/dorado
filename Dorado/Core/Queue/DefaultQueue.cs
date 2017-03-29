using Dorado.Core.Logger;
using System;
using System.Messaging;
using System.Threading;

namespace Dorado.Core.Queue
{
    public class Msmq<T> : IQueue<T>
    {
        private readonly MessageQueue _queue;
        private int _currentThreadCount;

        public Msmq(string qName, int timeOut = 100, int threadCount = 10)
        {
            if (!string.IsNullOrEmpty(qName) && !MessageQueue.Exists(qName))
            {
                MessageQueue.Create(qName);
            }
            _queue = new MessageQueue(qName);

            Timeout = TimeSpan.FromMilliseconds(timeOut);
            ThreadCount = threadCount;
        }

        private int _threadCount;

        private int ThreadCount
        {
            get { return _threadCount; }
            set
            {
                _threadCount = value;
                if (_threadCount < 0)
                    _threadCount = 0;
            }
        }

        private TimeSpan Timeout { get; set; }

        public void Push(T element)
        {
            Send(element);
        }

        private void Send(object element)
        {
            using (Message message = new Message())
            {
                message.Body = element;
                message.Formatter = new ActiveXMessageFormatter();
                _queue.Send(message);
            }
        }

        public T Pop()
        {
            return Receive();
        }

        private T Receive()
        {
            T element = default(T);
            try
            {
                using (Message message = _queue.Receive(Timeout))
                {
                    if (message != null)
                    {
                        message.Formatter = new ActiveXMessageFormatter();
                        element = (T)message.Body;
                    }
                }
            }
            catch (MessageQueueException ex)
            {
                //Ingore the exception when queue is empty
                if (ex.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    LoggerWrapper.Logger.Error("msmq receive error", ex);
                }
            }
            return element;
        }

        public void StartListener()
        {
            _queue.ReceiveCompleted += queue_ReceiveCompleted;
            BeginReceiveMessage();
        }

        public void StopListener()
        {
            _queue.ReceiveCompleted -= queue_ReceiveCompleted;
            _currentThreadCount = 0;
        }

        private event ProcessMessageHandler InternelProcessMessage;

        public event ProcessMessageHandler ProcessMessage
        {
            add { InternelProcessMessage += value; }
            remove { InternelProcessMessage -= value; }
        }

        private event NoMessageOrTimeoutHandler NoMessageOrTimeoutHandler;

        public event NoMessageOrTimeoutHandler ProcessNoMessageOrTimeout
        {
            add { NoMessageOrTimeoutHandler += value; }
            remove { NoMessageOrTimeoutHandler -= value; }
        }

        private event InvalidTypeHandler InvalidTypeHandler;

        public event InvalidTypeHandler ProcessInvalidType
        {
            add { InvalidTypeHandler += value; }
            remove { InvalidTypeHandler -= value; }
        }

        private void BeginReceiveMessage()
        {
            _queue.BeginReceive(Timeout);
        }

        private void queue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            Message msg = null;
            MessageStatus status = MessageStatus.Unknown;
            try
            {
                msg = ((MessageQueue)sender).EndReceive(e.AsyncResult);
                status = MessageStatus.Ok;
            }
            catch (MessageQueueException ex)
            {
                status = MessageStatus.QueueError;
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    status = MessageStatus.IoTimeout;
                }
                else
                {
                    LoggerWrapper.Logger.Error("msmq receive error", ex);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("msmq receive error", ex);
            }
            //Handle message in exception condition
            if (msg == null)
            {
                switch (status)
                {
                    case MessageStatus.IoTimeout:
                        if (NoMessageOrTimeoutHandler != null)
                        {
                            NoMessageOrTimeoutHandler(this, null);
                        }
                        break;
                }
                BeginReceiveMessage();
                return;
            }
            msg.Formatter = new ActiveXMessageFormatter();
            if (msg.Body is T)
            {
                T element = (T)msg.Body;
                if (InternelProcessMessage != null)
                {
                    //asynchronously execute
                    if (_threadCount > 0)
                    {
                        Action<T> callback = RunAsnyc;
                        callback.BeginInvoke(element, AfterRunAsnyc, null);
                        //control the multi-thread work flow to make sure only specified threads are working
                        while (true)
                        {
                            if (_currentThreadCount >= _threadCount)
                            {
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                Interlocked.Increment(ref _currentThreadCount);
                                BeginReceiveMessage();
                            }
                        }
                    }
                    // synchronously execute
                    InternelProcessMessage(this, new MessageArgs { ObjectToProcess = element });
                    BeginReceiveMessage();
                }
            }
            else
            {
                if (InvalidTypeHandler != null)
                {
                    InvalidTypeHandler(msg, null);
                }
                else
                {
                    Send(msg);
                }
            }
            msg.Dispose();
        }

        private void AfterRunAsnyc(IAsyncResult itfAr)
        {
            Interlocked.Decrement(ref _currentThreadCount);
        }

        private void RunAsnyc(T element)
        {
            if (InternelProcessMessage == null) return;

            foreach (var v in InternelProcessMessage.GetInvocationList())
            {
                ProcessMessageHandler pmh = (ProcessMessageHandler)v;
                pmh.Invoke(this, new MessageArgs { ObjectToProcess = element });
            }
        }

        void IDisposable.Dispose()
        {
            if (_queue != null)
                _queue.Dispose();
        }
    }
}