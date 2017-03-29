using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Dorado.Wcf.MiniEncoder
{
    #region MiniEncodingElement

    /// <summary>
    /// 自定义数据压缩配置节
    /// </summary>
    public class MiniEncodingElement : BindingElementExtensionElement
    {
        /// <summary>
        /// 自定义配置节点机和
        /// </summary>
        protected ConfigurationPropertyCollection _properties;

        public override Type BindingElementType
        {
            get
            {
                return typeof(MiniEncodingElement);
            }
        }

        protected override BindingElement CreateBindingElement()
        {
            MiniEncodingBindingElement bindingElement = new MiniEncodingBindingElement();
            ApplyConfiguration(bindingElement);
            return bindingElement;
        }

        /// <summary>
        /// 初始化配置信息
        /// </summary>
        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            MiniEncodingBindingElement element = (MiniEncodingBindingElement)bindingElement;

            // 获取所有自定义配置信息
            PropertyInformationCollection propertyInfo = ElementInformation.Properties;

            // 限制只能启用一个编码器
            if (TextMessageEncodingElement.ElementInformation.IsPresent &&
                BinaryMessageEncodingElement.ElementInformation.IsPresent)
            {
                throw new ConfigurationErrorsException("编码器 'textMessageEncoding' 跟 'binaryMessageEncoding' 只能设置一个！");
            }

            // 确定设置了编码器
            if (!TextMessageEncodingElement.ElementInformation.IsPresent &&
                !BinaryMessageEncodingElement.ElementInformation.IsPresent)
            {
                throw new ConfigurationErrorsException("没有制定编码器，编码器 'textMessageEncoding' 跟 'binaryMessageEncoding' 只能设置一个！");
            }

            if (TextMessageEncodingElement.ElementInformation.IsPresent)
            {
                element.InnerMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
                TextMessageEncodingElement.ApplyConfiguration(element.InnerMessageEncodingBindingElement);
            }
            else if (BinaryMessageEncodingElement.ElementInformation.IsPresent)
            {
                element.InnerMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
                BinaryMessageEncodingElement.ApplyConfiguration(element.InnerMessageEncodingBindingElement);
            }

            //设置压缩算法
            if (CompressAlgorithm != null)
                element.CompressAlgorithm = CompressAlgorithm;
        }

        /// <summary>
        /// 获取所有自定义配置集合
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (_properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("textMessageEncoding", typeof(TextMessageEncodingElement), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("binaryMessageEncoding", typeof(BinaryMessageEncodingElement), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("algorithm", typeof(CompressAlgorithm), null, null, null, ConfigurationPropertyOptions.None));

                    _properties = properties;
                }
                return _properties;
            }
        }

        [ConfigurationProperty("textMessageEncoding")]
        public TextMessageEncodingElement TextMessageEncodingElement
        {
            get
            {
                return (TextMessageEncodingElement)base["textMessageEncoding"];
            }
        }

        [ConfigurationProperty("binaryMessageEncoding")]
        public BinaryMessageEncodingElement BinaryMessageEncodingElement
        {
            get
            {
                return (BinaryMessageEncodingElement)base["binaryMessageEncoding"];
            }
        }

        /// <summary>
        /// 压缩算法配置
        /// </summary>
        [ConfigurationProperty("algorithm", DefaultValue = "GZip")]
        public CompressAlgorithm CompressAlgorithm
        {
            get
            {
                return (CompressAlgorithm)base["algorithm"];
            }
        }
    }

    #endregion MiniEncodingElement
}