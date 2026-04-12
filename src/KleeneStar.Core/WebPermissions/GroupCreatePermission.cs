using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting create access for groups.
    /// </summary>
    [Name("group_create")]
    [Policy<GroupEditPolicy>()]
    [Policy<GroupAdminPolicy>()]
    public sealed class GroupCreatePermission : IIdentityPermission
    {
    }
}
