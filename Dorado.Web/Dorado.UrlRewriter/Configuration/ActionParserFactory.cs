// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System;
using System.Collections.Generic;
using Dorado.UrlRewriter.Utilities;

namespace Dorado.UrlRewriter.Configuration
{
    /// <summary>
    /// Factory for creating the action parsers.
    /// </summary>
    public class ActionParserFactory
    {
        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parserType">The parser type.</param>
        public void AddParser(string parserType)
        {
            AddParser((IRewriteActionParser)TypeHelper.Activate(parserType, null));
        }

        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public void AddParser(IRewriteActionParser parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            IList<IRewriteActionParser> list;

            if (_parsers.ContainsKey(parser.Name))
            {
                list = _parsers[parser.Name];
            }
            else
            {
                list = new List<IRewriteActionParser>();
                _parsers.Add(parser.Name, list);
            }

            list.Add(parser);
        }

        /// <summary>
        /// Returns a list of parsers for the given verb.
        /// </summary>
        /// <param name="verb">The verb.</param>
        /// <returns>A list of parsers</returns>
        public IList<IRewriteActionParser> GetParsers(string verb)
        {
            return (_parsers.ContainsKey(verb))
                    ? _parsers[verb]
                    : null;
        }

        private Dictionary<string, IList<IRewriteActionParser>> _parsers = new Dictionary<string, IList<IRewriteActionParser>>();
    }
}