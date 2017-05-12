using System;

namespace Dorado.Spider.Filter
{
    /// <summary>
    /// SpiderFilter: Filters will cause the spider to skip
    /// URL's.
    /// </summary>
    public interface ISpiderFilter
    {
        /// <summary>
        /// Check to see if the specified URL is to be excluded.
        /// </summary>
        /// <param name="url">The URL to be checked.</param>
        /// <returns>Returns true if the URL should be excluded.</returns>
        bool IsExcluded(Uri url);

        /// <summary>
        /// Called when a new host is to be processed. SpiderFilter
        /// classes can not be shared among hosts.
        /// </summary>
        /// <param name="host">The new host.</param>
        /// <param name="userAgent">The user agent being used by the spider. Leave
        /// null for default.</param>
        void NewHost(String host, String userAgent);
    }
}