// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Net;

namespace Dorado.UrlRewriter.Actions
{
    /// <summary>
    /// Returns a 404 Not Found HTTP status code.
    /// </summary>
    public sealed class NotFoundAction : SetStatusAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotFoundAction()
            : base(HttpStatusCode.NotFound)
        {
        }
    }
}