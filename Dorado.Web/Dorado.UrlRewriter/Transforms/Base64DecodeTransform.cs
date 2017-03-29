// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System;
using System.Text;
using Dorado.UrlRewriter.Utilities;

namespace Dorado.UrlRewriter.Transforms
{
    /// <summary>
    /// Base 64 encodes the input.
    /// </summary>
    public sealed class Base64Transform : IRewriteTransform
    {
        /// <summary>
        /// Applies a transformation to the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The transformed string.</returns>
        public string ApplyTransform(string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// The name of the action.
        /// </summary>
        public string Name
        {
            get { return Constants.TransformBase64Decode; }
        }
    }
}