using KleeneStar.Core.WebParameter.Workspace;
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
    /// Represents a sidebar item link fragment that displays the 'All' quick filter option in the workspace sidebar.
    /// </summary>
    [Section<SectionSidebarPreferences>]
    [Scope<WWW.Workspaces._key_.Index>]
    [Cache]
    public sealed class WorkspaceSidebarIconFragment : FragmentControlSidebarItemIcon
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public WorkspaceSidebarIconFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
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
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(keyParameter?.Value);
            var uri = CoreHub.GetUri<WWW.Workspaces._key_.Avatar>()?
                .SetParameters
                (
                    new KeyParameter(workspace.Key)
                );

            return base.Render(renderContext, visualTree, workspace?.Icon, uri);
        }
    }
}
