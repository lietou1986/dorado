// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//

//

using System.Web.UI;
using System.Web.UI.Adapters;

namespace Dorado.UrlRewriter
{
    /// <summary>
    /// ControlAdapter for rewriting form actions
    /// </summary>
    public class FormRewriterControlAdapter : ControlAdapter
    {
        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <param name="writer">The writer to write to</param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(new RewriteFormHtmlTextWriter(writer));
        }
    }
}