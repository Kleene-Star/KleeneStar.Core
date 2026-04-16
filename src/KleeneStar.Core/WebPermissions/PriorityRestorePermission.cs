using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the restoration of an archived priority.
    /// </summary>
    [Name("priority_restore")]
    [Policy<PriorityPublisherPolicy>()]
    [Policy<PriorityAdminPolicy>()]
    public sealed class PriorityRestorePermission : IIdentityPermission
    {
    }

}
