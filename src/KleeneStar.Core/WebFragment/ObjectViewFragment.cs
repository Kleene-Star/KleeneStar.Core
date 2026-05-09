using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a fragment control for managing object tables, providing functionality to 
    /// render the fragment as HTML.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Objects.Index>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Cache]
    public sealed class ObjectViewFragment : ViewControlFragment
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Search.RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects.Wql>();
            Quickfilter.RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Quickfilter>();
            Table.RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Table>();
            Tile.RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Tile>();
            List.List.RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.List>();
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
