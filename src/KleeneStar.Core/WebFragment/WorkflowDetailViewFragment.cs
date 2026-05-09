using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a fragment that displays detailed information for a workflow, supporting rendering in various
    /// workflow views such as create, edit, and view.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Workflow._workflowid_.Index>]
    [Cache]
    public sealed class WorkflowDetailViewFragment : FragmentControlRestWorkflow
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public WorkflowDetailViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <remarks>
        /// The three predefined views (create, edit, view) are only rendered for standard forms.
        /// Additional forms do not display these tabs as they serve as flexible UI masks with
        /// their own layouts.
        /// </remarks>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var workflowIdParam = renderContext.Request.GetParameter<WorkflowIdParameter>();

            var restUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workflows.WorkflowEditor>()?
                .Add(new UriQuery("id", workflowIdParam?.Value.ToString()))
                .BindParameters(workflowIdParam)
                .BindParameters(renderContext.Request);

            return base.Render(renderContext, visualTree);
        }
    }
}
