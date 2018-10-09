using Dorado.Configuration;
using System.Configuration;

namespace Dorado.Tool.SqlSettings
{
    public class SqlSettingsConfigurationManager : LocalConfigurationWrapper<SqlSettingsConfiguration, SqlSettingsConfigurationManager>
    {
        public SqlSettingsConfigurationManager()
            : base("SqlSettings", ConfigurationManager.AppSettings["SqlSettingsConfig"])

        { }
    }
}