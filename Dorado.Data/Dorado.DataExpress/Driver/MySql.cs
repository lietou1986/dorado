namespace Dorado.DataExpress.Driver
{
    public class MySql : BaseReflectionDriver
    {
        private string _commandProviderString;
        private string _connectionProviderString;
        private string _dataAdapterProviderString;

        public override string ConnectionProviderType
        {
            get
            {
                return this._connectionProviderString;
            }
            set
            {
                this._connectionProviderString = value;
            }
        }

        public override string CommandProviderType
        {
            get
            {
                return this._commandProviderString;
            }
            set
            {
                this._commandProviderString = value;
            }
        }

        public override string DataAdapterProviderType
        {
            get
            {
                return this._dataAdapterProviderString;
            }
            set
            {
                this._dataAdapterProviderString = value;
            }
        }

        public MySql()
        {
            this._commandProviderString = "MySql.Data.MySqlClient.MySqlCommand,MySql.Data";
            this._connectionProviderString = "MySql.Data.MySqlClient.MySqlConnection,MySql.Data";
            this._dataAdapterProviderString = "MySql.Data.MySqlClient.MySqlDataAdapter,MySql.Data";
        }
    }
}