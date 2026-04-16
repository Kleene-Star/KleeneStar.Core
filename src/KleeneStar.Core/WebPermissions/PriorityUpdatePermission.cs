using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing modifications to an existing priority,
    /// including score, category, rules, and descriptive metadata.
    /// </summary>
    [Name("priority_update")]
    [Policy<PriorityEditPolicy>()]
    [Policy<PriorityAdminPolicy>()]
    public sealed class PriorityUpdatePermission : IIdentityPermission
    {
    }

}
