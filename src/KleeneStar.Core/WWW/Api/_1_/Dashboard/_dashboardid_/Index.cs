using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Dashboard._dashboardid_
{
    /// <summary>
    /// Provides the dashboard display data via a REST API for the dashboard view.
    /// </summary>
    [Cache]
    [DashboardIdSegment]
    public sealed class Index : RestApiDashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Retrieves the columns for the specified dashboard.
        /// </summary>
        /// <param name="request">The HTTP request providing the dashboard id parameter.</param>
        /// <returns>
        /// An enumerable collection of dashboard columns. Returns an empty collection if 
        /// the dashboard is not found.
        /// </returns>
        protected override IEnumerable<RestApiDashboardColumn> RetrieveColumns(IRequest request)
        {
            var dashboardIdParam = request.GetParameter<DashboardIdParameter>();
            var dashboard = CoreHub.DashboardManager.GetDashboard(dashboardIdParam);

            Title = dashboard?.Name;

            return [];
        }
    }
}
