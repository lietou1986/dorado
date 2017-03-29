using System.Xml;

namespace Dorado.Security.Certificate
{
    public class ConfigMessage
    {
        private XmlDocument _ConfigDoc;
        private XmlNode _Node;

        public XmlDocument ConfigDoc
        {
            get
            {
                return this._ConfigDoc;
            }
            set
            {
                this._ConfigDoc = value;
            }
        }

        public XmlNode Node
        {
            get
            {
                return this._Node;
            }
            set
            {
                this._Node = value;
            }
        }

        public ConfigMessage()
        {
        }

        public ConfigMessage(string ConfigPath)
        {
            try
            {
                this.ConfigDoc = new XmlDocument();
                this.ConfigDoc.Load(ConfigPath);
                this.Node = this.ConfigDoc.SelectSingleNode("Dorado");
            }
            catch (System.Exception)
            {
            }
        }
    }
}