using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting read-only access to identity data.
    /// </summary>
    [Name("identity_view_policy")]
    [Permission<IdentityReadPermission>()]
    public sealed class IdentityViewPolicy : IIdentityPolicy
    {
    }
}
