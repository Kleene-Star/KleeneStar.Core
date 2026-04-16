using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting full administrative access to identity data.
    /// </summary>
    [Name("identity_admin_policy")]
    [Permission<IdentityReadPermission>()]
    [Permission<IdentityCreatePermission>()]
    [Permission<IdentityUpdatePermission>()]
    [Permission<IdentityDeletePermission>()]
    public sealed class IdentityAdminPolicy : IIdentityPolicy
    {
    }
}
