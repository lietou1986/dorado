using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;

namespace Dorado.Wcf.DynamicProxy
{
    internal static class ChannelFactoryCreator
    {
        private static Hashtable channelFactories = new Hashtable();
        private static string ServerAddress;

        public static ChannelFactory<T> Create<T>(string serviceName, string serviceAddress)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException("serviceName");
            }
            ChannelFactory<T> channelFactory = null;
            if (channelFactories.ContainsKey(serviceName))
            {
                channelFactory = channelFactories[serviceName] as ChannelFactory<T>;
            }
            if (channelFactory == null)
            {
                NetTcpBinding ntb = new NetTcpBinding();
                ntb.Security.Mode = SecurityMode.None;
                ntb.TransferMode = TransferMode.Streamed;
                ntb.MaxBufferSize = 10240000;
                ntb.MaxReceivedMessageSize = 10240000;
                ntb.ReaderQuotas.MaxArrayLength = 10240000;
                ntb.ReaderQuotas.MaxBytesPerRead = 10240000;
                ntb.ReaderQuotas.MaxDepth = 20;
                ntb.ReaderQuotas.MaxNameTableCharCount = 10240000;
                ntb.ReaderQuotas.MaxStringContentLength = 10240000;
                ntb.CloseTimeout = TimeSpan.Parse("00:10:00");
                ntb.ReceiveTimeout = TimeSpan.Parse("00:10:00");
                ntb.SendTimeout = TimeSpan.Parse("00:10:00");
                ntb.OpenTimeout = TimeSpan.Parse("00:10:00");

                EndpointAddress ea = new EndpointAddress(serviceAddress);
                channelFactory = new ChannelFactory<T>(ntb, ea);
                lock (channelFactories.SyncRoot)
                {
                    channelFactories[serviceAddress] = channelFactory;
                }
            } return channelFactory;
        }
    }

    public class ServiceRealProxy<T> : RealProxy
    {
        private string _serviceName;

        private string _serviceAddress;

        public ServiceRealProxy(string serviceName, string serviceAddress)
            : base(typeof(T))
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException("serviceName");
            }

            if (string.IsNullOrEmpty(serviceAddress))
            {
                throw new ArgumentNullException("serviceAddress");
            }
            _serviceName = serviceName;
            _serviceAddress = serviceAddress;
        }

        public override IMessage Invoke(IMessage msg)
        {
            T channel = ChannelFactoryCreator.Create<T>(_serviceName, _serviceAddress).CreateChannel();
            IMethodCallMessage methodCall = (IMethodCallMessage)msg;
            IMethodReturnMessage methodReturn = null;
            object[] copiedArgs = Array.CreateInstance(typeof(object), methodCall.Args.Length) as object[];
            methodCall.Args.CopyTo(copiedArgs, 0);
            try
            {
                object returnValue = methodCall.MethodBase.Invoke(channel, copiedArgs);
                methodReturn = new ReturnMessage(returnValue, copiedArgs, copiedArgs.Length, methodCall.LogicalCallContext, methodCall); (channel as ICommunicationObject).Close();
            }
            catch (Exception ex)
            {
                if (ex.InnerException is CommunicationException || ex.InnerException is TimeoutException)
                {
                    (channel as ICommunicationObject).Abort();
                }
                if (ex.InnerException != null)
                {
                    methodReturn = new ReturnMessage(ex.InnerException, methodCall);
                }
                else
                {
                    methodReturn = new ReturnMessage(ex, methodCall);
                }
            } return methodReturn;
        }
    }

    public static class EasyWcfProxyFactory
    {
        public static T Create<T>(string serviceName, string serviceAddress)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException("serviceName");
            }
            return (T)(new ServiceRealProxy<T>(serviceName, serviceAddress).GetTransparentProxy());
        }
    }
}