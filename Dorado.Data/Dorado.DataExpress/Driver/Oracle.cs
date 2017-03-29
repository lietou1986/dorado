using System;
using System.Data.OracleClient;

namespace Dorado.DataExpress.Driver
{
    public class Oracle : BaseDriver
    {
        public override Type ConnectionProvider
        {
            get
            {
                return typeof(OracleConnection);
            }
        }

        public override Type CommandProvider
        {
            get
            {
                return typeof(OracleCommand);
            }
        }

        public override Type DataAdapterProvider
        {
            get
            {
                return typeof(OracleDataAdapter);
            }
        }
    }
}