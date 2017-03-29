// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System;

namespace Dorado.UrlRewriter.Conditions
{
    /// <summary>
    /// Matches on the current domain address.
    /// </summary>
    public sealed class HostCondition : IRewriteCondition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pattern"></param>
        public HostCondition(string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            _pattern = pattern;
        }

        /// <summary>
        /// Determines if the condition is matched.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        /// <returns>True if the condition is met.</returns>
        public bool IsMatch(RewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            string host = context.Host;
            return host != null && host.ToLower() == Pattern.ToLower();
        }

        /// <summary>
        /// The pattern to match.
        /// </summary>
        public string Pattern
        {
            get { return _pattern; }
        }

        private string _pattern;
    }
}