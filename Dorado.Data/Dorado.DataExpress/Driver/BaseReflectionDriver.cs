using System;

namespace Dorado.DataExpress.Driver
{
    public abstract class BaseReflectionDriver : BaseDriver
    {
        private Type _commandProvider;
        private Type _connectionProvider;
        private Type _dataAdapterProvider;

        public abstract string ConnectionProviderType
        {
            get;
            set;
        }

        public abstract string CommandProviderType
        {
            get;
            set;
        }

        public abstract string DataAdapterProviderType
        {
            get;
            set;
        }

        public override Type ConnectionProvider
        {
            get
            {
                Type arg_20_0;
                if ((arg_20_0 = this._connectionProvider) == null)
                {
                    arg_20_0 = (this._connectionProvider = Type.GetType(this.ConnectionProviderType, true, true));
                }
                return arg_20_0;
            }
        }

        public override Type CommandProvider
        {
            get
            {
                Type arg_20_0;
                if ((arg_20_0 = this._commandProvider) == null)
                {
                    arg_20_0 = (this._commandProvider = Type.GetType(this.CommandProviderType, true, true));
                }
                return arg_20_0;
            }
        }

        public override Type DataAdapterProvider
        {
            get
            {
                Type arg_20_0;
                if ((arg_20_0 = this._dataAdapterProvider) == null)
                {
                    arg_20_0 = (this._dataAdapterProvider = Type.GetType(this.DataAdapterProviderType, true, true));
                }
                return arg_20_0;
            }
        }
    }
}