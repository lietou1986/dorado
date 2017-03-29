using System.Reflection;
using System.Resources;

namespace Dorado.ESB.Extensions.Behaviors
{
    internal class ResourceHelper
    {
        // Gets the string associated with the specified key from the resource file
        public static string GetString(string key)
        {
            ResourceManager resourceManager = new ResourceManager(
                "Microsoft.ServiceModel.Samples.Properties.Resources",
                Assembly.GetExecutingAssembly());

            return resourceManager.GetString(key);
        }
    }
}