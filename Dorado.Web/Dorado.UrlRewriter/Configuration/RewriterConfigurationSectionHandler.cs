// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Configuration;
using System.Xml;

namespace Dorado.UrlRewriter.Configuration
{
    /// <summary>
    /// Configuration section handler for the rewriter section.
    /// </summary>
    public sealed class RewriterConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates the settings object.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="configContext">The configuration context.</param>
        /// <param name="section">The section.</param>
        /// <returns>The settings object.</returns>
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            return section;
        }
    }
}