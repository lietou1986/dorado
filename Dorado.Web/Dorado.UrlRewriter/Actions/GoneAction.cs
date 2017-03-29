// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Net;

namespace Dorado.UrlRewriter.Actions
{
    /// <summary>
    /// Returns a 410 Gone HTTP status code.
    /// </summary>
    public sealed class GoneAction : SetStatusAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GoneAction()
            : base(HttpStatusCode.Gone)
        {
        }
    }
}