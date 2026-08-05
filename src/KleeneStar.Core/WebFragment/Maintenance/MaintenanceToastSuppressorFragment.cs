using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebFragment;

namespace KleeneStar.Core.WebFragment.Maintenance
{
    /// <summary>
    /// Collapses the toast bar for as long as no maintenance notice is being made.
    /// </summary>
    /// <remarks>
    /// The framework emits the toast bar whenever a fragment is registered for its section, because
    /// the decision is made from the presence of the fragments and not from what they rendered.
    /// A fragment gated by a condition can therefore only empty the bar, never remove it, and
    /// <see cref="MaintenanceToastFragment"/> would leave an empty coloured strip at the top of
    /// every page while nothing is being announced.
    ///
    /// This fragment is the exact complement of that one: it occupies the same section whenever the
    /// other stays silent and hides the bar the framework has already committed to. It is a
    /// workaround with a known end -- once the framework renders its toast children first and drops
    /// the bar when all of them yield nothing, this fragment and
    /// <see cref="MaintenanceNoNoticeCondition"/> can be deleted without touching anything else.
    ///
    /// The rule is written against the bar rather than against the maintenance fragment, so it also
    /// holds once further fragments join the section.
    /// </remarks>
    [Section<SectionToastNotificationPrimary>]
    [Scope<IScopeGeneral>]
    [Scope<IScopeAdmin>]
    [Condition<MaintenanceNoNoticeCondition>]
    [Cache]
    public sealed class MaintenanceToastSuppressorFragment : FragmentControlHtml
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context in which the fragment is used.</param>
        public MaintenanceToastSuppressorFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Html = _ => "<style>#wx-toast{display:none}</style>";
        }
    }
}
