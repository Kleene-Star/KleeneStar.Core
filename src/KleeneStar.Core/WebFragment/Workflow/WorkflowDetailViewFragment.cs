using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workflow
{
    /// <summary>
    /// Represents a fragment that displays detailed information for a workflow, supporting rendering in various
    /// workflow views such as create, edit, and view.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Workflow._workflowid_.Index>]
    [Cache]
    public sealed class WorkflowDetailViewFragment : FragmentControlDataWorkflow
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
        /// Renders the control as an HTML node.
        /// </summary>
        /// <param name="renderContext">
        /// The context in which the control is rendered.
        /// </param>
        /// <param name="visualTree">
        /// The visual tree representing the control's structure.
        /// </param>
        /// <returns>
        /// An HTML node representing the rendered control.
        /// </returns>
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
