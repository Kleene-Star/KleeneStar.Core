using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a sidebar icon fragment for a workspace, providing rendering and 
    /// editing capabilities within the workspace sidebar.
    /// </summary>
    [Section<SectionSidebarPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Classes._workspacekey_.Index>]
    [Policy<WorkspaceViewPolicy>]
    [Cache]
    public sealed class WorkspaceSidebarIconFragment : FragmentControlSidebarItemIcon
    {
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        /// <param name="workspaceManager">
        /// The workspace manager used to retrieve workspace information. Cannot be null.
        /// </param>
        public WorkspaceSidebarIconFragment(IFragmentContext fragmentContext, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _workspaceManager = workspaceManager;

            IconEdit = _ => true;
            PrimaryAction = _ => new ActionModal("modal-form");
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(keyParameter?.Value);
            var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Avatar>()?
                .BindParameters(renderContext.Request);
            var primaryAction = new ActionModal("modal-form", uri);

            return base.Render(renderContext, visualTree);
        }
    }
}
