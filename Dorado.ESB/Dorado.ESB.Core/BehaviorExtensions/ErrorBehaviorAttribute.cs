using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Core.BehaviorExtensions
{
    #region ErrorBehaviorAttribute

    /// <summary>
    /// 自定义错误行为扩展标签
    /// </summary>
    public class ErrorBehaviorAttribute : Attribute, IServiceBehavior
    {
        private Type errorHandlerType;

        #region Constructor

        public ErrorBehaviorAttribute()
        {
            this.errorHandlerType = typeof(ServiceTraceErrorHandler);
        }

        public ErrorBehaviorAttribute(Type errorHandlerType)
        {
            this.errorHandlerType = errorHandlerType;
        }

        #endregion Constructor

        #region IServiceBehavior

        void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler;

            try
            {
                errorHandler = (IErrorHandler)Activator.CreateInstance(errorHandlerType);
            }
            catch (MissingMethodException ex)
            {
                throw new ArgumentException("The errorHandlerType must have a public empty constructor.", ex);
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException("The errorHandlerType must implement System.ServiceModel.Dispatcher.IErrorHandler.", ex);
            }

            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                //添加错误Handler
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }
        }

        #endregion IServiceBehavior
    }

    /// <summary>
    /// 自定义错误处理
    /// </summary>
    public class ServiceTraceErrorHandler : IErrorHandler
    {
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            //JsonError jsonError = new JsonError(error.Message);
            //fault = Message.CreateMessage(version, null, JsonBodyWriter.GetBodyWriter(jsonError));
        }

        public bool HandleError(Exception error)
        {
            try
            {
                //PlatformServicePerfCounters.IncrementExceptions(ServiceEndpointType.UnKnown);
                string errorInfo = String.Empty;
                try
                {
                    errorInfo = String.Format("Action:{0},Address:{1}", OperationContext.Current.RequestContext.RequestMessage.Headers.Action, OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString());

                    if (OperationContext.Current.RequestContext.RequestMessage.Properties["callTime"] != null)
                    {
                        errorInfo += String.Format(" || CallTime={0}", OperationContext.Current.RequestContext.RequestMessage.Properties["callTime"].ToString());
                    }

                    if (OperationContext.Current.RequestContext.RequestMessage.Properties["perfLog"] != null)
                    {
                        errorInfo += String.Format(" || PerfLog={0}", OperationContext.Current.RequestContext.RequestMessage.Properties["perfLog"].ToString());
                    }

                    if (OperationContext.Current.RequestContext.RequestMessage.Properties["callInfo"] != null)
                    {
                        errorInfo += String.Format(" || CallInfo={0}", OperationContext.Current.RequestContext.RequestMessage.Properties["callInfo"].ToString());
                    }
                    else
                    {
                        errorInfo += String.Format(" || CallInfo={0}", OperationContext.Current.RequestContext.RequestMessage.ToString());
                    }
                }
                catch
                {
                }

                if (errorInfo == String.Empty && error.GetType() == typeof(System.ServiceModel.CommunicationException))
                {
                    LoggerWrapper.Logger.Info(AppDomain.CurrentDomain.FriendlyName + ":" + error.Message);
                }
                else
                {
                    LoggerWrapper.Logger.Error(errorInfo, error);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    #endregion ErrorBehaviorAttribute
}