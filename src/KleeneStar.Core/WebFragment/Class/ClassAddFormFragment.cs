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

namespace KleeneStar.Core.WebFragment.Class
{
    /// <summary>
    /// Represents a add form fragment for a class.
    /// </summary>
    [Title("kleenestar.core:class.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Classes._workspacekey_.Add>]
    [Cache]
    public sealed class ClassAddFormFragment : FragmentControlDataFormAdd
    {
        /// <summary>
        /// Gets the input text control for specifying the name of the class.
        /// </summary>
        public ControlDataFormItemInputUnique ClassName { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Class.Name),
            Label = _ => "kleenestar.core:class.name.label",
            Placeholder = _ => "kleenestar.core:class.name.placeholder",
            Help = _ => "kleenestar.core:class.name.help",
            Required = _ => true,
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._workspacekey_.UniqueName>().ToString())};

        /// <summary>
        /// Gets the input text control for specifying the description of the class.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = _ => nameof(Model.Entities.Class.Description),
            Label = _ => "kleenestar.core:class.description.label",
            Placeholder = _ => "kleenestar.core:class.description.placeholder",
            Format = _ => TypeEditTextFormat.Wysiwyg,
            Required = _ => false
        };

        /// <summary>
        /// Gets the input selection control for the inherited class.
        /// </summary>
        public ControlDataFormItemInputSelection InheritedSelection { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Class.InheritedId),
            Label = _ => "kleenestar.core:class.inherited.label",
            Placeholder = _ => "kleenestar.core:class.inherited.placeholder",
            Help = _ => "kleenestar.core:class.inherited.help",
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._workspacekey_.Inherited>().ToString())};

        /// <summary>
        /// Gets the checkbox control for the abstract flag.
        /// </summary>
        public ControlFormItemInputCheck ClassIsAbstract { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Class.IsAbstract),
            Label = _ => "kleenestar.core:class.isabstract.label",
            Help = _ => "kleenestar.core:class.isabstract.help",
            Layout = _ => TypeLayoutCheck.Switch
        };

        /// <summary>
        /// Gets the input selection control for the parent class.
        /// </summary>
        public ControlDataFormItemInputSelection ParentSelection { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Class.ParentId),
            Label = _ => "kleenestar.core:class.parent.label",
            Placeholder = _ => "kleenestar.core:class.parent.placeholder",
            Help = _ => "kleenestar.core:class.parent.help",
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._workspacekey_.Parent>().ToString())};

        /// <summary>
        /// Gets the tag input control for specifying the allowed children classes.
        /// </summary>
        public ControlFormItemInputTag AllowedChildren { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Class.AllowedChildren),
            Label = _ => "kleenestar.core:class.allowedchildren.label",
            Placeholder = _ => "kleenestar.core:class.allowedchildren.placeholder",
            Help = _ => "kleenestar.core:class.allowedchildren.help"
        };

        /// <summary>
        /// Gets the input selection control for the access modifier.
        /// </summary>
        public ControlDataFormItemInputSelection AccessModifierSelection { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Class.AccessModifier),
            Label = _ => "kleenestar.core:class.accessmodifier.label",
            Placeholder = _ => "kleenestar.core:class.accessmodifier.placeholder",
            Help = _ => "kleenestar.core:class.accessmodifier.help",
            StickySelection = _ => true,
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes.AccessModifier>().ToString())};

        /// <summary>
        /// Gets the checkbox control for the sealed flag.
        /// </summary>
        public ControlFormItemInputCheck ClassSealed { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Class.Sealed),
            Label = _ => "kleenestar.core:class.sealed.label",
            Help = _ => "kleenestar.core:class.sealed.help",
            Layout = _ => TypeLayoutCheck.Switch
        };

        /// <summary>
        /// Gets the input selection control for the state.
        /// </summary>
        public ControlDataFormItemInputSelection ClassState { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Class.State),
            Label = _ => "kleenestar.core:class.state.label",
            Placeholder = _ => "kleenestar.core:class.state.placeholder",
            Help = _ => "kleenestar.core:class.state.help",
            StickySelection = _ => true,
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes.State>().ToString())};

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ClassAddFormFragment(IFragmentContext fragmentContext)
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

            this.DataService<global::KleeneStar.Core.WWW.Api._1_.Classes.Index>();
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

