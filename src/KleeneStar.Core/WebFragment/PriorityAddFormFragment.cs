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
    /// Represents a add form fragment for a priority.
    /// </summary>
    [Title("kleenestar.core:priority.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Priorities._classid_.Add>]
    [Cache]
    public sealed class PriorityAddFormFragment : FragmentControlRestFormAdd
    {
        /// <summary>
        /// Returns the input text control for specifying the name of the priority.
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
        /// Returns the input text control for specifying the description of the priority.
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
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public PriorityAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(PriorityName);
            Add(Description);

            Mode = TypeRestFormMode.Add;
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
            return base.Render(renderContext, visualTree, Items, null, Uri);
        }
    }
}
