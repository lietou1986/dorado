using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Dorado.ESB.Communication
{
    public class CustomEncodingBindingSection : BindingElementExtensionElement
    {
        private const string EncoderTypeConfigKey = "encoderType";

        [ConfigurationProperty(EncoderTypeConfigKey)]
        public string EncoderType
        {
            get
            {
                return (string)base[EncoderTypeConfigKey];
            }
            set
            {
                base[EncoderTypeConfigKey] = value;
            }
        }

        #region ctor

        public CustomEncodingBindingSection()
        {
        }

        #endregion ctor

        public override Type BindingElementType
        {
            get { return typeof(CustomEncodingBindingElement); }
        }

        protected override BindingElement CreateBindingElement()
        {
            CustomEncodingBindingElement binding = new CustomEncodingBindingElement();
            ApplyConfiguration(binding);
            return binding;
        }

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            CustomEncodingBindingElement binding = (CustomEncodingBindingElement)bindingElement;
            binding.EncoderType = this.EncoderType;
        }
    }
}