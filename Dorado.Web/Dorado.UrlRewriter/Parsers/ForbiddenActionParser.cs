// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Xml;
using Dorado.UrlRewriter.Actions;
using Dorado.UrlRewriter.Configuration;
using Dorado.UrlRewriter.Utilities;

namespace Dorado.UrlRewriter.Parsers
{
    /// <summary>
    /// Parser for forbidden actions.
    /// </summary>
    public sealed class ForbiddenActionParser : RewriteActionParserBase
    {
        /// <summary>
        /// The name of the action.
        /// </summary>
        public override string Name
        {
            get { return Constants.ElementForbidden; }
        }

        /// <summary>
        /// Whether the action allows nested actions.
        /// </summary>
        public override bool AllowsNestedActions
        {
            get { return false; }
        }

        /// <summary>
        /// Whether the action allows attributes.
        /// </summary>
        public override bool AllowsAttributes
        {
            get { return false; }
        }

        /// <summary>
        /// Parses the node.
        /// </summary>
        /// <param name="node">The node to parse.</param>
        /// <param name="config">The rewriter configuration.</param>
        /// <returns>The parsed action, or null if no action parsed.</returns>
        public override IRewriteAction Parse(XmlNode node, RewriterConfiguration config)
        {
            return new ForbiddenAction();
        }
    }
}