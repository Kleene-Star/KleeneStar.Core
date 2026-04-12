using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting edit access to identity data.
    /// </summary>
    [Name("identity_edit_policy")]
    [Permission<IdentityReadPermission>()]
    [Permission<IdentityCreatePermission>()]
    [Permission<IdentityUpdatePermission>()]
    public sealed class IdentityEditPolicy : IIdentityPolicy
    {
    }
}
