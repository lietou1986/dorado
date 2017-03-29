// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Net;

namespace Dorado.UrlRewriter.Actions
{
    /// <summary>
    /// Returns a 405 Method Not Allowed HTTP status code.
    /// </summary>
    public sealed class MethodNotAllowedAction : SetStatusAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MethodNotAllowedAction()
            : base(HttpStatusCode.MethodNotAllowed)
        {
        }
    }
}