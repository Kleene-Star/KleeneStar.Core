using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebFragment;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a control panel fragment that provides a login form for user authentication via REST API endpoints.
    /// </summary>
    [Title("kleenestar.core:login.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Session.Index>]
    [Cache]
    public sealed class LoginFormFragment : FragmentControlPanel
    {
        /// <summary>
        /// Gets the login form control used to authenticate users via REST API endpoints.
        /// </summary>
        public ControlRestLogin LoginForm { get; } = new()
        {

        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public LoginFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(LoginForm);
        }
    }
}
