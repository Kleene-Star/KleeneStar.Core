using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing tenants, including adding, retrieving, and removing, as well as
    /// handling tenant-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing tenants and events for tracking changes 
    /// to the tenant collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public interface ITenantManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when an tenant is added.
        /// </summary>
        event EventHandler<Tenant> TenantAdded;

        /// <summary>
        /// An event that fires when an tenant is udpated.
        /// </summary>
        event EventHandler<Tenant> TenantUpdated;

        /// <summary>
        /// An event that fires when an tenant is removed.
        /// </summary>
        event EventHandler<Tenant> TenantRemoved;

        /// <summary>
        /// Returns a tenant based on its id.
        /// </summary>
        /// <param name="tenantId">The id of the tenant.</param>
        /// <returns>The tenant.</returns>
        Tenant GetTenant(Guid tenantId);

        /// <summary>
        /// Returns a tenant based on its id.
        /// </summary>
        /// <param name="tenantId">The id of the tenant.</param>
        /// <returns>The tenant.</returns>
        Tenant GetTenant(TenantIdParameter tenantId);

        /// <summary>
        /// Retrieves a collection of tenants that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned tenants. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of tenants that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Tenant> GetTenants(IQuery<Tenant> query);

        /// <summary>
        /// Retrieves a collection of tenants that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned tenants. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of tenants that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Tenant> GetTenants(IQuery<Tenant> query, IQueryContext context);

        /// <summary>
        /// Adds a tenant to the manager.
        /// </summary>
        /// <param name="tenantEntity">The tenant to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        ITenantManager Add(Tenant tenantEntity);

        /// <summary>
        /// Update a tenant to the manager.
        /// </summary>
        /// <param name="tenantEntity">The tenant to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        ITenantManager Update(Tenant tenantEntity);

        /// <summary>
        /// Removes the specified tenant from the manager.
        /// </summary>
        /// <remarks>This method removes the specified tenant from the manager. If the tenant does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="tenantId">The tenant id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        ITenantManager Remove(Guid tenantId);
    }
}
