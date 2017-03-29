using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace Dorado.Core
{
    /// <summary>
    /// ObjectFactory的App.config配置节
    /// </summary>
    public class ObjectFactoryConfigurationSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            Dictionary<Type, ObjectFactoryConfigurationItem> dict = new Dictionary<Type, ObjectFactoryConfigurationItem>();

            foreach (XmlElement element in section.ChildNodes.OfType<XmlElement>().Where(element => element.Name == "add"))
            {
                string objectTypeString = element.GetAttribute("objectType"), creatorTypeString = element.GetAttribute("creatorType");
                if (string.IsNullOrEmpty(objectTypeString) || string.IsNullOrEmpty(creatorTypeString))
                    throw new ConfigurationErrorsException(string.Format("配置文件错误：未指定{0}属性",
                        string.IsNullOrEmpty(objectTypeString) ? "objectType" : "creatorType"), element);

                Type objectType = Type.GetType(objectTypeString), creatorType = Type.GetType(creatorTypeString);
                if (objectType == null || creatorType == null)
                    throw new ConfigurationErrorsException(string.Format("配置文件错误：类型{0}未能正确加载",
                        objectType == null ? objectTypeString : creatorTypeString), element);

                dict.Add(objectType, new ObjectFactoryConfigurationItem(objectType, creatorType));
            }

            return dict;
        }

        #endregion IConfigurationSectionHandler Members
    }

    public class ObjectFactoryConfigurationItem
    {
        public ObjectFactoryConfigurationItem(Type objectType, Type creatorType)
        {
            ObjectType = objectType;
            CreatorType = creatorType;
        }

        public Type ObjectType { get; private set; }

        public Type CreatorType { get; private set; }
    }
}