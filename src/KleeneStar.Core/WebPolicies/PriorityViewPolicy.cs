using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting read-only access to priority metadata,
    /// rules, and state information.
    /// </summary>
    [Name("priority_view_policy")]
    [Permission<PriorityReadPermission>()]
    public sealed class PriorityViewPolicy : IIdentityPolicy
    {
    }
}
