using Dorado.Configuration;
using System;
using System.Xml.Serialization;

namespace Dorado.Tool.SqlSettings
{
    [Serializable, XmlRoot("SqlSettings", IsNullable = false)]
    public partial class SqlSettingsRemoteConfiguration : BaseConfig<SqlSettingsRemoteConfiguration>
    {
        private Sqls sqlsField;

        private string majorVersionField;

        private string minorVersionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Sqls", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Sqls Sqls
        {
            get
            {
                return this.sqlsField;
            }
            set
            {
                this.sqlsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MajorVersion
        {
            get
            {
                return this.majorVersionField;
            }
            set
            {
                this.majorVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MinorVersion
        {
            get
            {
                return this.minorVersionField;
            }
            set
            {
                this.minorVersionField = value;
            }
        }
    }
}