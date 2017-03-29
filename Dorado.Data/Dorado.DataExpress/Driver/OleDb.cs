using System;
using System.Data.OleDb;

namespace Dorado.DataExpress.Driver
{
    public class OleDb : BaseDriver
    {
        public override Type ConnectionProvider
        {
            get
            {
                return typeof(OleDbConnection);
            }
        }

        public override Type CommandProvider
        {
            get
            {
                return typeof(OleDbCommand);
            }
        }

        public override Type DataAdapterProvider
        {
            get
            {
                return typeof(OleDbDataAdapter);
            }
        }
    }
}