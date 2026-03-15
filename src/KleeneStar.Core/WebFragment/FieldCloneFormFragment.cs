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
    /// Represents a clone form fragment for a field.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Field._fieldid_.Clone>]
    [Cache]
    public sealed class FieldCloneFormFragment : FragmentControlRestFormClone
    {
        /// <summary>
        /// Returns the input text control for specifying the name of the class.
        /// </summary>
        public ControlRestFormItemInputUnique FieldName { get; } = new()
        {
            Name = nameof(Model.Entities.Field),
            Label = "kleenestar.core:field.name.label",
            Placeholder = "kleenestar.core:field.name.placeholder",
            Help = "kleenestar.core:field.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._workspacekey_.UniqueName>()
        };

        /// <summary>
        /// Returns the input text control for specifying the description of the workspace.
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
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public FieldCloneFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(FieldName);
            Add(Description);

            Mode = TypeRestFormMode.Clone;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes.Index>();
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
            var param = renderContext.Request.GetParameter<ClassIdParameter>();

            return base.Render(renderContext, visualTree, Items, param?.Value, Uri);
        }
    }
}
