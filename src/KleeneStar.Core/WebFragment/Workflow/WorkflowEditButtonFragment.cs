using KleeneStar.Core.WebParameter;
using System;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workflow
{
    /// <summary>
    /// Represents a control button fragment that provides an edit action for workflow forms.
    /// </summary>
    [Section<SectionHeadlinePrimary>]
    [Scope<global::KleeneStar.Core.WWW.Workflow._workflowid_.Index>]
    [Cache]
    public sealed class WorkflowEditButtonFragment : FragmentControlButtonLink
    {
        private static readonly IUri _uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Workflow._workflowid_.Edit>();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public WorkflowEditButtonFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "kleenestar.core:workflow.edit.label";
            Icon = _ => new IconPen();
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Primary);
            PrimaryAction = renderContext => new ActionModal
            (
                "modal-form",
                GetUri(renderContext),
                TypeModalSize.ExtraLarge
            );
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            return base.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Retrieves the URI for the edit page of a form based on the current render context.
        /// </summary>
        /// <param name="renderContext">
        /// The context for the current render operation, providing access to request parameters and
        /// rendering state.
        /// </param>
        /// <returns>
        /// An object representing the URI for the edit page, with parameters bound according 
        /// to the current context.
        /// </returns>
        private static IUri GetUri(IRenderControlContext renderContext)
        {
            var workflowIdParameter = renderContext.Request.GetParameter<WorkflowIdParameter>();
            var workflowId = Guid.TryParse(workflowIdParameter?.Value, out var result) ? result : Guid.Empty;

            return _uri.BindParameters(new WorkflowIdParameter(workflowId));
        }
    }
}
