using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorado.Core.Queue
{
    public enum MessageStatus
    {
        /// <summary>
        /// Message received successfully
        /// </summary>
        Ok,

        /// <summary>
        /// Queue is empty or occur when time out
        /// </summary>
        IoTimeout,

        /// <summary>
        /// Cannot convert the messsage to expected object type
        /// </summary>
        InvalidType,

        /// <summary>
        /// Exception occurs when receiving the message
        /// </summary>
        QueueError,

        /// <summary>
        /// Exception thrown not by Msmq
        /// </summary>
        Unknown
    }
}
