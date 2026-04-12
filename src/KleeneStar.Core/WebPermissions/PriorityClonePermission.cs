using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing the cloning of an existing priority,
    /// including its rules and metadata.
    /// </summary>
    [Name("priority_clone")]
    [Policy<PriorityEditPolicy>()]
    [Policy<PriorityAdminPolicy>()]
    public sealed class PriorityClonePermission : IIdentityPermission
    {
    }

}
