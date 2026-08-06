using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebCore.WebAttribute;

namespace KleeneStar.Core.WebPermission
{
    /// <summary>
    /// Reads the policies a permission dialog can grant from the ones the application registered.
    /// </summary>
    /// <remarks>
    /// The catalog is the running system's own list of <c>IIdentityPolicy</c> components, so it
    /// cannot fall behind the classes the guards check — which a table of policies maintained
    /// alongside them would.
    ///
    /// A policy belongs to the resource whose name it carries: the registered names follow
    /// <c>&lt;scope&gt;_&lt;role&gt;_policy</c>, so the dialog of a workspace offers the
    /// <c>workspace_…</c> policies rather than the whole catalog.
    /// </remarks>
    public static class PolicyCatalog
    {
        /// <summary>
        /// Returns the registered policies that apply to a resource.
        /// </summary>
        /// <param name="scope">The kind of resource, as named in <see cref="PermissionScope"/>.</param>
        /// <returns>
        /// The policy names, in the order they are offered. Empty when the application registered
        /// none for that resource.
        /// </returns>
        public static IEnumerable<string> GetPolicies(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope))
            {
                return [];
            }

            var prefix = scope + "_";

            return [.. GetRegisteredPolicies()
                .Where(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];
        }

        /// <summary>
        /// Determines whether a policy is registered for a resource.
        /// </summary>
        /// <remarks>
        /// The dialog posts a name the client picked, so it is checked against the catalog before
        /// it is stored — a grant naming a policy no guard knows would never take effect and would
        /// sit in the list looking as though it had.
        /// </remarks>
        /// <param name="policy">The policy name to check.</param>
        /// <param name="scope">The kind of resource.</param>
        /// <returns>True when the policy is registered and applies to that resource.</returns>
        public static bool IsKnown(string policy, string scope)
        {
            return !string.IsNullOrWhiteSpace(policy) &&
                GetPolicies(scope).Contains(policy, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the label shown for a policy.
        /// </summary>
        /// <remarks>
        /// The registered name is a key rather than prose, so the role it names is turned into
        /// something readable: <c>workspace_admin_policy</c> reads as <c>Admin</c>. The resource is
        /// left out because the dialog already belongs to one.
        /// </remarks>
        /// <param name="policy">The registered policy name.</param>
        /// <param name="scope">The kind of resource.</param>
        /// <returns>The label of the policy.</returns>
        public static string GetLabel(string policy, string scope)
        {
            if (string.IsNullOrWhiteSpace(policy))
            {
                return policy;
            }

            var role = policy;
            var prefix = scope + "_";

            if (!string.IsNullOrWhiteSpace(scope) && role.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                role = role[prefix.Length..];
            }

            if (role.EndsWith("_policy", StringComparison.OrdinalIgnoreCase))
            {
                role = role[..^"_policy".Length];
            }

            role = role.Replace('_', ' ').Trim();

            return string.IsNullOrEmpty(role)
                ? policy
                : char.ToUpperInvariant(role[0]) + role[1..];
        }

        /// <summary>
        /// Returns the names of every policy the application registered.
        /// </summary>
        /// <remarks>
        /// The name is read from the policy type's <c>Name</c> attribute rather than from the
        /// registered component id, which is the full type name: it is the attribute value that
        /// the grants are stored under and that the rest of the system knows a policy by.
        ///
        /// Reported as empty rather than throwing when the host is not fully initialized, which is
        /// the case in unit tests that exercise the surrounding logic without a component hub.
        /// </remarks>
        /// <returns>The registered policy names.</returns>
        private static IEnumerable<string> GetRegisteredPolicies()
        {
            var policies = CoreHub.ComponentHub?.IdentityManager?.Policies;

            if (policies is null)
            {
                return [];
            }

            return [.. policies
                .Select(x => GetName(x.Policy))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)];
        }

        /// <summary>
        /// Returns the name a policy type is registered under.
        /// </summary>
        /// <remarks>
        /// The attribute discards its argument, so the value is taken from the attribute data the
        /// compiler recorded rather than from an instance of it.
        /// </remarks>
        /// <param name="policy">The policy type.</param>
        /// <returns>The registered name, or null when the type carries none.</returns>
        private static string GetName(Type policy)
        {
            var attribute = policy?.CustomAttributes
                .FirstOrDefault(x => x.AttributeType == typeof(NameAttribute));

            return attribute?.ConstructorArguments.FirstOrDefault().Value as string;
        }
    }
}
