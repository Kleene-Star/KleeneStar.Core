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
    /// Represents a edit form fragment for a priority.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Priority._priorityid_.Edit>]
    [Cache]
    public sealed class PriorityEditFormFragment : FragmentControlRestFormEdit
    {
        /// <summary>
        /// Gets the input text control for specifying the name of the field.
        /// </summary>
        public ControlRestFormItemInputUnique PriorityName { get; } = new()
        {
            Name = nameof(Model.Entities.Priority.Name),
            Label = "kleenestar.core:priority.name.label",
            Placeholder = "kleenestar.core:priority.name.placeholder",
            Help = "kleenestar.core:priority.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Priorities.UniqueName>()
        };

        /// <summary>
        /// Gets the input text control for specifying the description of the field.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Model.Entities.Priority.Description),
            Label = "kleenestar.core:priority.description.label",
            Placeholder = "kleenestar.core:priority.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Gets the input selection control for the state.
        /// </summary>
        public ControlRestFormItemInputSelection PriorityState { get; } = new()
        {
            Name = nameof(Model.Entities.Priority.State),
            Label = "kleenestar.core:priority.state.label",
            Placeholder = "kleenestar.core:priority.state.placeholder",
            Help = "kleenestar.core:priority.state.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Priorities.State>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public PriorityEditFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(PriorityName);
            Add(Description);
            Add(PriorityState);

            Mode = TypeRestFormMode.Edit;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Priorities.Index>();
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
            var param = renderContext.Request.GetParameter<PriorityIdParameter>();

            return base.Render(renderContext, visualTree, Items, param?.Value, Uri);
        }
    }
}
