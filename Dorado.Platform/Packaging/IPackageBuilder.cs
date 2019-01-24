using Dorado.Platform.Plugins;
using Dorado.Platform.Themes;
using System.IO;

namespace Dorado.Platform.Packaging
{
    public interface IPackageBuilder
    {
        Stream BuildPackage(PluginDescriptor pluginDescriptor);

        Stream BuildPackage(ThemeManifest themeManifest);
    }
}