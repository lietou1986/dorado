using System;
using System.IO;
using System.Text;
using System.Web;

namespace Dorado.Web.Fileset
{
    public class LocalFileLoader : IFileLoader
    {
        public virtual string Load(string uri)
        {
            return LoadWithEncoding(uri, Encoding.UTF8);
        }

        public virtual string LoadWithEncoding(string uri, Encoding encoding)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException("uri", "指定的Uri不能为空");
            if (encoding == null)
                throw new ArgumentNullException("encoding", "指定编码不能为空");
            if (uri[0] == '~')
                uri = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VirtualPathUtility.ToAbsolute(uri).Substring(1));
            if (!File.Exists(uri))
                throw new FileNotFoundException("指定的静态文件未能找到", uri);
            return File.ReadAllText(uri, encoding);
        }
    }
}