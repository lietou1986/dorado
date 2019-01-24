using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Web;

namespace Dorado.Platform.Exceptions
{
    public static class ExceptionExtensions
    {
        public static bool IsFatal(this Exception ex)
        {
            return
                ex is StackOverflowException ||
                ex is OutOfMemoryException ||
                ex is AccessViolationException ||
                ex is AppDomainUnloadedException ||
                ex is ThreadAbortException ||
                ex is SecurityException ||
                ex is SEHException;
        }

        public static bool Is404(this Exception ex)
        {
            var httpException = ex as HttpException;
            return httpException != null && httpException.GetHttpCode() == 404;
        }
    }
}