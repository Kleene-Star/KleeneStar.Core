using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Identity
{
    /// <summary>
    /// Represents a add form fragment for an identity.
    /// </summary>
    [Title("kleenestar.core:setting.identity.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Settings.Identities.Add>]
    [Cache]
    public sealed class IdentityAddFormFragment : FragmentControlRestFormAdd
    {
        /// <summary>
        /// Gets the input text control for specifying the name of the identity.
        /// </summary>
        public ControlRestFormItemInputUnique IdentityName { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Identity.Name),
            Label = _ => "kleenestar.core:setting.identity.name.label",
            Placeholder = _ => "kleenestar.core:setting.identity.name.placeholder",
            Help = _ => "kleenestar.core:setting.identity.name.help",
            Required = _ => true,
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.UniqueName>()
        };

        /// <summary>
        /// Gets the input text control for specifying the email of the identity.
        /// </summary>
        public ControlFormItemInputText Email { get; } = new ControlFormItemInputText()
        {
            Name = _ => nameof(Model.Entities.Identity.Email),
            Label = _ => "kleenestar.core:setting.identity.email.label",
            Placeholder = _ => "kleenestar.core:setting.identity.email.placeholder",
            Required = _ => false
        };

        /// <summary>
        /// Gets the input selection control for the state.
        /// </summary>
        public ControlRestFormItemInputSelection IdentityState { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Identity.State),
            Label = _ => "kleenestar.core:setting.identity.state.label",
            Placeholder = _ => "kleenestar.core:setting.identity.state.placeholder",
            Help = _ => "kleenestar.core:setting.identity.state.help",
            StickySelection = _ => true,
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.State>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public IdentityAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(IdentityName);
            Add(Email);
            Add(IdentityState);

            Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.Index>();
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
