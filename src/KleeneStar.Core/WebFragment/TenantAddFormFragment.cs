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
    /// Represents a add form fragment for a tenant.
    /// </summary>
    [Title("kleenestar.core:setting.tenant.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Settings.Tenants.Add>]
    [Cache]
    public sealed class TenantAddFormFragment : FragmentControlRestFormAdd
    {
        /// <summary>
        /// Gets the input text control for specifying the name of the tenant.
        /// </summary>
        public ControlRestFormItemInputUnique TenantName { get; } = new()
        {
            Name = nameof(Model.Entities.Tenant.Name),
            Label = "kleenestar.core:setting.tenant.name.label",
            Placeholder = "kleenestar.core:setting.tenant.name.placeholder",
            Help = "kleenestar.core:setting.tenant.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Tenants.UniqueName>()
        };

        /// <summary>
        /// Gets the input text control for specifying the description of the tenant.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Model.Entities.Tenant.Description),
            Label = "kleenestar.core:setting.tenant.description.label",
            Placeholder = "kleenestar.core:setting.tenant.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Gets the input selection control for the state.
        /// </summary>
        public ControlRestFormItemInputSelection TenantState { get; } = new()
        {
            Name = nameof(Model.Entities.Tenant.State),
            Label = "kleenestar.core:setting.tenant.state.label",
            Placeholder = "kleenestar.core:setting.tenant.state.placeholder",
            Help = "kleenestar.core:setting.tenant.state.help",
            StickySelection = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Tenants.State>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public TenantAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(TenantName);
            Add(Description);
            Add(TenantState);

            Mode = TypeRestFormMode.Add;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Tenants.Index>();
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
