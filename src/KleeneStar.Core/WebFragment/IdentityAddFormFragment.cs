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
    /// Represents an add form fragment for an identity.
    /// </summary>
    [Title("kleenestar.core:setting.identity.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Settings.Identities.Add>]
    [Cache]
    public sealed class IdentityAddFormFragment : FragmentControlRestFormAdd
    {
        /// <summary>
        /// Gets the input for the identity name.
        /// </summary>
        public ControlRestFormItemInputUnique IdentityName { get; } = new()
        {
            Name = nameof(Model.Entities.Identity.Name),
            Label = "kleenestar.core:setting.identity.name.label",
            Placeholder = "kleenestar.core:setting.identity.name.placeholder",
            Help = "kleenestar.core:setting.identity.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.UniqueName>()
        };

        /// <summary>
        /// Gets the input for the email.
        /// </summary>
        public ControlFormItemInputText Email { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Model.Entities.Identity.Email),
            Label = "kleenestar.core:setting.identity.email.label",
            Placeholder = "kleenestar.core:setting.identity.email.placeholder",
            Required = false
        };

        /// <summary>
        /// Gets the input for the state.
        /// </summary>
        public ControlRestFormItemInputSelection IdentityState { get; } = new()
        {
            Name = nameof(Model.Entities.Identity.State),
            Label = "kleenestar.core:setting.identity.state.label",
            Placeholder = "kleenestar.core:setting.identity.state.placeholder",
            Help = "kleenestar.core:setting.identity.state.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.State>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public IdentityAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(IdentityName);
            Add(Email);
            Add(IdentityState);

            Mode = TypeRestFormMode.Add;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.Index>();
        }

        /// <summary>
        /// Renders the control as HTML.
        /// </summary>
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree, Items, null, Uri);
        }
    }
}
