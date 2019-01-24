using System.Web.Hosting;

namespace Dorado.Platform.FileSystems.VirtualPath
{
    public interface ICustomVirtualPathProvider
    {
        VirtualPathProvider Instance { get; }
    }
}