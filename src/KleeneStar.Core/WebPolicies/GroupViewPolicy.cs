using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting read-only access to group data.
    /// </summary>
    [Name("group_view_policy")]
    [Permission<GroupReadPermission>()]
    public sealed class GroupViewPolicy : IIdentityPolicy
    {
    }
}
