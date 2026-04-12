using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting full administrative access to group data.
    /// </summary>
    [Name("group_admin_policy")]
    [Permission<GroupReadPermission>()]
    [Permission<GroupCreatePermission>()]
    [Permission<GroupUpdatePermission>()]
    [Permission<GroupDeletePermission>()]
    public sealed class GroupAdminPolicy : IIdentityPolicy
    {
    }
}
