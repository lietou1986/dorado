using Dorado.Core.Logger;
using System;
using System.Messaging;
using System.Threading;

namespace Dorado.Core.Queue
{
    public class MsmqQueue<T> : IQueue<T>
    {
        private readonly MessageQueue _queue;
        private int _currentThreadCount;
        private readonly bool _transactional;

        public MsmqQueue(string qName, bool transactional = false, int timeout = 100, int threadCount = 0)
        {
            if (!string.IsNullOrEmpty(qName) && !MessageQueue.Exists(qName))
            {
                MessageQueue.Create(qName, transactional);
            }
            _transactional = transactional;
            _queue = new MessageQueue(qName);
            Timeout = TimeSpan.FromMilliseconds(timeout);
            ThreadCount = threadCount;
        }

        private int _threadCount;

        private int ThreadCount
        {
            set
            {
                _threadCount = value;
                if (_threadCount < 0)
                    _threadCount = 0;
            }
        }

        private TimeSpan Timeout { get; set; }

        public bool Push(T element)
        {
            return Send(element);
        }

        private bool Send(object element)
        {
            MessageQueueTransaction transaction = new MessageQueueTransaction();
            try
            {
                if (_transactional)
                    transaction.Begin();

                Message message = new Message
                {
                    Body = element,
                    Formatter = new ActiveXMessageFormatter()
                };

                _queue.Send(message, transaction);

                if (_transactional)
                    transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("send msmq error", ex);

                if (_transactional)
                    transaction.Abort();

                return false;
            }
        }

        public T Pop()
        {
            return Receive();
        }

        private T Receive()
        {
            T element = default(T);
            MessageQueueTransaction transaction = new MessageQueueTransaction();
            try
            {
                if (_transactional)
                    transaction.Begin();

                Message message = _queue.Receive(Timeout, transaction);

                if (message != null)
                {
                    message.Formatter = new ActiveXMessageFormatter();
                    element = (T)message.Body;
                }

                if (_transactional)
                    transaction.Commit();
            }
            catch (MessageQueueException ex)
            {
                if (_transactional)
                    transaction.Abort();
                if (ex.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    LoggerWrapper.Logger.Error("msmq receive error", ex);
                }
            }
            return element;
        }

        public void StartListener()
        {
            if (ProcessMessageHandler == null)
                throw new CoreException("no handler found to process msg");

            _queue.ReceiveCompleted += queue_ReceiveCompleted;
            BeginReceiveMessage();
        }

        public void StopListener()
        {
            _queue.ReceiveCompleted -= queue_ReceiveCompleted;
            _currentThreadCount = 0;
        }

        private event ProcessMessageHandler ProcessMessageHandler;

        public event ProcessMessageHandler ProcessMessage
        {
            add { ProcessMessageHandler += value; }
            remove { ProcessMessageHandler -= value; }
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

            if (msg == null)
            {
                if (status == MessageStatus.IoTimeout && NoMessageOrTimeoutHandler != null)
                    NoMessageOrTimeoutHandler(this, null);

                BeginReceiveMessage();
                return;
            }
            msg.Formatter = new ActiveXMessageFormatter();
            if (msg.Body is T)
            {
                T element = (T)msg.Body;

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
                else
                {
                    // synchronously execute
                    ProcessMessageHandler(this, new MessageArgs { ObjectToProcess = element });
                    BeginReceiveMessage();
                }
            }
            else
            {
                if (InvalidTypeHandler != null)
                    InvalidTypeHandler(msg, null);
                else
                    LoggerWrapper.Logger.Error("msmq receive error", "invalid msg type");
            }
            msg.Dispose();
        }

        private void AfterRunAsnyc(IAsyncResult itfAr)
        {
            Interlocked.Decrement(ref _currentThreadCount);
        }

        private void RunAsnyc(T element)
        {
            if (ProcessMessageHandler == null) return;

            foreach (var v in ProcessMessageHandler.GetInvocationList())
            {
                ProcessMessageHandler pmh = (ProcessMessageHandler)v;
                pmh.Invoke(this, new MessageArgs { ObjectToProcess = element });
            }
        }

        public void Dispose()
        {
            if (_queue != null)
                _queue.Dispose();
        }
    }
}