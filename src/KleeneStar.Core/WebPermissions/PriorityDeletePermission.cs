using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the permanent deletion of a priority.
    /// Only archived priorities may be deleted.
    /// </summary>
    [Name("priority_delete")]
    [Policy<PriorityAdminPolicy>()]
    public sealed class PriorityDeletePermission : IIdentityPermission
    {
    }

}
