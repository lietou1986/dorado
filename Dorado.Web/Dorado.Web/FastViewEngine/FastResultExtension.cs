using System;
using System.Text;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine
{
    public static class FastResultExtension
    {
        public static FastResult FastResult(this Controller controller, string data)
        {
            return new FastResult(data);
        }

        public static FastResult FastResult(this Controller controller, string data, Encoding contentEncoding)
        {
            return new FastResult(data, contentEncoding);
        }

        public static FastResult FastSuccessResult(this Controller controller)
        {
            return new FastSuccessResult();
        }

        public static FastResult FastSuccessResult(this Controller controller, Encoding contentEncoding)
        {
            return new FastSuccessResult(contentEncoding);
        }

        public static FastResult SuccFastSuccessResultess(this Controller controller, string value)
        {
            return new FastSuccessResult(value);
        }

        public static FastResult FastSuccessResult(this Controller controller, string value, Encoding contentEncoding)
        {
            return new FastSuccessResult(value, contentEncoding);
        }

        public static FastResult FastSuccessResult(this Controller controller, object value)
        {
            return new FastSuccessResult(value);
        }

        public static FastResult FastSuccessResult(this Controller controller, object value, Encoding contentEncoding)
        {
            return new FastSuccessResult(value, contentEncoding);
        }

        public static FastResult FastSuccessResult(this Controller controller, string value, params object[] args)
        {
            return new FastSuccessResult(value, args);
        }

        public static FastResult FastSuccessResult(this Controller controller, Encoding contentEncoding, string value, params object[] args)
        {
            return new FastSuccessResult(contentEncoding, value, args);
        }

        public static FastResult FastErrorResult(this Controller controller, string ex)
        {
            return new FastErrorResult(ex);
        }

        public static FastResult FastErrorResult(this Controller controller, string ex, Encoding contentEncoding)
        {
            return new FastErrorResult(ex, contentEncoding);
        }

        public static FastResult FastErrorResult(this Controller controller, string message, Exception ex)
        {
            return new FastErrorResult(message, ex);
        }

        public static FastResult FastErrorResult(this Controller controller, string message, Exception ex, Encoding contentEncoding)
        {
            return new FastErrorResult(message, ex, contentEncoding);
        }

        public static FastResult FastErrorResult(this Controller controller, Exception ex)
        {
            return new FastErrorResult(ex);
        }

        public static FastResult FastErrorResult(this Controller controller, Exception ex, Encoding contentEncoding)
        {
            return new FastErrorResult(ex, contentEncoding);
        }
    }
}