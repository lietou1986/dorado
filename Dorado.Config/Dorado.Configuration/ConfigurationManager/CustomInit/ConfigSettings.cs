using System.Collections.Generic;

namespace Dorado.Configuration.ConfigurationManager.CustomInit
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigSettings
    {
        private List<Config> configField;

        private string basePathField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Config", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<Config> Configs
        {
            get
            {
                return this.configField;
            }
            set
            {
                this.configField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BasePath
        {
            get
            {
                return this.basePathField;
            }
            set
            {
                this.basePathField = value;
            }
        }
    }
}