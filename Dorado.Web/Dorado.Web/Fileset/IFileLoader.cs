using System.Text;

namespace Dorado.Web.Fileset
{
    public interface IFileLoader
    {
        string Load(string uri);

        string LoadWithEncoding(string uri, Encoding encoding);
    }
}