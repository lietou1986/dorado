using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Dorado.Platform.Extensions
{
    public static class XDocumentExtensions
    {
        public static string ToString(this XDocument document, Encoding encoding)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter stringWriter = new StringWriterWithEncoding(stringBuilder, encoding))
            {
                document.Save(stringWriter);
            }
            return stringBuilder.ToString();
        }
    }
}