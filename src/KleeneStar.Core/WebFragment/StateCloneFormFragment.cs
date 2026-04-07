using KleeneStar.Core.WebParameter;
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
    /// Represents a clone form fragment for a state.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.State._stateid_.Clone>]
    [Cache]
    public sealed class StateCloneFormFragment : FragmentControlRestFormClone
    {
        /// <summary>
        /// Returns the input text control for specifying the name of the state.
        /// </summary>
        public ControlRestFormItemInputUnique StateName { get; } = new()
        {
            Name = nameof(Model.Entities.WorkflowState.Name),
            Label = "kleenestar.core:state.name.label",
            Placeholder = "kleenestar.core:state.name.placeholder",
            Help = "kleenestar.core:state.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.States.UniqueName>()
        };

        /// <summary>
        /// Returns the input text control for specifying the description of the state.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Model.Entities.WorkflowState.Description),
            Label = "kleenestar.core:state.description.label",
            Placeholder = "kleenestar.core:state.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public StateCloneFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(StateName);
            Add(Description);

            Mode = TypeRestFormMode.Clone;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.States.Index>();
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
            var param = renderContext.Request.GetParameter<WorkflowStateIdParameter>();

            return base.Render(renderContext, visualTree, Items, param?.Value, Uri);
        }
    }
}
