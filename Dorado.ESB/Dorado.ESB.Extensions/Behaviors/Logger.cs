using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Extensions.Behaviors
{
    #region LogMessageInspector

    /// <summary>
    /// Publisher class
    /// IDispatchMessageInspector定义一些方法，通过这些方法，可以在服务应用程序中对入站和出站应用程序消息进行自定义检查或修改。
    /// IClientMessageInspector 定义一个消息检查器对象，该对象可以添加到 MessageInspectors 集合来查看或修改消息。
    /// </summary>
    public class LogMessageInspector : IDispatchMessageInspector, IClientMessageInspector
    {
        #region Properties

        // stub
        /// <summary>
        /// 接收请求之后
        /// </summary>
        private bool _logAfterReceiveRequest = true;

        /// <summary>
        /// 发送回复之前
        /// </summary>
        private bool _logBeforeSendReply = true;

        // proxy

        /// <summary>
        /// 接收回复之后
        /// </summary>
        private bool _logAfterReceiveReply = true;

        /// <summary>
        /// 发送请求之前
        /// </summary>
        private bool _logBeforeSendRequest = true;

        /// <summary>
        /// 接收请求之后
        /// </summary>
        public bool LogAfterReceiveRequest
        {
            get { return _logAfterReceiveRequest; }
            set { _logAfterReceiveRequest = value; }
        }

        /// <summary>
        /// 发送回复之前
        /// </summary>
        public bool LogBeforeSendReply
        {
            get { return _logBeforeSendReply; }
            set { _logBeforeSendReply = value; }
        }

        /// <summary>
        /// 接收回复之后
        /// </summary>
        public bool LogAfterReceiveReply
        {
            get { return _logAfterReceiveReply; }
            set { _logAfterReceiveReply = value; }
        }

        /// <summary>
        /// 发送请求之前
        /// </summary>
        public bool LogBeforeSendRequest
        {
            get { return _logBeforeSendRequest; }
            set { _logBeforeSendRequest = value; }
        }

        #endregion Properties

        #region IStubMessageInspector

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (request != null)
            {
                if (LogAfterReceiveRequest && request != null)
                {
                    using (MessageBuffer buffer = request.CreateBufferedCopy(int.MaxValue))
                    {
                        Message msg = buffer.CreateMessage();
                        string logMsg = String.Format("\n[{2}]>>>>> ReceivedRequest {0}\n{1}\n", msg.Headers.MessageId, msg, AppDomain.CurrentDomain.FriendlyName);
                        Console.ForegroundColor = msg.IsFault ? ConsoleColor.Red : ConsoleColor.Yellow;
                        Console.WriteLine(logMsg);
                        Console.ResetColor();
                        LoggerWrapper.Logger.Info(logMsg);
                        request = msg;
                    }
                }
            }
            return request;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (LogBeforeSendReply && reply != null)
            {
                using (MessageBuffer buffer = reply.CreateBufferedCopy(int.MaxValue))
                {
                    Message msg = buffer.CreateMessage();
                    string logMsg = String.Format("\n[{2}]<<<<< SendReply {0}\n{1}\n", msg.Headers.ReplyTo, msg, AppDomain.CurrentDomain.FriendlyName);
                    Console.ForegroundColor = msg.IsFault ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.WriteLine(logMsg);
                    Console.ResetColor();

                    LoggerWrapper.Logger.Info(logMsg);
                    reply = msg;
                }
            }
        }

        #endregion IStubMessageInspector

        #region IProxyMessageInspector

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (LogAfterReceiveReply && reply != null)
            {
                using (MessageBuffer buffer = reply.CreateBufferedCopy(int.MaxValue))
                {
                    Message msg = buffer.CreateMessage();
                    string logMsg = String.Empty;
                    Console.ForegroundColor = msg.IsFault ? ConsoleColor.Red : ConsoleColor.Green;
                    if (msg.Headers.RelatesTo != null)
                    {
                        logMsg = String.Format("\n[{2}]>>>>>> ReceivedReply {0}\n{1}\n", msg.Headers.RelatesTo, msg, AppDomain.CurrentDomain.FriendlyName);
                        Console.WriteLine(logMsg);
                    }
                    else
                    {
                        logMsg = String.Format("\n[{2}]>>>>>> ReceivedReply {0}\n{1}\n", "?", msg, AppDomain.CurrentDomain.FriendlyName);
                        Console.WriteLine(logMsg);
                    }
                    Console.ResetColor();
                    LoggerWrapper.Logger.Info(logMsg);
                    reply = msg;
                }
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (LogBeforeSendRequest && request != null)
            {
                using (MessageBuffer buffer = request.CreateBufferedCopy(int.MaxValue))
                {
                    Message msg = buffer.CreateMessage();
                    if (msg.Headers.MessageId == null)
                        msg.Headers.MessageId = new System.Xml.UniqueId();

                    string logMsg = String.Format("\n[{2}]>>>>> SendRequest {0}\n{1}\n", msg.Headers.MessageId, msg, AppDomain.CurrentDomain.FriendlyName);

                    Console.ForegroundColor = msg.IsFault ? ConsoleColor.Red : ConsoleColor.Yellow;
                    Console.WriteLine(logMsg);
                    Console.ResetColor();
                    LoggerWrapper.Logger.Info(logMsg);
                    request = msg;
                }
            }
            return request;
        }

        #endregion IProxyMessageInspector
    }

    #endregion LogMessageInspector

    #region LoggerAttribute

    /// <summary>
    /// Plumbing class
    /// 可以直接使用此声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class LoggerAttribute : Attribute, IEndpointBehavior, IServiceBehavior
    {
        #region Properties

        private bool _enable = true;
        private bool _logAfterReceiveRequest = true;
        private bool _logBeforeSendReply = true;
        private bool _logAfterReceiveReply = true;
        private bool _logBeforeSendRequest = true;

        public bool Enable
        {
            get { return _enable; }
            set { _enable = value; }
        }

        public bool LogAfterReceiveRequest
        {
            get { return _logAfterReceiveRequest; }
            set { _logAfterReceiveRequest = value; }
        }

        public bool LogBeforeSendReply
        {
            get { return _logBeforeSendReply; }
            set { _logBeforeSendReply = value; }
        }

        public bool LogAfterReceiveReply
        {
            get { return _logAfterReceiveReply; }
            set { _logAfterReceiveReply = value; }
        }

        public bool LogBeforeSendRequest
        {
            get { return _logBeforeSendRequest; }
            set { _logBeforeSendRequest = value; }
        }

        #endregion Properties

        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters)
        {
            //nothing to do;
        }

        public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
        {
            if (Enable)
            {
                LogMessageInspector inspector = new LogMessageInspector();
                inspector.LogAfterReceiveRequest = LogAfterReceiveRequest;
                inspector.LogBeforeSendReply = LogBeforeSendReply;
                inspector.LogAfterReceiveReply = LogAfterReceiveReply;
                inspector.LogBeforeSendRequest = LogBeforeSendRequest;
                behavior.MessageInspectors.Add(inspector);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            LogMessageInspector inspector = new LogMessageInspector();
            inspector.LogAfterReceiveRequest = LogAfterReceiveRequest;
            inspector.LogBeforeSendReply = LogBeforeSendReply;
            inspector.LogAfterReceiveReply = LogAfterReceiveReply;
            inspector.LogBeforeSendRequest = LogBeforeSendRequest;
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        public void Validate(ServiceEndpoint serviceEndpoint)
        {
            //nothing to do;
        }

        #endregion IEndpointBehavior Members

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
            //nothing to do;
        }

        public void ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            string logMsg = String.Format("[{1}]: Endpoints in the Service '{0}'", description.ConfigurationName, AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine(logMsg);
            foreach (ServiceEndpoint se in description.Endpoints)
            {
                string strSE = String.Format(" {0}", se.Address.ToString());
                Console.WriteLine(strSE);
                logMsg += String.Format("\n {0}", strSE);
            }
            Console.WriteLine("");
            LoggerWrapper.Logger.Info(logMsg);
            Console.ResetColor();

            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher epdispatch in cd.Endpoints)
                {
                    ApplyDispatchBehavior(null, epdispatch);
                }
            }
        }

        public void Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            //nothing to do;
        }

        #endregion IServiceBehavior Members
    }

    #endregion LoggerAttribute

    #region LoggerBehaviorSection

    /// <summary>
    /// Configuration class
    /// </summary>
    public class LoggerBehaviorSection : BehaviorExtensionElement
    {
        #region Constructor(s)

        public LoggerBehaviorSection()
        {
        }

        #endregion Constructor(s)

        #region ConfigurationProperties

        [ConfigurationProperty("enable", DefaultValue = true)]
        public bool Enable
        {
            get { return (bool)base["enable"]; }
            set { base["enable"] = value; }
        }

        [ConfigurationProperty("logAfterReceiveRequest", DefaultValue = true)]
        public bool LogAfterReceiveRequest
        {
            get { return (bool)base["logAfterReceiveRequest"]; }
            set { base["logAfterReceiveRequest"] = value; }
        }

        [ConfigurationProperty("logBeforeSendReply", DefaultValue = true)]
        public bool LogBeforeSendReply
        {
            get { return (bool)base["logBeforeSendReply"]; }
            set { base["logBeforeSendReply"] = value; }
        }

        [ConfigurationProperty("logAfterReceiveReply", DefaultValue = true)]
        public bool LogAfterReceiveReply
        {
            get { return (bool)base["logAfterReceiveReply"]; }
            set { base["logAfterReceiveReply"] = value; }
        }

        [ConfigurationProperty("logBeforeSendRequest", DefaultValue = true)]
        public bool LogBeforeSendRequest
        {
            get { return (bool)base["logBeforeSendRequest"]; }
            set { base["logBeforeSendRequest"] = value; }
        }

        #endregion ConfigurationProperties

        #region BehaviorExtensionSection

        protected override object CreateBehavior()
        {
            LoggerAttribute logger = new LoggerAttribute();
            logger.Enable = (bool)this["enable"];
            logger.LogAfterReceiveRequest = (bool)this["logAfterReceiveRequest"];
            logger.LogBeforeSendReply = (bool)this["logBeforeSendReply"];
            logger.LogAfterReceiveReply = (bool)this["logAfterReceiveReply"];
            logger.LogBeforeSendRequest = (bool)this["logBeforeSendRequest"];
            return logger;
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            LoggerBehaviorSection section = (LoggerBehaviorSection)from;
            this.Enable = section.Enable;
            this.LogAfterReceiveRequest = section.LogAfterReceiveRequest;
            this.LogBeforeSendReply = section.LogBeforeSendReply;
            this.LogAfterReceiveRequest = section.LogAfterReceiveRequest;
            this.LogAfterReceiveReply = section.LogAfterReceiveReply;
            this.LogBeforeSendRequest = section.LogBeforeSendRequest;
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection collection = new ConfigurationPropertyCollection();
                collection.Add(new ConfigurationProperty("enable", typeof(bool), true));
                collection.Add(new ConfigurationProperty("logAfterReceiveRequest", typeof(bool), true));
                collection.Add(new ConfigurationProperty("logBeforeSendReply", typeof(bool), true));
                collection.Add(new ConfigurationProperty("logAfterReceiveReply", typeof(bool), true));
                collection.Add(new ConfigurationProperty("logBeforeSendRequest", typeof(bool), true));
                return collection;
            }
        }

        #endregion BehaviorExtensionSection

        public override Type BehaviorType
        {
            get { return typeof(LoggerAttribute); }
        }
    }

    #endregion LoggerBehaviorSection
}