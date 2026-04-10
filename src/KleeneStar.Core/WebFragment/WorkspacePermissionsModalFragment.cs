using KleeneStar.Core.WebParameter;
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
    /// Renders the workspace permissions modal content.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Permissions>]
    [Cache]
    public sealed class WorkspacePermissionsModalFragment : FragmentControlText
    {
        public WorkspacePermissionsModalFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Format = TypeFormatText.Markdown;
        }

        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            Text = $"Manage Permissions for '{workspace?.Name}'";

            return base.Render(renderContext, visualTree);
        }
    }
}
