using System;
using System.Xml.Serialization;

namespace Dorado.Web.Fileset
{
    [Serializable]
    public class VariableItem
    {
        [XmlAttribute(AttributeName = "key")]
        public string VariableKey
        {
            get;
            set;
        }

        [XmlText]
        public string VariableValue
        {
            get;
            set;
        }

        public VariableItem()
        {
        }

        public VariableItem(string key, string value)
        {
            VariableKey = key;
            VariableValue = value;
        }
    }
}