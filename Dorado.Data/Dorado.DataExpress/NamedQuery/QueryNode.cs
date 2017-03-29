using System;
using System.Xml.Serialization;

namespace Dorado.DataExpress.NamedQuery
{
    [Serializable]
    public class QueryNode
    {
        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlText]
        public string Query
        {
            get;
            set;
        }

        [XmlAttribute("description")]
        public string Description
        {
            get;
            set;
        }

        public QueryNode()
        {
            this.Description = "";
            this.Query = "";
            this.Name = "";
        }
    }
}