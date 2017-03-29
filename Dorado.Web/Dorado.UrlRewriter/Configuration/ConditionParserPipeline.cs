// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Collections;
using Dorado.UrlRewriter.Utilities;

namespace Dorado.UrlRewriter.Configuration
{
    /// <summary>
    /// Pipeline for creating the Condition parsers.
    /// </summary>
    public class ConditionParserPipeline : CollectionBase
    {
        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parserType">The parser type.</param>
        public void AddParser(string parserType)
        {
            AddParser((IRewriteConditionParser)TypeHelper.Activate(parserType, null));
        }

        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public void AddParser(IRewriteConditionParser parser)
        {
            InnerList.Add(parser);
        }
    }
}