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
    /// Represents a add form fragment for a field.
    /// </summary>
    [Title("kleenestar.core:field.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Fields._classid_.Add>]
    [Cache]
    public sealed class FieldAddFormFragment : FragmentControlRestFormAdd
    {
        /// <summary>
        /// Gets the input text control for specifying the name of the field.
        /// </summary>
        public ControlRestFormItemInputUnique FieldName { get; } = new()
        {
            Name = nameof(Model.Entities.Field.Name),
            Label = "kleenestar.core:field.name.label",
            Placeholder = "kleenestar.core:field.name.placeholder",
            Help = "kleenestar.core:field.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields.UniqueName>()
        };

        /// <summary>
        /// Gets the input text control for specifying the description of the field.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Model.Entities.Field.Description),
            Label = "kleenestar.core:field.description.label",
            Placeholder = "kleenestar.core:field.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Gets the input selection control for the state.
        /// </summary>
        public ControlRestFormItemInputSelection FieldState { get; } = new()
        {
            Name = nameof(Model.Entities.Field.State),
            Label = "kleenestar.core:field.state.label",
            Placeholder = "kleenestar.core:field.state.placeholder",
            Help = "kleenestar.core:field.state.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields.State>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public FieldAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(FieldName);
            Add(Description);
            Add(FieldState);

            Mode = TypeRestFormMode.Add;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields.Index>();
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
