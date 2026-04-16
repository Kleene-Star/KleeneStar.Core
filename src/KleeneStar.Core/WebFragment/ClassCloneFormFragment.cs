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
    /// Represents a clone form fragment for a class.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Class._classid_.Clone>]
    [Cache]
    public sealed class ClassCloneFormFragment : FragmentControlRestFormClone
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
        /// Gets the input selection control for the inherited class.
        /// </summary>
        public ControlRestFormItemInputSelection InheritedSelection { get; } = new()
        {
            Name = nameof(Model.Entities.Class.InheritedId),
            Label = "kleenestar.core:class.inherited.label",
            Placeholder = "kleenestar.core:class.inherited.placeholder",
            Help = "kleenestar.core:class.inherited.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._workspacekey_.Inherited>()
        };

        /// <summary>
        /// Gets the checkbox control for the abstract flag.
        /// </summary>
        public ControlFormItemInputCheck ClassIsAbstract { get; } = new()
        {
            Name = nameof(Model.Entities.Class.IsAbstract),
            Label = "kleenestar.core:class.isabstract.label",
            Help = "kleenestar.core:class.isabstract.help"
        };

        /// <summary>
        /// Gets the input selection control for the parent class.
        /// </summary>
        public ControlRestFormItemInputSelection ParentSelection { get; } = new()
        {
            Name = nameof(Model.Entities.Class.ParentId),
            Label = "kleenestar.core:class.parent.label",
            Placeholder = "kleenestar.core:class.parent.placeholder",
            Help = "kleenestar.core:class.parent.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._workspacekey_.Parent>()
        };

        /// <summary>
        /// Gets the tag input control for specifying the allowed children classes.
        /// </summary>
        public ControlFormItemInputTag AllowedChildren { get; } = new()
        {
            Name = nameof(Model.Entities.Class.AllowedChildren),
            Label = "kleenestar.core:class.allowedchildren.label",
            Placeholder = "kleenestar.core:class.allowedchildren.placeholder",
            Help = "kleenestar.core:class.allowedchildren.help"
        };

        /// <summary>
        /// Gets the input selection control for the access modifier.
        /// </summary>
        public ControlRestFormItemInputSelection AccessModifierSelection { get; } = new()
        {
            Name = nameof(Model.Entities.Class.AccessModifier),
            Label = "kleenestar.core:class.accessmodifier.label",
            Placeholder = "kleenestar.core:class.accessmodifier.placeholder",
            Help = "kleenestar.core:class.accessmodifier.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes.AccessModifier>()
        };

        /// <summary>
        /// Gets the checkbox control for the sealed flag.
        /// </summary>
        public ControlFormItemInputCheck ClassSealed { get; } = new()
        {
            Name = nameof(Model.Entities.Class.Sealed),
            Label = "kleenestar.core:class.sealed.label",
            Help = "kleenestar.core:class.sealed.help"
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
        public ClassCloneFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(ClassName);
            Add(Description);
            Add(InheritedSelection);
            Add(ClassIsAbstract);
            Add(ParentSelection);
            Add(AllowedChildren);
            Add(AccessModifierSelection);
            Add(ClassSealed);
            Add(ClassState);

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
