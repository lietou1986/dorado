using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine
{
    public class FastErrorResult : FastResult
    {
        public FastErrorResult(string ex)
            : this(ex, Encoding.UTF8)
        {
        }

        public FastErrorResult(string ex, Encoding contentEncoding) : base(Output.Error(ex), contentEncoding)
        {
        }

        public FastErrorResult(string message, Exception ex)
           : this(message, ex, Encoding.UTF8)
        {
        }

        public FastErrorResult(string message, Exception ex, Encoding contentEncoding)
          : base(Output.Error(message, ex), Encoding.UTF8)
        {
        }

        public FastErrorResult(Exception ex)
         : this(ex, Encoding.UTF8)
        {
        }

        public FastErrorResult(Exception ex, Encoding contentEncoding)
          : base(Output.Error(ex), Encoding.UTF8)
        {
        }
    }
}