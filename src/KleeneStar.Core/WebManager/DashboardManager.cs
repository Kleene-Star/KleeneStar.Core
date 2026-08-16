using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Manages dashboards, including adding, retrieving, updating, and removing, as well as
    /// handling dashboard-related events.
    /// </summary>
    /// <remarks>
    /// The class provides methods for managing dashboards and events for tracking changes 
    /// to the dashboard collection. It ensures controlled lifecycle management and 
    /// data integrity for all dashboard entities.
    /// </remarks>
    public sealed class DashboardManager : IDashboardManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// An event that fires when a dashboard is added.
        /// </summary>
        public event EventHandler<Dashboard> DashboardAdded;

        /// <summary>
        /// An event that fires when a dashboard is updated.
        /// </summary>
        public event EventHandler<Dashboard> DashboardUpdated;

        /// <summary>
        /// An event that fires when a dashboard is removed.
        /// </summary>
        public event EventHandler<Dashboard> DashboardRemoved;

        /// <summary>
        /// Gets the collection of names that are reserved and cannot be used for custom dashboards.
        /// </summary>
        public static IEnumerable<string> ReservedDashboardNames =>
        [
            "default", "admin", "system", "assets", "api", "workspace",
            "workspaces", "icons", "setting"
        ];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private DashboardManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns a dashboard based on its id.
        /// </summary>
        /// <param name="dashboardId">The id of the dashboard.</param>
        /// <returns>The dashboard.</returns>
        public Dashboard GetDashboard(Guid dashboardId)
        {
            var query = new Query<Dashboard>()
                .Where(x => x.Id == dashboardId)
                .WithPaging(0, 1);

            return ModelHub.GetDashboards(query)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns a dashboard based on its id.
        /// </summary>
        /// <param name="dashboardId">The id of the dashboard.</param>
        /// <returns>The dashboard.</returns>
        public Dashboard GetDashboard(DashboardIdParameter dashboardId)
        {
            var guid = Guid.TryParse(dashboardId.Value, out Guid id) ? id : Guid.Empty;

            return GetDashboard(guid);
        }

        /// <summary>
        /// Retrieves a collection of dashboards that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned dashboards. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of dashboards that match the given predicate. If no dashboards 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Dashboard> GetDashboards(IQuery<Dashboard> query)
        {
            return ModelHub.GetDashboards(query);
        }

        /// <summary>
        /// Retrieves a collection of dashboards that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned dashboards. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of dashboards that match the given predicate. If no dashboards 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Dashboard> GetDashboards(IQuery<Dashboard> query, IQueryContext context)
        {
            return ModelHub.GetDashboards(query, context as KleeneStarDbContext);
        }

        /// <summary>
        /// Adds a dashboard to the manager.
        /// </summary>
        /// <param name="dashboard">The dashboard to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IDashboardManager Add(Dashboard dashboard)
        {
            ArgumentNullException.ThrowIfNull(dashboard);

            ModelHub.Add(dashboard);

            DashboardAdded?.Invoke(this, dashboard);

            CoreHub.AddNotification("kleenestar.core:notification.title.created", "kleenestar.core:notification.dashboard.created", dashboard);

            return this;
        }

        /// <summary>
        /// Updates a dashboard in the manager.
        /// </summary>
        /// <param name="dashboard">The dashboard to update. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IDashboardManager Update(Dashboard dashboard)
        {
            ArgumentNullException.ThrowIfNull(dashboard);

            ModelHub.Update(dashboard);

            DashboardUpdated?.Invoke(this, dashboard);

            CoreHub.AddNotification("kleenestar.core:notification.title.updated", "kleenestar.core:notification.dashboard.updated", dashboard);

            return this;
        }

        /// <summary>
        /// Removes the specified dashboard from the manager.
        /// </summary>
        /// <remarks>This method removes the specified dashboard from the manager. If the dashboard does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="dashboardId">The dashboard id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IDashboardManager Remove(Guid dashboardId)
        {
            var dashboard = GetDashboard(dashboardId);

            if (dashboard is not null)
            {
                ModelHub.Remove(dashboard);
                DashboardRemoved?.Invoke(this, dashboard);

                CoreHub.AddNotification("kleenestar.core:notification.title.deleted", "kleenestar.core:notification.dashboard.deleted", dashboard);
            }

            return this;
        }

        /// <summary>
        /// Applies a column-only layout change (add, rename, resize, recolor, reorder, delete) to a
        /// dashboard while preserving the widgets of the surviving columns.
        /// </summary>
        /// <param name="dashboardId">The id of the dashboard to update.</param>
        /// <param name="columns">
        /// The desired columns in their target order. Widgets on these instances are ignored. Must not
        /// be null.
        /// </param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IDashboardManager SetColumns(Guid dashboardId, IReadOnlyList<DashboardColumn> columns)
        {
            ArgumentNullException.ThrowIfNull(columns);

            ModelHub.SetDashboardColumns(dashboardId, columns);

            var dashboard = GetDashboard(dashboardId);

            if (dashboard is not null)
            {
                DashboardUpdated?.Invoke(this, dashboard);
            }

            return this;
        }

        /// <summary>
        /// Applies a full board update (a widget being added, deleted, reconfigured or moved) to a
        /// dashboard, rebuilding the widgets of every column from the desired state.
        /// </summary>
        /// <param name="dashboardId">The id of the dashboard to update.</param>
        /// <param name="columns">
        /// The desired columns, each carrying the widgets it should hold, in their target order. Must
        /// not be null.
        /// </param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IDashboardManager SetBoard(Guid dashboardId, IReadOnlyList<DashboardColumn> columns)
        {
            ArgumentNullException.ThrowIfNull(columns);

            ModelHub.SetDashboardBoard(dashboardId, columns);

            var dashboard = GetDashboard(dashboardId);

            if (dashboard is not null)
            {
                DashboardUpdated?.Invoke(this, dashboard);
            }

            return this;
        }

        /// <summary>
        /// Release of unmanaged resources reserved during use.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
