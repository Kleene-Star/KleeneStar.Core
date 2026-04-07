using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a add form fragment for a workflow.
    /// </summary>
    [Title("kleenestar.core:workflow.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Workflows._classid_.Add>]
    [Cache]
    public sealed class WorkflowAddFormFragment : FragmentControlRestFormAdd
    {
        /// <summary>
        /// Returns the input text control for specifying the name of the form.
        /// </summary>
        public ControlRestFormItemInputUnique WorkflowName { get; } = new()
        {
            Name = nameof(Model.Entities.Workflow.Name),
            Label = "kleenestar.core:workflow.name.label",
            Placeholder = "kleenestar.core:workflow.name.placeholder",
            Help = "kleenestar.core:workflow.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workflows.UniqueName>()
        };

        /// <summary>
        /// Returns the input text control for specifying the description of the form.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Model.Entities.Workflow.Description),
            Label = "kleenestar.core:workflow.description.label",
            Placeholder = "kleenestar.core:workflow.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public WorkflowAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(WorkflowName);
            Add(Description);

            Mode = TypeRestFormMode.Add;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workflows.Index>();
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
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree, Items, null, Uri);
        }
    }
}
