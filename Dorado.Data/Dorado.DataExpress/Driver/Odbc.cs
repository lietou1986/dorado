using System;
using System.Data.Odbc;

namespace Dorado.DataExpress.Driver
{
    public class Odbc : BaseDriver
    {
        public override Type ConnectionProvider
        {
            get
            {
                return typeof(OdbcConnection);
            }
        }

        public override Type CommandProvider
        {
            get
            {
                return typeof(OdbcCommand);
            }
        }

        public override Type DataAdapterProvider
        {
            get
            {
                return typeof(OdbcDataAdapter);
            }
        }
    }
}