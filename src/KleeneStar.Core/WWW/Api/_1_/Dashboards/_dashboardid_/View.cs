using KleeneStar.Core.WebParameter;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.Dashboards._dashboardid_
{
    /// <summary>
    /// Provides the REST API endpoint that returns the widget layout for a specific dashboard,
    /// consumed by the <c>ControlRestDashboard</c> control on the dashboard view page.
    /// </summary>
    [Title("kleenestar.core:dashboard.view.label")]
    [Cache]
    public sealed class View : RestApiDashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public View()
        {
        }

        /// <summary>
        /// Retrieves the column and widget layout for the dashboard identified by the
        /// <c>dashboardId</c> path segment in the current request.
        /// </summary>
        /// <remarks>
        /// Widget configuration is not yet stored in the model. The method validates that the
        /// dashboard exists but always returns an empty column set until widget persistence
        /// is implemented.
        /// </remarks>
        /// <param name="request">The current HTTP request. Cannot be null.</param>
        /// <returns>
        /// An empty enumerable. Returns without yielding when no dashboard matches the id.
        /// </returns>
        protected override IEnumerable<RestApiDashboardColumn> RetrieveColumns(IRequest request)
        {
            var dashboardParameter = request.GetParameter<DashboardIdParameter>();
            var dashboard = CoreHub.DashboardManager.GetDashboard(dashboardParameter);

            if (dashboard == null)
            {
                yield break;
            }

            // TODO: Yield RestApiDashboardColumn entries once widget configuration is
            // persisted in the model.
            yield break;
        }
    }
}
