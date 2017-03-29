using Dorado.Configuration;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dorado.Web.Configuration.Anti
{
    [XmlRoot("BrowerAndSearchEngineConfiguration")]
    [Serializable]
    public class BrowerAndSearchEngineConfiguration : BaseConfig<BrowerAndSearchEngineConfiguration>
    {
        [XmlArrayItem("BrowerSecurity"), XmlArray("BrowerSecurityItems")]
        public List<BrowerSecurityItems> BrowerSecurityItems
        {
            get;
            set;
        }

        [XmlArrayItem("BrowerBlack"), XmlArray("BrowerBlackItems")]
        public List<BrowerBlackItems> BrowerBlackItems
        {
            get;
            set;
        }

        [XmlArrayItem("SearchEngineSecurity "), XmlArray("SearchEngineSecurityItems")]
        public List<SearchEngineSecurityItems> SearchEngineSecurityItems
        {
            get;
            set;
        }

        [XmlArray("SearchEngineBlackItems"), XmlArrayItem("SearchEngineBlack")]
        public List<SearchEngineBlackItems> SearchEngineBlackItems
        {
            get;
            set;
        }

        public BrowerAndSearchEngineConfiguration()
        {
            BrowerSecurityItems = new List<BrowerSecurityItems>();
            BrowerBlackItems = new List<BrowerBlackItems>();
            SearchEngineSecurityItems = new List<SearchEngineSecurityItems>();
            SearchEngineBlackItems = new List<SearchEngineBlackItems>();
        }
    }
}