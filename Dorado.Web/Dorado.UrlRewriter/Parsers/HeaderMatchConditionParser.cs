// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System;
using System.Xml;
using Dorado.UrlRewriter.Conditions;
using Dorado.UrlRewriter.Utilities;

namespace Dorado.UrlRewriter.Parsers
{
    /// <summary>
    /// Parser for header match conditions.
    /// </summary>
    public sealed class HeaderMatchConditionParser : IRewriteConditionParser
    {
        /// <summary>
        /// Parses the condition.
        /// </summary>
        /// <param name="node">The node to parse.</param>
        /// <returns>The condition parsed, or null if nothing parsed.</returns>
        public IRewriteCondition Parse(XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            XmlNode headerAttr = node.Attributes.GetNamedItem(Constants.AttrHeader);
            if (headerAttr == null)
            {
                return null;
            }

            string match = node.GetRequiredAttribute(Constants.AttrMatch, true);
            return new PropertyMatchCondition(headerAttr.Value, match);
        }
    }
}