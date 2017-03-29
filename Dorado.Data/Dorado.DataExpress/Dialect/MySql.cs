namespace Dorado.DataExpress.Dialect
{
    public class MySql : BaseDialect
    {
        private static string sFunctionPrefix = "SELECT RESULT FROM {0}";
        private static string sProcedurePrefix = "Call {0}";
        private static string sParameterPrefix = "?";
        private string _newLine = "\n";
        private string _systemNameTemplate = "`{0}`";

        public override string NewLine
        {
            get
            {
                return this._newLine;
            }
        }

        public override string ParameterPrefix
        {
            get
            {
                return MySql.sParameterPrefix;
            }
        }

        public override string SystemNameTemplate
        {
            get
            {
                return this._systemNameTemplate;
            }
        }

        public override string NoCount
        {
            get
            {
                return "";
            }
        }

        public override string LastIdentity
        {
            get
            {
                return ";\r\nSELECT last_insert_id() 'LastID'";
            }
        }
    }
}