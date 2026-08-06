using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing the group-to-policy grants the permission dialogs
    /// administer.
    /// </summary>
    public interface IPermissionManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when a grant is added.
        /// </summary>
        event EventHandler<PermissionAssignment> PermissionAssigned;

        /// <summary>
        /// An event that fires when a grant is withdrawn.
        /// </summary>
        event EventHandler<PermissionAssignment> PermissionRevoked;

        /// <summary>
        /// Returns the grants on one resource, in the order they are listed.
        /// </summary>
        /// <param name="scope">The kind of resource.</param>
        /// <param name="scopeId">The identifier of the resource within its scope.</param>
        /// <returns>The grants, ordered by group and then by policy.</returns>
        IEnumerable<PermissionAssignment> GetAssignments(string scope, string scopeId);

        /// <summary>
        /// Grants a group a policy on a resource.
        /// </summary>
        /// <remarks>
        /// Granting what is already granted changes nothing and is reported as the existing grant,
        /// so a repeated click cannot produce a duplicate row.
        /// </remarks>
        /// <param name="scope">The kind of resource.</param>
        /// <param name="scopeId">The identifier of the resource within its scope.</param>
        /// <param name="groupId">The group the policy is granted to.</param>
        /// <param name="policy">The registered name of the policy.</param>
        /// <returns>The grant, or null when the group or the policy is not known.</returns>
        PermissionAssignment Assign(string scope, string scopeId, Guid groupId, string policy);

        /// <summary>
        /// Withdraws a policy from a group on a resource.
        /// </summary>
        /// <param name="scope">The kind of resource.</param>
        /// <param name="scopeId">The identifier of the resource within its scope.</param>
        /// <param name="groupId">The group the policy is withdrawn from.</param>
        /// <param name="policy">The registered name of the policy.</param>
        /// <returns>True when a grant was withdrawn; false when there was none.</returns>
        bool Revoke(string scope, string scopeId, Guid groupId, string policy);
    }
}
