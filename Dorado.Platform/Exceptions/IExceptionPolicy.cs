using System;

namespace Dorado.Platform.Exceptions
{
    public interface IExceptionPolicy
    {
        /* return false if the exception should be rethrown by the caller */

        bool HandleException(object sender, Exception exception);
    }
}