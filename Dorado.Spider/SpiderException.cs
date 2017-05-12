using System;

namespace Dorado.Spider
{
    /// <summary>
    /// SpiderException: This exception is thrown when the spider
    /// encounters an error.
    /// </summary>
    public class SpiderException : Exception
    {
        /// <summary>
        /// Throw a spider exception.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        public SpiderException(String msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Pass on an exception as a SpiderException.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public SpiderException(Exception e)
            : base("Exception while spidering", e)
        {
        }
    }
}