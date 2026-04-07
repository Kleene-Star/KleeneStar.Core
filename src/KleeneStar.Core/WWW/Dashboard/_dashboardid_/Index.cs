using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebUri;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Dashboard._dashboardid_
{
    /// <summary>
    /// Provides functionality for displaying a single dashboard.
    /// </summary>
    [WebIcon<IconGauge>]
    [DashboardIdSegment]
    [Scope<IScopeGeneral>]
    [Domain<Model.Entities.Dashboard>]
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
            var dashboardParameter = renderContext.Request.GetParameter<DashboardIdParameter>();
            var dashboard = CoreHub.DashboardManager.GetDashboard(dashboardParameter);

            var uri = renderContext.PageContext.ApplicationContext.Route
                .Concat(new UriPathSegmentConstant("dashboards")
                {
                    Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Dashboards.Index>()
                })
                .Concat(new DashboardIdUriPathSegmentVariable<DashboardIdParameter>()
                {
                    Uri = renderContext.Request.Uri
                })
                .ToUri()
                .BindParameters(renderContext.Request);

            visualTree.BreadcrumbUri = uri;

            visualTree.Title = dashboard?.Name;
            visualTree.Content.MainPanel.Headline.Title = dashboard?.Name;
        }
    }
}
