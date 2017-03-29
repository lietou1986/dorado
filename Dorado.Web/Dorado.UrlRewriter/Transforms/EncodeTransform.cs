// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Web;
using Dorado.UrlRewriter.Utilities;

namespace Dorado.UrlRewriter.Transforms
{
    /// <summary>
    /// Url encodes the input.
    /// </summary>
    public sealed class EncodeTransform : IRewriteTransform
    {
        /// <summary>
        /// Applies a transformation to the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The transformed string.</returns>
        public string ApplyTransform(string input)
        {
            return HttpUtility.UrlEncode(input);
        }

        /// <summary>
        /// The name of the action.
        /// </summary>
        public string Name
        {
            get { return Constants.TransformEncode; }
        }
    }
}