namespace Dorado.DataExpress.Driver
{
    public class NativeOracle : BaseReflectionDriver
    {
        private string _commandProviderType;
        private string _connectionProviderType;
        private string _dataAdapterProviderType;

        public override string ConnectionProviderType
        {
            get
            {
                return this._connectionProviderType;
            }
            set
            {
                this._connectionProviderType = value;
            }
        }

        public sealed override string CommandProviderType
        {
            get
            {
                return this._commandProviderType;
            }
            set
            {
                this._commandProviderType = value;
            }
        }

        public override string DataAdapterProviderType
        {
            get
            {
                return this._dataAdapterProviderType;
            }
            set
            {
                this._dataAdapterProviderType = value;
            }
        }

        public NativeOracle()
        {
            this._commandProviderType = "Oracle.DataAccess.Client.OracleCommand, Oracle.DataAccess, Version=2.112.1.2, Culture=neutral, PublicKeyToken=89b483f429c47342";
            this._connectionProviderType = "Oracle.DataAccess.Client.OracleConnection, Oracle.DataAccess, Version=2.112.1.2, Culture=neutral, PublicKeyToken=89b483f429c47342";
            this._dataAdapterProviderType = "Oracle.DataAccess.Client.OracleDataAdapter, Oracle.DataAccess, Version=2.112.1.2, Culture=neutral, PublicKeyToken=89b483f429c47342";
        }
    }
}