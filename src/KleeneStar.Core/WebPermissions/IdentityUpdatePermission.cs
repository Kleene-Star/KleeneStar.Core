using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting update access for identities.
    /// </summary>
    [Name("identity_update")]
    [Policy<IdentityEditPolicy>()]
    [Policy<IdentityAdminPolicy>()]
    public sealed class IdentityUpdatePermission : IIdentityPermission
    {
    }
}
