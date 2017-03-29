// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Web;

namespace Dorado.UrlRewriter
{
    /// <summary>
    /// Interface for rewriter error handlers.
    /// </summary>
    public interface IRewriteErrorHandler
    {
        /// <summary>
        /// Handles the error.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        void HandleError(HttpContext context);
    }
}