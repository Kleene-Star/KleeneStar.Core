using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Displays whether a workspace is sealed in read-only mode.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Index>]
    [Cache]
    public sealed class WorkspacePropertySealedFragment : FragmentControlAttribute
    {
        public WorkspacePropertySealedFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Key = "kleenestar.core:workspace.sealed.label";
        }

        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(keyParameter?.Value);
            var sealedState = WorkspacePropertyValueHelper.ReadBoolean(workspace, "Sealed")
                ? I18N.Translate(renderContext, "kleenestar.core:workspace.property.yes")
                : I18N.Translate(renderContext, "kleenestar.core:workspace.property.no");

            return base.Render(renderContext, visualTree, Key, sealedState, Uri, Icon);
        }
    }
}
