using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting create access for identities.
    /// </summary>
    [Name("identity_create")]
    [Policy<IdentityEditPolicy>()]
    [Policy<IdentityAdminPolicy>()]
    public sealed class IdentityCreatePermission : IIdentityPermission
    {
    }
}
