using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.Dashboards._dashboardid_
{
    /// <summary>
    /// Provides the REST API endpoint that returns the widget layout for a specific dashboard,
    /// consumed by the <c>ControlDataDashboard</c> control on the dashboard view page.
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

            foreach (var column in dashboard.Columns)
            {
                yield return new RestApiDashboardColumn
                {
                    Id = column.Id.ToString(),
                    Label = column.Name,
                    Size = column.Size,
                    Widgets = MapWidgets(column.Widgets)
                };
            }
        }

        /// <summary>
        /// Maps the widget entities of a dashboard column to their REST API representations.
        /// </summary>
        /// <param name="widgets">
        /// The collection of widgets associated with a dashboard column.
        /// Must not be null.
        /// </param>
        /// <returns>
        /// A list of REST API dashboard widgets ready for serialization.
        /// </returns>
        private static List<RestApiDashboardWidget> MapWidgets(IEnumerable<Widget> widgets)
        {
            var result = new List<RestApiDashboardWidget>();

            foreach (var widget in widgets)
            {
                result.Add(new RestApiDashboardWidgetInfo
                {
                    Color = "blue",
                    Movable = true,
                    Title = widget.Name,
                    Description = widget.Wql
                });
            }

            return result;
        }
    }
}
