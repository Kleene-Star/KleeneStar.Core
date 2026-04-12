using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents an avatar icon fragment in the navigation bar that provides
    /// sign-in and sign-out functionality for identities.
    /// </summary>
    [Section<SectionAppNavigationSecondary>]
    [Scope<IScopeGeneral>]
    [Scope<IScopeAdmin>]
    [Scope<IScopeStatusPage>]
    [Cache]
    public sealed class IdentityAvatarFragment : FragmentControlDropdown, IFragmentControl<FragmentControlDropdown>, IFragmentControlNavigationItem
    {
        /// <summary>
        /// Gets the context of the fragment.
        /// </summary>
        public IFragmentContext FragmentContext { get; private set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment.
        /// </param>
        public IdentityAvatarFragment(IFragmentContext fragmentContext)
            : base(fragmentContext?.FragmentId?.ToString())
        {
            FragmentContext = fragmentContext;
            Icon = new IconUserCircle();
            AlignmentHorizontal = TypeAlignmentHorizontal.Right;
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var identity = renderContext?.Request?.Identity;
            var isSignedIn = identity is not null;

            if (isSignedIn)
            {
                Text = identity.Name;

                Add(new ControlDropdownItemHeader()
                {
                    Text = identity.Name
                });

                Add(new ControlDropdownItemDivider());

                Add(new ControlDropdownItemLink()
                {
                    Text = I18N.Translate(renderContext, "kleenestar.core:identity.signout.label"),
                    Icon = new IconSignOutAlt(),
                    Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Settings.Identities.Index>()
                        ?.BindParameters(renderContext.Request)
                });
            }
            else
            {
                Text = I18N.Translate(renderContext, "kleenestar.core:identity.signin.label");

                Add(new ControlDropdownItemLink()
                {
                    Text = I18N.Translate(renderContext, "kleenestar.core:identity.signin.label"),
                    Icon = new IconSignInAlt(),
                    Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Settings.Identities.Index>()
                        ?.BindParameters(renderContext.Request)
                });
            }

            return base.Render(renderContext, visualTree);
        }
    }
}
