using Dorado.Web.Fileset;
using System.Text;

namespace Dorado.Web.Fileset
{
    public class LocalMiniFileLoader : LocalFileLoader
    {
        public StaticFileType FileType
        {
            get;
            set;
        }

        public LocalMiniFileLoader(StaticFileType type)
        {
            FileType = type;
        }

        public override string LoadWithEncoding(string uri, Encoding encoding)
        {
            string content = base.LoadWithEncoding(uri, encoding);
            switch (FileType)
            {
                case StaticFileType.Css:
                    {
                        return MicrosoftAjaxMinifer.MinifyCss(content);
                    }
                case StaticFileType.Javascript:
                    {
                        return MicrosoftAjaxMinifer.MinifyJavaScript(content);
                    }
            }
            return content;
        }
    }
}