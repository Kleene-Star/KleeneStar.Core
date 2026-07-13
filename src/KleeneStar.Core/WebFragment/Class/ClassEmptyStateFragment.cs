using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebFragment.Class
{
    /// <summary>
    /// Empty-state placeholder rendered when a class has no fields, forms or
    /// objects yet. Uses the standard <see cref="ControlEmptyState"/> pattern
    /// from the WebExpress.WebUI framework so the page still communicates
    /// the available next step.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Classes._workspacekey_.Index>]
    [Cache]
    public sealed class ClassEmptyStateFragment : FragmentControlEmptyState
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment. Cannot be null.
        /// </param>
        public ClassEmptyStateFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconInbox();
            Title = _ => "kleenestar.core:class.empty.title";
            Message = _ => "kleenestar.core:class.empty.message";
        }
    }
}
