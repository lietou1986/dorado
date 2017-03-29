namespace Dorado.Configuration.ConfigurationManager.CustomInit
{
    public class InitConfigurationManager : LocalConfigurationWrapper<InitConfiguration, InitConfigurationManager>
    {
        public InitConfigurationManager()
            : base("Init", System.Configuration.ConfigurationManager.AppSettings["InitConfig"])

        { }
    }
}