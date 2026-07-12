using WebExpress.WebApp.WebCondition;
using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a control panel fragment that provides a login form for user authentication via 
    /// REST API endpoints.
    /// </summary>
    [Section<SectionBodySecondary>]
    [Scope<IScopeGeneral>]
    [Scope<IScopeAdmin>]
    [Scope<IScopeStatusPage>]
    [Condition<ConditionLogout>]
    [Cache]
    public sealed class LoginModalFragment : ControlModalRemotePage, IFragmentControl<ControlModalRemotePage>
    {
        /// <summary>
        /// Gets the context of the fragment.
        /// </summary>
        public IFragmentContext FragmentContext { get; }

        /// <summary>
        /// Initializes a new instance of the class with the well-known
        /// <c>modal-login</c> id, so the avatar Login link can target it.
        /// The id <c>modal-form</c> is reserved for <see cref="ModalFormFragment"/>
        /// (REST add/edit/clone/delete forms); sharing it would race with the
        /// form modal when the user is logged out and hide the form modal body.
        /// </summary>
        /// <param name="fragmentContext">The context in which the fragment is used.</param>
        public LoginModalFragment(IFragmentContext fragmentContext)
            : base("modal-login")
        {
            FragmentContext = fragmentContext;
            Header = _ => "webexpress.webapp:login.label";
            Selector = _ => "#login";
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragment, or null when conditions are not met.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            return base.Render(renderContext, visualTree);
        }
    }
}
