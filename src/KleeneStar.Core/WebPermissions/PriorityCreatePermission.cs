using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the creation of new priorities within a class.
    /// </summary>
    [Name("priority_create")]
    [Policy<PriorityAdminPolicy>()]
    public sealed class PriorityCreatePermission : IIdentityPermission
    {
    }

}
