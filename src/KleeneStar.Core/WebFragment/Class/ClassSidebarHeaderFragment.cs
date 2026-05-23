using KleeneStar.Core.WebParameter;
using System;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Class
{
    /// <summary>
    /// Represents a sidebar header fragment that displays class-related information within 
    /// the user interface sidebar.
    /// </summary>
    [Section<SectionSidebarPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Class._classid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Fields._classid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Forms._classid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Priorities._classid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Workflows._classid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Statuses._classid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Slas._classid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Form._formid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Workflow._workflowid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Sla._slaid_.Index>]
    [Cache]
    public sealed class ClassSidebarHeaderFragment : FragmentControlSidebarItemHeader
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public ClassSidebarHeaderFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var classParameter = renderContext.Request.GetParameter<ClassIdParameter>();
            var formParameter = renderContext.Request.GetParameter<FormIdParameter>();
            var workflowParameter = renderContext.Request.GetParameter<WorkflowIdParameter>();

            // ensure that at least one of the required parameters is present
            if (classParameter == null && formParameter == null && workflowParameter == null)
            {
                throw new InvalidOperationException("One of the parameters 'class', 'form' or 'workflow' must be set.");
            }

            // normalize the class parameter:
            if (classParameter == null && formParameter != null)
            {
                var formId = Guid.TryParse(formParameter.Value, out var formGuid)
                    ? formGuid
                    : Guid.Empty;

                var form = CoreHub.FormManager.GetForm(formId)
                    ?? throw new InvalidOperationException($"Form with ID '{formId}' not found.");

                // create a synthetic class parameter based on the form's ClassId
                classParameter = new ClassIdParameter(form.ClassId.ToString());
            }
            else if (classParameter == null && workflowParameter != null)
            {
                var workflowId = Guid.TryParse(workflowParameter.Value, out var formGuid)
                    ? formGuid
                    : Guid.Empty;

                var workflow = CoreHub.WorkflowManager.GetWorkflow(workflowId)
                    ?? throw new InvalidOperationException($"Workflow with ID '{workflowId}' not found.");

                // create a synthetic class parameter based on the form's ClassId
                classParameter = new ClassIdParameter(workflow.ClassId.ToString());
            }

            var guid = Guid.TryParse(classParameter.Value, out var classGuid)
                ? classGuid
                : Guid.Empty;
            var @class = CoreHub.ClassManager.GetClass(classGuid);

            return base.Render(renderContext, visualTree, @class?.Name);
        }
    }
}
