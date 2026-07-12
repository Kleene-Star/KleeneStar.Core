using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workflow
{
    /// <summary>
    /// Represents a add form fragment for a workflow.
    /// </summary>
    [Title("kleenestar.core:workflow.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Workflows._classid_.Add>]
    [Cache]
    public sealed class WorkflowAddFormFragment : FragmentControlDataFormAdd
    {
        /// <summary>
        /// Gets the input text control for specifying the name of the form.
        /// </summary>
        public ControlDataFormItemInputUnique WorkflowName { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Workflow.Name),
            Label = _ => "kleenestar.core:workflow.name.label",
            Placeholder = _ => "kleenestar.core:workflow.name.placeholder",
            Help = _ => "kleenestar.core:workflow.name.help",
            Required = _ => true,
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workflows.UniqueName>().ToString())};

        /// <summary>
        /// Gets the input selection control for the state.
        /// </summary>
        public ControlDataFormItemInputSelection WorkflowState { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Workflow.State),
            Label = _ => "kleenestar.core:workflow.state.label",
            Placeholder = _ => "kleenestar.core:workflow.state.placeholder",
            Help = _ => "kleenestar.core:workflow.state.help",
            StickySelection = _ => true,
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workflows.State>().ToString())};

        /// <summary>
        /// Gets the input text control for specifying the description of the form.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = _ => nameof(Model.Entities.Workflow.Description),
            Label = _ => "kleenestar.core:workflow.description.label",
            Placeholder = _ => "kleenestar.core:workflow.description.placeholder",
            Format = _ => TypeEditTextFormat.Wysiwyg,
            Required = _ => false
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

            this.DataService<global::KleeneStar.Core.WWW.Api._1_.Workflows.Index>();
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
            return base.Render(renderContext, visualTree);
        }
    }
}
