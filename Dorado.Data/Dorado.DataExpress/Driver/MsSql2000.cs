using System;
using System.Data.SqlClient;

namespace Dorado.DataExpress.Driver
{
    public class MsSql2000 : BaseDriver
    {
        private const string SqlBackup = "\r\nBACKUP DATABASE [{0}] TO DISK='{1}'\r\nSELECT '{1}' as 'FILE'\r\n";
        private static readonly Type ConnectionProviderType;
        private static readonly Type CommandProviderType;
        private static readonly Type DataAdapterProviderType;

        public override Type ConnectionProvider
        {
            get
            {
                return MsSql2000.ConnectionProviderType;
            }
        }

        public override Type CommandProvider
        {
            get
            {
                return MsSql2000.CommandProviderType;
            }
        }

        public override Type DataAdapterProvider
        {
            get
            {
                return MsSql2000.DataAdapterProviderType;
            }
        }

        public override bool SupportMuliReader
        {
            get
            {
                return false;
            }
        }

        static MsSql2000()
        {
            MsSql2000.ConnectionProviderType = typeof(SqlConnection);
            MsSql2000.CommandProviderType = typeof(SqlCommand);
            MsSql2000.DataAdapterProviderType = typeof(SqlDataAdapter);
        }

        public override string BackupSql(string databse, string dir)
        {
            return string.Format("\r\nBACKUP DATABASE [{0}] TO DISK='{1}'\r\nSELECT '{1}' as 'FILE'\r\n", databse, dir);
        }
    }
}