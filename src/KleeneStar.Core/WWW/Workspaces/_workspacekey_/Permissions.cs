using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Workspaces._workspacekey_
{
    /// <summary>
    /// Represents the permissions management modal page for a workspace.
    /// </summary>
    [WebIcon<IconUserShield>]
    [Title("kleenestar.core:workspace.permissions.title")]
    [Scope<IScopeGeneral>]
    public sealed class Permissions : IPage<VisualTreeWebApp>, IScope
    {
        /// <summary>
        /// Processing of the resource.
        /// </summary>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
        }
    }
}
