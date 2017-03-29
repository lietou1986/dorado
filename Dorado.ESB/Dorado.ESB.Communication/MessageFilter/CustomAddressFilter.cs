using System;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Communication
{
    /// <summary>
    /// 用于查询消息的筛选器
    /// </summary>
    public class CustomAddressFilter : MessageFilter
    {
        private MessageFilter innerFilter;

        public CustomAddressFilter(MessageFilter originFilter)
        {
            if (originFilter == null) throw new ArgumentNullException("originFilter");
            this.innerFilter = originFilter;
        }

        public override bool Match(System.ServiceModel.Channels.Message message)
        {
            return innerFilter.Match(message);
        }

        public override bool Match(System.ServiceModel.Channels.MessageBuffer buffer)
        {
            return innerFilter.Match(buffer);
        }
    }
}