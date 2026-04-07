using KleeneStar.Core.WebAttribute;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebRestApi;

namespace KleeneStar.Core.WWW.Api._1_.Dashboards._dashboardid_
{
    /// <summary>
    /// Serves as the routing parent for dashboard-specific REST API endpoints,
    /// binding the dashboard id path segment.
    /// </summary>
    [Cache]
    [DashboardIdSegment]
    public sealed class Index : IRestApi
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }
    }
}
