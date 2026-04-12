using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to priority metadata, rules, and state information.
    /// </summary>
    [Name("priority_read")]
    [Policy<PriorityViewPolicy>()]
    [Policy<PriorityEditPolicy>()]
    [Policy<PriorityPublisherPolicy>()]
    [Policy<PriorityAdminPolicy>()]
    public sealed class PriorityReadPermission : IIdentityPermission
    {
    }

}
