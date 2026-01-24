using KleeneStar.Core.WebParameter.Workspace;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workspace
{
    /// <summary>
    /// Represents a fragment control for managing workspace tables, providing functionality to 
    /// render the fragment as HTML.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<WWW.Workspaces.Index>]
    [Cache]
    public sealed class WorkspaceTableFragment : FragmentControlRestTable
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public WorkspaceTableFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            RestUri = CoreHub.GetUri<WWW.Api._1_.Workspaces.Table>();
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var categoryParameter = renderContext.Request.GetParameter<CategoryParameter>();

            RestUri = CoreHub
                    .GetUri<WWW.Api._1_.Workspaces.Table>()
                    .Add(categoryParameter is not null
                        ? new UriQuery("category", categoryParameter.Value)
                        : null);

            return base.Render(renderContext, visualTree);
        }
    }
}
