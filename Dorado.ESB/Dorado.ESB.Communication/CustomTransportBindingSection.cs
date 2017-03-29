using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Dorado.ESB.Communication
{
    public class CustomTransportBindingSection : BindingElementExtensionElement
    {
        private const string SchemeConfigKey = "scheme";
        private const string ChannelFactoryTypeConfigKey = "channelFactoryType";
        private const string ChannelListenerTypeConfigKey = "channelListenerType";

        [ConfigurationProperty(SchemeConfigKey)]
        public string Scheme
        {
            get
            {
                return (string)base[SchemeConfigKey];
            }
            set
            {
                base[SchemeConfigKey] = value;
            }
        }

        [ConfigurationProperty(ChannelFactoryTypeConfigKey)]
        public string ChannelFactoryType
        {
            get
            {
                return (string)base[ChannelFactoryTypeConfigKey];
            }
            set
            {
                base[ChannelFactoryTypeConfigKey] = value;
            }
        }

        [ConfigurationProperty(ChannelListenerTypeConfigKey)]
        public string ChannelListenerType
        {
            get
            {
                return (string)base[ChannelListenerTypeConfigKey];
            }
            set
            {
                base[ChannelListenerTypeConfigKey] = value;
            }
        }

        public override Type BindingElementType
        {
            get { return typeof(CustomTransportBindingElement); }
        }

        protected override BindingElement CreateBindingElement()
        {
            CustomTransportBindingElement binding = new CustomTransportBindingElement();
            ApplyConfiguration(binding);
            return binding;
        }

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            CustomTransportBindingElement binding = (CustomTransportBindingElement)bindingElement;
            binding.SetScheme(this.Scheme);
            binding.ChannelFactoryType = this.ChannelFactoryType;
            binding.ChannelListenerType = this.ChannelListenerType;
        }
    }
}