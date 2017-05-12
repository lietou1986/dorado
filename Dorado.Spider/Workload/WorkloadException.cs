using System;

namespace Dorado.Spider.Workload
{
    /// <summary>
    /// WorkloadException: This exception is thrown when the
    /// workload manager encounters an error.
    /// </summary>
    public class WorkloadException : Exception
    {
        /// <summary>
        /// Throw a workload exception.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        public WorkloadException(String msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Pass on an exception as a WorkloadException.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public WorkloadException(Exception e)
            : base("Exception while processing workload", e)
        {
        }
    }
}