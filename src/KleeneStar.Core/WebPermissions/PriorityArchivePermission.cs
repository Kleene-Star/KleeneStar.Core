using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling the archiving of an active priority.
    /// </summary>
    [Name("priority_archive")]
    [Policy<PriorityPublisherPolicy>()]
    [Policy<PriorityAdminPolicy>()]
    public sealed class PriorityArchivePermission : IIdentityPermission
    {
    }

}
