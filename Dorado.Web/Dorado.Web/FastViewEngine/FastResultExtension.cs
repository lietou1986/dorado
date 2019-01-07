using Dorado.Core.Data;
using System;
using System.Text;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine
{
    public static class FastResultExtension
    {
        public static FastResult FastResult(this ControllerBase controller, DataArray data)
        {
            return new FastResult(data.ToString());
        }

        public static FastResult FastResult(this ControllerBase controller, DataArray data, Encoding contentEncoding)
        {
            return new FastResult(data.ToString(), contentEncoding);
        }

        public static FastResult FastResult(this ControllerBase controller, string data)
        {
            return new FastResult(data);
        }

        public static FastResult FastResult(this ControllerBase controller, string data, Encoding contentEncoding)
        {
            return new FastResult(data, contentEncoding);
        }

        public static FastResult FastSuccessResult(this ControllerBase controller)
        {
            return new FastSuccessResult();
        }

        public static FastResult FastSuccessResult(this ControllerBase controller, Encoding contentEncoding)
        {
            return new FastSuccessResult(contentEncoding);
        }

        public static FastResult SuccFastSuccessResultess(this ControllerBase controller, string value)
        {
            return new FastSuccessResult(value);
        }

        public static FastResult FastSuccessResult(this ControllerBase controller, string value, Encoding contentEncoding)
        {
            return new FastSuccessResult(value, contentEncoding);
        }

        public static FastResult FastSuccessResult(this ControllerBase controller, object value)
        {
            return new FastSuccessResult(value);
        }

        public static FastResult FastSuccessResult(this ControllerBase controller, object value, Encoding contentEncoding)
        {
            return new FastSuccessResult(value, contentEncoding);
        }

        public static FastResult FastSuccessResult(this ControllerBase controller, string value, params object[] args)
        {
            return new FastSuccessResult(value, args);
        }

        public static FastResult FastSuccessResult(this ControllerBase controller, Encoding contentEncoding, string value, params object[] args)
        {
            return new FastSuccessResult(contentEncoding, value, args);
        }

        public static FastResult FastErrorResult(this ControllerBase controller, string ex)
        {
            return new FastErrorResult(ex);
        }

        public static FastResult FastErrorResult(this ControllerBase controller, string ex, Encoding contentEncoding)
        {
            return new FastErrorResult(ex, contentEncoding);
        }

        public static FastResult FastErrorResult(this ControllerBase controller, string message, Exception ex)
        {
            return new FastErrorResult(message, ex);
        }

        public static FastResult FastErrorResult(this ControllerBase controller, string message, Exception ex, Encoding contentEncoding)
        {
            return new FastErrorResult(message, ex, contentEncoding);
        }

        public static FastResult FastErrorResult(this ControllerBase controller, Exception ex)
        {
            return new FastErrorResult(ex);
        }

        public static FastResult FastErrorResult(this ControllerBase controller, Exception ex, Encoding contentEncoding)
        {
            return new FastErrorResult(ex, contentEncoding);
        }
    }
}