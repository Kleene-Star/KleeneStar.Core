using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting delete access for identities.
    /// </summary>
    [Name("identity_delete")]
    [Policy<IdentityAdminPolicy>()]
    public sealed class IdentityDeletePermission : IIdentityPermission
    {
    }
}
