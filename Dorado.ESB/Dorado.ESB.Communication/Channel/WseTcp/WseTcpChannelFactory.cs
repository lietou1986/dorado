using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class WseTcpChannelFactory : CustomChannelFactory
    {
        #region ctor

        public WseTcpChannelFactory(CustomTransportBindingElement bindingElement, BindingContext context)
            : base(bindingElement, context)
        {
        }

        #endregion ctor

        #region ChannelFactoryBase Members

        protected override IDuplexSessionChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            return new ClientWseTcpChannel(base.EncoderFactory, base.BufferManager, address, via, this);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        #endregion ChannelFactoryBase Members
    }
}