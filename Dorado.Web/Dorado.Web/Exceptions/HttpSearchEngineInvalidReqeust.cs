using System.Web;

namespace Dorado.Web.Exceptions
{
    public sealed class HttpSearchEngineInvalidReqeust : HttpException
    {
        public HttpSearchEngineInvalidReqeust(string configName, string host)
            : base(403, string.Format("来源[{0}]不符合[{1}]的规定", host, configName))
        {
        }
    }
}