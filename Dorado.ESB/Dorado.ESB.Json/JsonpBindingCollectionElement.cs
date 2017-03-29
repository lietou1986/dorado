using System.Configuration;
using System.ServiceModel.Configuration;
using System.Web.Configuration;
using System.Web.Hosting;

namespace Dorado.ESB.Json
{
    public class JsonpBindingCollectionElement : StandardBindingCollectionElement<JsonpBinding, JsonpBindingConfigurationElement>
    {
        internal static readonly string JsonpBindingCollectionElementName = "jsonpBinding";

        internal static JsonpBindingCollectionElement GetBindingCollectionElement()
        {
            JsonpBindingCollectionElement result = null;
            BindingsSection bindingsSection = GetBindingsSection();
            if (bindingsSection != null)
            {
                result = bindingsSection[JsonpBindingCollectionElementName] as JsonpBindingCollectionElement;
            }
            return result;
        }

        private static BindingsSection GetBindingsSection()
        {
            BindingsSection bindingsSection = null;

            string sectionName = "system.serviceModel/bindings";
            if (HostingEnvironment.IsHosted)
            {
                if (HostingEnvironment.ApplicationVirtualPath != null)
                {
                    bindingsSection = (BindingsSection)WebConfigurationManager.GetSection(sectionName, HostingEnvironment.ApplicationVirtualPath);
                }
                else
                {
                    bindingsSection = (BindingsSection)WebConfigurationManager.GetSection(sectionName);
                }
            }
            else
            {
                bindingsSection = (BindingsSection)ConfigurationManager.GetSection(sectionName);
            }

            return bindingsSection;
        }
    }
}