using System.Web;

namespace Dorado.Web.Exceptions
{
    public sealed class HttpBrowerInvalidReqeust : HttpException
    {
        public HttpBrowerInvalidReqeust(string configName, string brower)
            : base(403, string.Format("[{0}]浏览器不符合[{1}]的规定", brower, configName))
        {
        }
    }
}