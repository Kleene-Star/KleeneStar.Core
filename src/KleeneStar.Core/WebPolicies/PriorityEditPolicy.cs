using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy authorizing rule and model maintenance for priorities,
    /// including reading, updating, and cloning.
    /// </summary>
    [Name("priority_edit_policy")]
    [Permission<PriorityReadPermission>()]
    [Permission<PriorityUpdatePermission>()]
    [Permission<PriorityClonePermission>()]
    public sealed class PriorityEditPolicy : IIdentityPolicy
    {
    }
}
