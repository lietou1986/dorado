using System;
using System.Runtime.Serialization;

namespace Dorado.Platform.Exceptions
{
    [Serializable]
    public class SitemapException : Exception
    {
        public SitemapException(string message) : base(message)
        {
        }

        public SitemapException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SitemapException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}