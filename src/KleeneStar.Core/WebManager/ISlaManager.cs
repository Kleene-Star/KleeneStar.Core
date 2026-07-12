using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing SLA policies attached to a <see cref="Class"/>.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface should ensure thread safety if used in a multi-threaded
    /// environment.
    /// </remarks>
    public interface ISlaManager : IComponentManager
    {
        /// <summary>
        /// Raised when a new SLA policy has been added.
        /// </summary>
        event EventHandler<SlaPolicy> SlaAdded;

        /// <summary>
        /// Raised when an SLA policy has been updated.
        /// </summary>
        event EventHandler<SlaPolicy> SlaUpdated;

        /// <summary>
        /// Raised when an SLA policy has been removed.
        /// </summary>
        event EventHandler<SlaPolicy> SlaRemoved;

        /// <summary>
        /// Returns a single SLA policy by its id, including all related targets, scope rules,
        /// and escalation levels.
        /// </summary>
        /// <param name="slaId">The id of the policy.</param>
        /// <returns>The policy, or <c>null</c> when it does not exist.</returns>
        SlaPolicy GetSla(Guid slaId);

        /// <summary>
        /// Returns a single SLA policy by its id parameter.
        /// </summary>
        /// <param name="slaId">The id parameter.</param>
        /// <returns>The policy, or <c>null</c> when it does not exist.</returns>
        SlaPolicy GetSla(SlaIdParameter slaId);

        /// <summary>
        /// Returns all SLA policies attached to the specified class.
        /// </summary>
        /// <param name="classId">The class id parameter.</param>
        /// <returns>The policies attached to the class.</returns>
        IEnumerable<SlaPolicy> GetSlas(ClassIdParameter classId);

        /// <summary>
        /// Returns all SLA policies attached to the specified class.
        /// </summary>
        /// <param name="classId">The class id.</param>
        /// <returns>The policies attached to the class.</returns>
        IEnumerable<SlaPolicy> GetSlas(Guid classId);

        /// <summary>
        /// Returns SLA policies satisfying the supplied query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The matching policies.</returns>
        IEnumerable<SlaPolicy> GetSlas(IQuery<SlaPolicy> query);

        /// <summary>
        /// Returns SLA policies satisfying the supplied query in the supplied query context.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The matching policies.</returns>
        IEnumerable<SlaPolicy> GetSlas(IQuery<SlaPolicy> query, IQueryContext context);

        /// <summary>
        /// Adds an SLA policy to the manager.
        /// </summary>
        /// <param name="policy">The policy to add.</param>
        /// <returns>The current instance to allow chaining.</returns>
        ISlaManager Add(SlaPolicy policy);

        /// <summary>
        /// Updates an existing SLA policy.
        /// </summary>
        /// <param name="policy">The policy to update.</param>
        /// <returns>The current instance to allow chaining.</returns>
        ISlaManager Update(SlaPolicy policy);

        /// <summary>
        /// Removes the SLA policy identified by the supplied id.
        /// </summary>
        /// <param name="slaId">The id of the policy to remove.</param>
        /// <returns>The current instance to allow chaining.</returns>
        ISlaManager Remove(Guid slaId);
    }
}
