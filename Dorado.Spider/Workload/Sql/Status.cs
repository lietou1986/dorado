using System;

namespace Dorado.Spider.Workload.Sql
{
    /// <summary>
    /// Status: This class defines the constant status values for
    /// both the spider_host and spider_workload tables.
    /// </summary>
    internal class Status
    {
        /// <summary>
        /// The item is waiting to be processed.
        /// </summary>
        public const String StatusWaiting = "W";

        /// <summary>
        /// The item was processed, but resulted in an error.
        /// </summary>
        public const String StatusError = "E";

        /// <summary>
        /// The item was processed successfully.
        /// </summary>
        public const String StatusDone = "D";

        /// <summary>
        /// The item is currently being processed.
        /// </summary>
        public const String StatusProcessing = "P";

        /// <summary>
        /// This item should be ignored, only applies to hosts.
        /// </summary>
        public const String StatusIgnore = "I";
    }
}