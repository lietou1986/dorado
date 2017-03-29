using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.Extensions.Auth;
using Dorado.Services.Behaviors;
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
    /// </summary>
    public class MsgCheckerInspector : IDispatchMessageInspector, IClientMessageInspector
    {
        #region Properties

        private bool _ipFilter = true;

        public bool IPFilter
        {
            get { return _ipFilter; }
            set { _ipFilter = value; }
        }

        #endregion Properties

        #region IStubMessageInspector

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (IPFilter)
            {
                try
                {
                    if (request != null && System.ServiceModel.OperationContext.Current != null)
                    {
                        HttpRequestMessageProperty httpRequestProp = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                        string clientIP = httpRequestProp.Headers["CLIENT-IP"];

                        if (string.IsNullOrEmpty(clientIP))
                        {
                            RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                            clientIP = prop.Address;
                        }

                        UriTemplateMatch uriMatch = (UriTemplateMatch)request.Properties["UriTemplateMatchResults"];
                        if (!string.IsNullOrEmpty(clientIP))
                        {
                            if (uriMatch != null)
                            {
                                string method = uriMatch.RequestUri.LocalPath;
                                if (!IPSecurityManager.Instance.AllowAccess(clientIP, method.ToLower()))
                                {
                                    IPSecurityMessage errorMessage = new IPSecurityMessage(new PermissionException(new Exception("RequestAddress:" + clientIP + " Method:" + uriMatch.RequestUri.ToString())));

                                    request = errorMessage;
                                    System.ServiceModel.OperationContext.Current.RequestContext.Reply(request);
                                    System.ServiceModel.OperationContext.Current.RequestContext.Abort();
                                    System.ServiceModel.OperationContext.Current.RequestContext = null;

                                    //throw new Exceptions.DoradoException("Not Allow");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("Dorado.ESB.Extensions", ex);
                }
            }
            return request;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        #endregion IStubMessageInspector

        #region IProxyMessageInspector

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return request;
        }

        #endregion IProxyMessageInspector
    }

    #endregion LogMessageInspector

    #region MsgCheckerAttribute

    /// <summary>
    /// Plumbing class
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class MsgCheckerAttribute : Attribute, IEndpointBehavior, IServiceBehavior
    {
        #region Properties

        private bool _ipFilter = true;

        public bool IPFilter
        {
            get { return _ipFilter; }
            set { _ipFilter = value; }
        }

        #endregion Properties

        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters)
        {
            //nothing to do;
        }

        public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
        {
            MsgCheckerInspector inspector = new MsgCheckerInspector();
            behavior.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            MsgCheckerInspector inspector = new MsgCheckerInspector();
            inspector.IPFilter = IPFilter;
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

    #endregion MsgCheckerAttribute

    #region MsgCheckerBehaviorSection

    /// <summary>
    /// Configuration class
    /// </summary>
    public class MsgCheckerBehaviorSection : BehaviorExtensionElement
    {
        #region Constructor(s)

        public MsgCheckerBehaviorSection()
        {
        }

        #endregion Constructor(s)

        #region ConfigurationProperties

        [ConfigurationProperty("IPFilter", DefaultValue = false)]
        public bool IPFilter
        {
            get { return (bool)base["IPFilter"]; }
            set { base["IPFilter"] = value; }
        }

        #endregion ConfigurationProperties

        #region BehaviorExtensionSection

        protected override object CreateBehavior()
        {
            MsgCheckerAttribute msgChecker = new MsgCheckerAttribute();
            msgChecker.IPFilter = (bool)this["IPFilter"];

            return msgChecker;
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            MsgCheckerBehaviorSection section = (MsgCheckerBehaviorSection)from;
            this.IPFilter = section.IPFilter;
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection collection = new ConfigurationPropertyCollection();
                collection.Add(new ConfigurationProperty("IPFilter", typeof(bool), true));
                return collection;
            }
        }

        #endregion BehaviorExtensionSection

        public override Type BehaviorType
        {
            get { return typeof(MsgCheckerAttribute); }
        }
    }

    #endregion MsgCheckerBehaviorSection
}