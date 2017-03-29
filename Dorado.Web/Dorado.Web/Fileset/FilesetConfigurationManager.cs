using Dorado.Configuration;
using System.Configuration;

namespace Dorado.Web.Fileset
{
    public class FilesetConfigurationManager : LocalConfigurationWrapper<FilesetConfiguration, FilesetConfigurationManager>
    {
        public FilesetConfigurationManager()
            : base("Fileset", ConfigurationManager.AppSettings["FilesetConfig"])
        {
        }
    }
}