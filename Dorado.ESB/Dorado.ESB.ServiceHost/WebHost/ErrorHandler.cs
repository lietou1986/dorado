using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.ServiceHost
{
    /// <summary>
    /// This class shows how an error handler can be used as a behavior for a service.  For Dorado Platform Services
    /// </summary>
    /// 在整个服务内修改或插入自定义扩展的机制，
    public class ServiceErrorBehavior1 : IServiceBehavior
    {
        #region ErrorBehaviorAttribute Members

        private ErrorHandler ErrorHandler;

        public ServiceErrorBehavior1()
        {
            this.ErrorHandler = new ErrorHandler();
        }

        //用于检查服务宿主和服务说明，从而确定服务是否可成功运行。
        void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
        }

        /// <summary>
        /// 用于向绑定元素传递自定义数据，以支持协定实现。
        /// </summary>
        /// <param name="description">服务的服务说明。</param>
        /// <param name="serviceHostBase"></param>
        /// <param name="endpoints"></param>
        /// <param name="parameters">绑定元素可访问的自定义对象</param>
        void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        //ApplyDispatchBehavior用于更改运行时属性值或插入自定义扩展对象（例如错误处理程序、消息或参数拦截器、安全扩展以及其他自定义扩展对象）。
        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            //如果出现错误,记录错误
            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                channelDispatcher.ErrorHandlers.Add(ErrorHandler);
            }
        }

        #endregion ErrorBehaviorAttribute Members
    }

    //实现可用于扩展服务或客户端应用程序中的终结点的运行时行为的方法。
    public class EndpointErrorBehavior : IEndpointBehavior
    {
        private ErrorHandler errorHandler = new ErrorHandler();

        #region IEndpointBehavior Members

        //实现此方法可以在运行时将数据传递给绑定，从而支持自定义行为。
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        //在终结点范围内实现客户端的修改或扩展。
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        //在终结点范围内实现客户端的修改或扩展。
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(errorHandler);
        }

        //实现此方法可以确认终结点是否满足某些设定条件。
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion IEndpointBehavior Members
    }

    /// <summary>
    /// 允许实施者对返回给调用方的错误消息进行控制，还可以选择执行自定义错误处理，例如日志记录。
    /// </summary>
    public class ErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        //启用错误相关处理并返回一个值，该值指示调度程序在某些情况下是否中止会话和实例上下文。
        public bool HandleError(Exception error)
        {
            LoggerWrapper.Logger.Error("Dorado.ESB.ServiceHost", error);

            return false;
        }

        //ProvideFault 方法可创建返回到客户端的自定义错误消息。
        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
        }

        #endregion IErrorHandler Members
    }
}