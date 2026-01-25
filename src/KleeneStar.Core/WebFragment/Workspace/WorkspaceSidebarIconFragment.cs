using KleeneStar.Core.WebParameter.Workspace;
using KleeneStar.Core.Workspace;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workspace
{
    /// <summary>
    /// Represents a sidebar icon fragment for a workspace, providing rendering and 
    /// editing capabilities within the workspace sidebar.
    /// </summary>
    [Section<SectionSidebarPreferences>]
    [Scope<WWW.Workspaces._key_.Index>]
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

            IconEdit = true;
            Modal = new ModalTarget("modal-form");
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<KeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(keyParameter?.Value);
            var uri = CoreHub.GetUri<WWW.Workspaces._key_.Avatar>()?
                .SetParameters
                (
                    new KeyParameter(workspace.Key)
                );

            return base.Render(renderContext, visualTree, workspace?.Icon, uri);
        }
    }
}
