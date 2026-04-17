using WebExpress.WebApp.WebCondition;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a dropdown menu item that provides a logout link for the user interface.
    /// </summary>
    /// <remarks>This fragment is intended for use in application sections where a logout option should be
    /// presented to authenticated users. It integrates with the component hub to determine the appropriate logout URI
    /// and renders a standardized logout control. The fragment is typically used within avatar or user profile
    /// dropdowns.</remarks>
    [Section<SectionAppAvatarPrimary>]
    [Scope<IScopeGeneral>]
    [Scope<IScopeAdmin>]
    [Scope<IScopeStatusPage>]
    [Condition<ConditionLogin>]
    [Cache]
    public sealed class LogoutLinkFragment : FragmentControlDropdownItemLinkLogout
    {
        private readonly IComponentHub _componentHub;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="componentHub">The component hub used to manage components.</param>
        /// <param name="fragmentContext">The context in which the fragment is used.</param>
        public LogoutLinkFragment(IComponentHub componentHub, IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            _componentHub = componentHub;
        }

        /// <summary>
        /// Convert the control to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the control is rendered.</param>
        /// <param name="visualTree">The visual tree representing the control's structure.</param>
        /// <returns>An HTML node representing the rendered control.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var logoutUri = _componentHub.SitemapManager.GetUri<global::KleeneStar.Core.WWW.Api._1_.Session>(renderContext?.PageContext.ApplicationContext);

            return base.Render(renderContext, visualTree, logoutUri);
        }
    }
}
