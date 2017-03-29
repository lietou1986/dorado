// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System;
using System.Threading;
using Dorado.UrlRewriter.Utilities;

namespace Dorado.UrlRewriter.Transforms
{
    /// <summary>
    /// Transforms the input to upper case.
    /// </summary>
    public sealed class UpperTransform : IRewriteTransform
    {
        /// <summary>
        /// Applies a transformation to the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The transformed string.</returns>
        public string ApplyTransform(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            return input.ToUpper(Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// The name of the action.
        /// </summary>
        public string Name
        {
            get { return Constants.TransformUpper; }
        }
    }
}