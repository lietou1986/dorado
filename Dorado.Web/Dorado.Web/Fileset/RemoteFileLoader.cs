using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace Dorado.Web.Fileset
{
    public class RemoteFileLoader : IFileLoader
    {
        public virtual string Load(string uri)
        {
            return LoadWithEncoding(uri, Encoding.UTF8);
        }

        public virtual string LoadWithEncoding(string uri, Encoding encoding)
        {
            WebRequest request = null;
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                request = WebRequest.Create(uri);
                response = request.GetResponse();
                if (response is HttpWebResponse)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    if (httpResponse.ContentType.Contains(';'))
                        encoding = (string.IsNullOrEmpty(httpResponse.ContentType) ? encoding : Encoding.GetEncoding(httpResponse.ContentType.Substring(httpResponse.ContentType.LastIndexOf('=') + 1)));
                    else
                        encoding = Encoding.GetEncoding("gb2312");
                    if (!string.IsNullOrEmpty(httpResponse.ContentEncoding) && string.CompareOrdinal(httpResponse.ContentEncoding, "gzip") == 0)
                        reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress), encoding, true);
                    else
                        reader = new StreamReader(response.GetResponseStream(), encoding, true);
                }
                else
                    reader = new StreamReader(response.GetResponseStream(), encoding, true);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
                if (response != null)
                    response.Close();
                if (request != null)
                    request = null;
            }
        }
    }
}