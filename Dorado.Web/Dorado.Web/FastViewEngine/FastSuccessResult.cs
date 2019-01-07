using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine
{
    public class FastSuccessResult : FastResult
    {
        public FastSuccessResult()
          : this(Encoding.UTF8)
        {
        }

        public FastSuccessResult(Encoding contentEncoding)
          : base(Output.Success(), Encoding.UTF8)
        {
        }

        public FastSuccessResult(string value)
           : this(value, Encoding.UTF8)
        {
        }

        public FastSuccessResult(string value, Encoding contentEncoding) : base(Output.Success(value), contentEncoding)
        {
        }

        public FastSuccessResult(object value)
          : this(value, Encoding.UTF8)
        {
        }

        public FastSuccessResult(object value, Encoding contentEncoding) : base(Output.Success(value), contentEncoding)
        {
        }

        public FastSuccessResult(string value, params object[] args)
        : this(Encoding.UTF8, value, args)
        {
        }

        public FastSuccessResult(Encoding contentEncoding, string value, params object[] args) : base(Output.Success(value, args), contentEncoding)
        {
        }
    }
}