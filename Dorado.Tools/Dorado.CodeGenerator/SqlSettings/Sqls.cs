using System.Collections.Generic;

namespace Dorado.Tool.SqlSettings
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Sqls
    {
        private List<Sql> sqlField;

        private string useDBField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Sql", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<Sql> SqlList
        {
            get
            {
                return this.sqlField;
            }
            set
            {
                this.sqlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UseDB
        {
            get
            {
                return this.useDBField;
            }
            set
            {
                this.useDBField = value;
            }
        }
    }
}