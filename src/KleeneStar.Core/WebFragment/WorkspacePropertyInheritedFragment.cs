using KleeneStar.Core.WebParameter;
using System;
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
    /// Displays the inherited workspace in read-only mode.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Index>]
    [Cache]
    public sealed class WorkspacePropertyInheritedFragment : FragmentControlAttribute
    {
        public WorkspacePropertyInheritedFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Key = "kleenestar.core:workspace.inherited.label";
        }

        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            var inheritedName = I18N.Translate(renderContext, "kleenestar.core:workspace.property.none");
            var inheritedId = WorkspacePropertyValueHelper.ReadGuid(workspace, "InheritedId");

            if (inheritedId.HasValue)
            {
                inheritedName = CoreHub.WorkspaceManager.GetWorkspace(inheritedId.Value)?.Name ?? inheritedName;
            }
            else
            {
                inheritedName = WorkspacePropertyValueHelper.ReadString(workspace, "Inherited") ?? inheritedName;
            }

            return base.Render(renderContext, visualTree, Key, inheritedName, Uri, Icon);
        }
    }
}
