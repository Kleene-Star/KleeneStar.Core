using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebFragment;

namespace KleeneStar.Core.WebFragment.Maintenance
{
    /// <summary>
    /// Shows the maintenance instruction text as a toast at the top of every page.
    /// </summary>
    /// <remarks>
    /// The fragment is gated by <see cref="MaintenanceNoticeCondition"/>, so it contributes nothing
    /// while no announcement is active. The text is read on each render rather than captured in the
    /// constructor, because the fragment instance is cached while the notice behind it is not.
    /// </remarks>
    [Section<SectionToastNotificationPrimary>]
    [Scope<IScopeGeneral>]
    [Scope<IScopeAdmin>]
    [Condition<MaintenanceNoticeCondition>]
    [Cache]
    public sealed class MaintenanceToastFragment : FragmentControlText
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context in which the fragment is used.</param>
        public MaintenanceToastFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            // the text is authored by an administrator rather than shipped with the plugin; the
            // control's translation lookup leaves such a string untouched and renders it verbatim
            Text = _ => CoreHub.MaintenanceManager?.GetMaintenance()?.Message;
        }
    }
}
