using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebFragment.Workspace
{
    /// <summary>
    /// Empty-state placeholder rendered when a workspace has no associated
    /// content yet (no classes, no objects, no forms). Uses the standard
    /// <see cref="ControlEmptyState"/> pattern from the WebExpress.WebUI
    /// framework so the page still communicates what is available.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Workspaces.Index>]
    [Cache]
    public sealed class WorkspaceEmptyStateFragment : FragmentControlEmptyState
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment. Cannot be null.
        /// </param>
        public WorkspaceEmptyStateFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconInbox();
            Title = _ => "kleenestar.core:workspace.empty.title";
            Message = _ => "kleenestar.core:workspace.empty.message";
        }
    }
}
