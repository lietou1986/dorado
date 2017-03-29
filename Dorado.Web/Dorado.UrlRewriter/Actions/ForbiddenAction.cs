// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Net;

namespace Dorado.UrlRewriter.Actions
{
    /// <summary>
    /// Returns a 403 Forbidden HTTP status code.
    /// </summary>
    public sealed class ForbiddenAction : SetStatusAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ForbiddenAction()
            : base(HttpStatusCode.Forbidden)
        {
        }
    }
}