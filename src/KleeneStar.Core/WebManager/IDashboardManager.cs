using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing dashboards, including adding, retrieving, and removing, as well as
    /// handling dashboard-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing dashboards and events for tracking changes 
    /// to the dashboard collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public interface IDashboardManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when a dashboard is added.
        /// </summary>
        event EventHandler<Dashboard> DashboardAdded;

        /// <summary>
        /// An event that fires when a dashboard is updated.
        /// </summary>
        event EventHandler<Dashboard> DashboardUpdated;

        /// <summary>
        /// An event that fires when a dashboard is removed.
        /// </summary>
        event EventHandler<Dashboard> DashboardRemoved;

        /// <summary>
        /// Returns a dashboard based on its id.
        /// </summary>
        /// <param name="dashboardId">The id of the dashboard.</param>
        /// <returns>The dashboard.</returns>
        Dashboard GetDashboard(Guid dashboardId);

        /// <summary>
        /// Returns a dashboard based on its id.
        /// </summary>
        /// <param name="dashboardId">The id of the dashboard.</param>
        /// <returns>The dashboard.</returns>
        Dashboard GetDashboard(DashboardIdParameter dashboardId);

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
        IEnumerable<Dashboard> GetDashboards(IQuery<Dashboard> query);

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
        IEnumerable<Dashboard> GetDashboards(IQuery<Dashboard> query, IQueryContext context);

        /// <summary>
        /// Adds a dashboard to the manager.
        /// </summary>
        /// <param name="dashboard">The dashboard to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IDashboardManager Add(Dashboard dashboard);

        /// <summary>
        /// Updates a dashboard in the manager.
        /// </summary>
        /// <param name="dashboard">The dashboard to update. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IDashboardManager Update(Dashboard dashboard);

        /// <summary>
        /// Removes the specified dashboard from the manager.
        /// </summary>
        /// <remarks>This method removes the specified dashboard from the manager. If the dashboard does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="dashboardId">The dashboard id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IDashboardManager Remove(Guid dashboardId);
    }
}
