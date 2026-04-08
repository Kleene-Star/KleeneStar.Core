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
    /// Represents a add form fragment for a class.
    /// </summary>
    [Title("kleenestar.core:class.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Classes._workspacekey_.Add>]
    [Cache]
    public sealed class ClassAddFormFragment : FragmentControlRestFormAdd
    {
        /// <summary>
        /// Gets the input text control for specifying the name of the class.
        /// </summary>
        public ControlRestFormItemInputUnique ClassName { get; } = new()
        {
            Name = nameof(Model.Entities.Class.Name),
            Label = "kleenestar.core:class.name.label",
            Placeholder = "kleenestar.core:class.name.placeholder",
            Help = "kleenestar.core:class.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._workspacekey_.UniqueName>()
        };

        /// <summary>
        /// Gets the input text control for specifying the description of the class.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Model.Entities.Class.Description),
            Label = "kleenestar.core:class.description.label",
            Placeholder = "kleenestar.core:class.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Gets the input selection control for the state.
        /// </summary>
        public ControlRestFormItemInputSelection ClassState { get; } = new()
        {
            Name = nameof(Model.Entities.Class.State),
            Label = "kleenestar.core:class.state.label",
            Placeholder = "kleenestar.core:class.state.placeholder",
            Help = "kleenestar.core:class.state.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes.State>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ClassAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(ClassName);
            Add(Description);
            Add(ClassState);

            Mode = TypeRestFormMode.Add;
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
            return base.Render(renderContext, visualTree, Items, null, Uri);
        }
    }
}
