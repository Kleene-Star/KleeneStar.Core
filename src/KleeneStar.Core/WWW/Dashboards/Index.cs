using KleeneStar.Core.WebIcon;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;

namespace KleeneStar.Core.WWW.Dashboards
{
    /// <summary>
    /// Represents the main dashboard management page within the kleenestar web application.
    /// </summary>
    [WebIcon<DashboardIcon>]
    [Title("kleenestar.core:dashboard.manage.title")]
    [Scope<IScopeGeneral>]
    [Domain<Model.Entities.Dashboard>]
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
