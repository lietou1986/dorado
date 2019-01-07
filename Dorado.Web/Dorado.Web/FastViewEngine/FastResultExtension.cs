using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine
{
    public static class FastResultExtension
    {
        public static FastResult Success(this FastResult result)
        {
            return new FastSuccessResult();
        }

        public static FastResult Success(this FastResult result, Encoding contentEncoding)
        {
            return new FastSuccessResult(contentEncoding);
        }

        public static FastResult Success(this FastResult result, string value)
        {
            return new FastSuccessResult(value);
        }

        public static FastResult Success(this FastResult result, string value, Encoding contentEncoding)
        {
            return new FastSuccessResult(value, contentEncoding);
        }

        public static FastResult Success(this FastResult result, object value)
        {
            return new FastSuccessResult(value);
        }

        public static FastResult Success(this FastResult result, object value, Encoding contentEncoding)
        {
            return new FastSuccessResult(value, contentEncoding);
        }

        public static FastResult Success(this FastResult result, string value, params object[] args)
        {
            return new FastSuccessResult(value, args);
        }

        public static FastResult Success(this FastResult result, Encoding contentEncoding, string value, params object[] args)
        {
            return new FastSuccessResult(contentEncoding, value, args);
        }

        public static FastResult Error(this FastResult result, string ex)
        {
            return new FastErrorResult(ex);
        }

        public static FastResult Error(this FastResult result, string ex, Encoding contentEncoding)
        {
            return new FastErrorResult(ex, contentEncoding);
        }

        public static FastResult Error(this FastResult result, string message, Exception ex)
        {
            return new FastErrorResult(message, ex);
        }

        public static FastResult Error(this FastResult result, string message, Exception ex, Encoding contentEncoding)
        {
            return new FastErrorResult(message, ex, contentEncoding);
        }

        public static FastResult Error(this FastResult result, Exception ex)
        {
            return new FastErrorResult(ex);
        }

        public static FastResult Error(this FastResult result, Exception ex, Encoding contentEncoding)
        {
            return new FastErrorResult(ex, contentEncoding);
        }
    }
}