// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Net;

namespace Dorado.UrlRewriter.Actions
{
    /// <summary>
    /// Returns a 501 Not Implemented HTTP status code.
    /// </summary>
    public sealed class NotImplementedAction : SetStatusAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotImplementedAction()
            : base(HttpStatusCode.NotImplemented)
        {
        }
    }
}