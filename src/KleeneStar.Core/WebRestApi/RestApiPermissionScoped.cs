using KleeneStar.Core.WebPermission;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WebRestApi
{
    /// <summary>
    /// Serves the permission dialog of one kind of resource: the grants on it, and the additions
    /// and withdrawals the dialog performs.
    /// </summary>
    /// <remarks>
    /// Every resource administers the same thing — which group holds which policy on it — so the
    /// work is done once here and a resource contributes only its scope and how its id is read
    /// from the route.
    /// </remarks>
    public abstract class RestApiPermissionScoped : RestApiPermission
    {
        /// <summary>
        /// Gets the kind of resource this endpoint administers, as named in
        /// <see cref="PermissionScope"/>.
        /// </summary>
        protected abstract string Scope { get; }

        /// <summary>
        /// Returns the identifier of the resource the request addresses.
        /// </summary>
        /// <param name="request">The request whose route names the resource.</param>
        /// <returns>The identifier, or null when the route addresses none.</returns>
        protected abstract string ResolveScopeId(IRequest request);

        /// <summary>
        /// Returns the grants on the addressed resource.
        /// </summary>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>
        /// The grants, naming the group and the policy. Empty when the route addresses no resource.
        /// </returns>
        protected override IEnumerable<RestApiPermissionItem> RetrieveAssignments(IRequest request)
        {
            var scopeId = ResolveScopeId(request);

            foreach (var assignment in CoreHub.PermissionManager.GetAssignments(Scope, scopeId))
            {
                yield return new RestApiPermissionItem()
                {
                    GroupId = assignment.GroupId.ToString(),
                    GroupName = assignment.Group?.Name,
                    PolicyId = assignment.Policy,
                    PolicyName = PolicyCatalog.GetLabel(assignment.Policy, Scope)
                };
            }
        }

        /// <summary>
        /// Grants a group a policy on the addressed resource.
        /// </summary>
        /// <param name="groupId">The group the policy is granted to.</param>
        /// <param name="policyId">The registered name of the policy.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>
        /// The grant, or null when the resource, the group or the policy is not known — the base
        /// endpoint answers that as a bad request rather than storing a grant that cannot take
        /// effect.
        /// </returns>
        protected override RestApiPermissionItem AddAssignment(string groupId, string policyId, IRequest request)
        {
            var scopeId = ResolveScopeId(request);

            if (!Guid.TryParse(groupId, out var group))
            {
                return null;
            }

            var assignment = CoreHub.PermissionManager.Assign(Scope, scopeId, group, policyId);

            if (assignment is null)
            {
                return null;
            }

            return new RestApiPermissionItem()
            {
                GroupId = assignment.GroupId.ToString(),
                GroupName = assignment.Group?.Name ?? CoreHub.GroupManager.GetGroup(group)?.Name,
                PolicyId = assignment.Policy,
                PolicyName = PolicyCatalog.GetLabel(assignment.Policy, Scope)
            };
        }

        /// <summary>
        /// Withdraws a policy from a group on the addressed resource.
        /// </summary>
        /// <param name="groupId">The group the policy is withdrawn from.</param>
        /// <param name="policyId">The registered name of the policy.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>True when a grant was withdrawn; false when there was none.</returns>
        protected override bool RemoveAssignment(string groupId, string policyId, IRequest request)
        {
            var scopeId = ResolveScopeId(request);

            return Guid.TryParse(groupId, out var group) &&
                CoreHub.PermissionManager.Revoke(Scope, scopeId, group, policyId);
        }

        /// <summary>
        /// Narrows the listed grants by the dialog's search term.
        /// </summary>
        /// <remarks>
        /// The term is matched against what the dialog shows — the group and the policy as
        /// labelled — so searching for what is on screen finds it.
        /// </remarks>
        /// <param name="search">The search term.</param>
        /// <param name="assignments">The grants to narrow.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The matching grants.</returns>
        protected override IEnumerable<RestApiPermissionItem> Filter(string search, IEnumerable<RestApiPermissionItem> assignments, IRequest request)
        {
            if (string.IsNullOrWhiteSpace(search) || search == "null")
            {
                return assignments;
            }

            return assignments.Where(x =>
                (x.GroupName ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (x.PolicyName ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (x.PolicyId ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase));
        }
    }
}
