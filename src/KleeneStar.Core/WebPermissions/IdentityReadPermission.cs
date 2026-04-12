using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to identity data.
    /// </summary>
    [Name("identity_read")]
    [Policy<IdentityViewPolicy>()]
    [Policy<IdentityEditPolicy>()]
    [Policy<IdentityAdminPolicy>()]
    public sealed class IdentityReadPermission : IIdentityPermission
    {
    }
}
