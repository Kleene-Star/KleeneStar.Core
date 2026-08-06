using KleeneStar.Core.WebPermission;
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
    /// Manages the group-to-policy grants the permission dialogs administer.
    /// </summary>
    public sealed class PermissionManager : IPermissionManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// An event that fires when a grant is added.
        /// </summary>
        public event EventHandler<PermissionAssignment> PermissionAssigned;

        /// <summary>
        /// An event that fires when a grant is withdrawn.
        /// </summary>
        public event EventHandler<PermissionAssignment> PermissionRevoked;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private PermissionManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns the grants on one resource, in the order they are listed.
        /// </summary>
        /// <param name="scope">The kind of resource.</param>
        /// <param name="scopeId">The identifier of the resource within its scope.</param>
        /// <returns>The grants, ordered by group and then by policy.</returns>
        public IEnumerable<PermissionAssignment> GetAssignments(string scope, string scopeId)
        {
            if (string.IsNullOrWhiteSpace(scope) || string.IsNullOrWhiteSpace(scopeId))
            {
                return [];
            }

            var query = new Query<PermissionAssignment>()
                .WhereEquals(x => x.Scope, scope);

            // the resource id is compared after materialization so the comparison is the same one
            // the route makes, rather than whatever collation the store happens to use
            return [.. ModelHub.GetPermissionAssignments(query)
                .Where(x => string.Equals(x.ScopeId, scopeId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Group?.Name, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(x => x.Policy, StringComparer.OrdinalIgnoreCase)];
        }

        /// <summary>
        /// Grants a group a policy on a resource.
        /// </summary>
        /// <param name="scope">The kind of resource.</param>
        /// <param name="scopeId">The identifier of the resource within its scope.</param>
        /// <param name="groupId">The group the policy is granted to.</param>
        /// <param name="policy">The registered name of the policy.</param>
        /// <returns>The grant, or null when the group or the policy is not known.</returns>
        public PermissionAssignment Assign(string scope, string scopeId, Guid groupId, string policy)
        {
            if (string.IsNullOrWhiteSpace(scope) || string.IsNullOrWhiteSpace(scopeId))
            {
                return null;
            }

            // a grant naming a policy no guard knows would never take effect, and a grant to a
            // group that does not exist could never be read back with a name to show
            if (!PolicyCatalog.IsKnown(policy, scope) || CoreHub.GroupManager.GetGroup(groupId) is null)
            {
                return null;
            }

            var existing = GetAssignments(scope, scopeId)
                .FirstOrDefault(x => x.GroupId == groupId && string.Equals(x.Policy, policy, StringComparison.OrdinalIgnoreCase));

            if (existing is not null)
            {
                return existing;
            }

            var assignment = new PermissionAssignment(Guid.NewGuid())
            {
                Scope = scope,
                ScopeId = scopeId,
                GroupId = groupId,
                Policy = policy,
                Created = DateTime.UtcNow
            };

            ModelHub.Add(assignment);

            PermissionAssigned?.Invoke(this, assignment);

            // the stored record carries the group, which the caller needs to name it in the list
            return GetAssignments(scope, scopeId)
                .FirstOrDefault(x => x.Id == assignment.Id) ?? assignment;
        }

        /// <summary>
        /// Withdraws a policy from a group on a resource.
        /// </summary>
        /// <param name="scope">The kind of resource.</param>
        /// <param name="scopeId">The identifier of the resource within its scope.</param>
        /// <param name="groupId">The group the policy is withdrawn from.</param>
        /// <param name="policy">The registered name of the policy.</param>
        /// <returns>True when a grant was withdrawn; false when there was none.</returns>
        public bool Revoke(string scope, string scopeId, Guid groupId, string policy)
        {
            var assignment = GetAssignments(scope, scopeId)
                .FirstOrDefault(x => x.GroupId == groupId && string.Equals(x.Policy, policy, StringComparison.OrdinalIgnoreCase));

            if (assignment is null)
            {
                return false;
            }

            ModelHub.Remove(assignment);

            PermissionRevoked?.Invoke(this, assignment);

            return true;
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
