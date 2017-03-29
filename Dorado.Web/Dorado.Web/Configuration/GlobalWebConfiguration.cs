using Dorado.Configuration;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dorado.Web.Configuration
{
    [XmlRoot("global-web")]
    [Serializable]
    public class GlobalWebConfiguration : BaseConfig<GlobalWebConfiguration>
    {
        [XmlArray("registeredAssemblies"), XmlArrayItem(ElementName = "assembly")]
        public List<string> RegisteredAssemblies
        {
            get;
            set;
        }

        static GlobalWebConfiguration()
        {
            ConfigChanged += GlobalWebConfiguration_ConfigChanged;
        }

        private static void GlobalWebConfiguration_ConfigChanged(object sender, EventArgs e)
        {
        }
    }
}