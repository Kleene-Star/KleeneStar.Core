using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Displays assigned permission profiles in read-only mode.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Index>]
    [Cache]
    public sealed class WorkspacePropertyPermissionProfilesFragment : FragmentControlAttribute
    {
        public WorkspacePropertyPermissionProfilesFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Key = "kleenestar.core:workspace.permissionprofiles.label";
        }

        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(keyParameter?.Value);
            var profiles = WorkspacePropertyValueHelper.JoinEnumerable(WorkspacePropertyValueHelper.ReadValue(workspace, "PermissionProfiles"));

            return base.Render(renderContext, visualTree, Key, profiles, Uri, Icon);
        }
    }
}
