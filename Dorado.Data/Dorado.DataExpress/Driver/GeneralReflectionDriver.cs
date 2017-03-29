namespace Dorado.DataExpress.Driver
{
    public class GeneralReflectionDriver : BaseReflectionDriver
    {
        public override string ConnectionProviderType
        {
            get;
            set;
        }

        public override string CommandProviderType
        {
            get;
            set;
        }

        public override string DataAdapterProviderType
        {
            get;
            set;
        }
    }
}