// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using Dorado.UrlRewriter.Utilities;

namespace Dorado.UrlRewriter.Parsers
{
    /// <summary>
    /// Parses the IFNOT node.
    /// </summary>
    public class UnlessConditionActionParser : IfConditionActionParser
    {
        /// <summary>
        /// The name of the action.
        /// </summary>
        public override string Name
        {
            get { return Constants.ElementUnless; }
        }
    }
}