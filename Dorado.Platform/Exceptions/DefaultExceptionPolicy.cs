using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Dorado.Platform.Exceptions
{
    public class DefaultExceptionPolicy : IExceptionPolicy
    {
        public bool HandleException(object sender, Exception exception)
        {
            if (IsFatal(exception))
            {
                return false;
            }

            LoggerWrapper.Logger.Error(exception, "An unexpected exception was caught");

            do
            {
                RaiseNotification(exception);
                exception = exception.InnerException;
            } while (exception != null);

            return true;
        }

        private static bool IsFatal(Exception exception)
        {
            return
                exception is StackOverflowException ||
                exception is AccessViolationException ||
                exception is AppDomainUnloadedException ||
                exception is ThreadAbortException ||
                exception is SecurityException ||
                exception is SEHException;
        }

        private void RaiseNotification(Exception exception)
        {
        }
    }
}