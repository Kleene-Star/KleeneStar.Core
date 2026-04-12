using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting edit access to group data.
    /// </summary>
    [Name("group_edit_policy")]
    [Permission<GroupReadPermission>()]
    [Permission<GroupCreatePermission>()]
    [Permission<GroupUpdatePermission>()]
    public sealed class GroupEditPolicy : IIdentityPolicy
    {
    }
}
