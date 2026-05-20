using KleeneStar.Core.WebAttribute;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Settings.Tenant._tenantid_
{
    /// <summary>
    /// Represents the main page for a tenant within the class.
    /// </summary>
    [WebIcon<IconList>]
    [Title("kleenestar.core:setting.tenant.manage.label")]
    [TenantIdSegment]
    [Scope<IScopeGeneral>]
    [Domain<Model.Entities.Tenant>]
    [Cache]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
        }
    }
}
