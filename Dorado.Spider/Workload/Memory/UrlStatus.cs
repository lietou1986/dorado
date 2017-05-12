using System;

namespace Dorado.Spider.Workload.Memory
{
    /// <summary>
    /// URLStatus: This class holds in memory status information
    /// for URLs. Specifically it holds their processing status,
    /// depth and source URL.
    /// </summary>
    public class UrlStatus
    {
        /// <summary>
        /// The current status of this URL.
        /// </summary>
        public Status CurrentStatus { get; set; }

        /// <summary>
        /// The page that this URL was found on.
        /// </summary>
        public Uri Source { get; set; }

        /// <summary>
        /// The depth of this URL from the starting URL.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// The values for URL statues.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// Waiting to be processed.
            /// </summary>
            Waiting,

            /// <summary>
            /// Successfully processed.
            /// </summary>
            Processed,

            /// <summary>
            /// Unsuccessfully processed.
            /// </summary>
            Error,

            /// <summary>
            /// Currently being processed.
            /// </summary>
            Working
        };
    }
}