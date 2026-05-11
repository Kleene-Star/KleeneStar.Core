using KleeneStar.Core.WebParameter;
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
    /// Represents a delete form fragment for an identity.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Settings.Identity._identityid_.Delete>]
    [Cache]
    public sealed class IdentityDeleteFormFragment : FragmentControlRestFormDelete
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public IdentityDeleteFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.Index>();
        }

        /// <summary>
        /// Renders the control as HTML.
        /// </summary>
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            var param = renderContext.Request.GetParameter<IdentityIdParameter>();

            return base.Render(renderContext, visualTree);
        }
    }
}
