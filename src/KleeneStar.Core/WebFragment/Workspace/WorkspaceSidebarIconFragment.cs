using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;
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
            Icon = renderContext => GetIcon(renderContext);
            PrimaryAction = renderContext => new ActionModal("modal-form", GetUri(renderContext));
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Gets the URI for the Avatar endpoint with bound request parameters.
        /// </summary>
        /// <param name="renderContext">
        /// The render control context containing the request.
        /// </param>
        /// <returns>
        /// The URI for the Avatar endpoint with bound parameters, or
        /// <see langword="null"/> if the URI cannot be retrieved.
        /// </returns>
        private static IUri GetUri(IRenderControlContext renderContext)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();

            return CoreHub.GetUri<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Avatar>()?
                .BindParameters(keyParameter);
        }

        /// <summary>
        /// Retrieves the icon associated with the workspace specified in the current
        /// render context.
        /// </summary>
        /// <param name="renderContext">
        /// The rendering context containing the request parameters used to identify
        /// the workspace.
        /// </param>
        /// <returns>
        /// The icon for the specified workspace, or null if the workspace is not found
        /// or does not have an associated icon.
        /// </returns>
        private IIcon GetIcon(IRenderControlContext renderContext)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            return workspace?.Icon;
        }
    }
}
