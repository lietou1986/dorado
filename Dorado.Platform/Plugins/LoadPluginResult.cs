using System.IO;

namespace Dorado.Platform.Plugins
{
    public class LoadPluginResult
    {
        public FileInfo DescriptionFile { get; set; }
        public PluginDescriptor Descriptor { get; set; }
        public bool IsIncompatible { get; set; }
        public bool Success { get; set; }
    }
}