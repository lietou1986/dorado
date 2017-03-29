using System.Web;

namespace Dorado.Web.Exceptions
{
    public sealed class HttpInvalidReqeust : HttpException
    {
        public HttpInvalidReqeust(string paramKey, string paramValue)
            : base(400, string.Format("用户请求[{0}]的值[{1}]可能存在风险", paramKey, paramValue))
        {
        }
    }
}