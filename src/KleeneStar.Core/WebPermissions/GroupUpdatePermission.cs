using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting update access for groups.
    /// </summary>
    [Name("group_update")]
    [Policy<GroupEditPolicy>()]
    [Policy<GroupAdminPolicy>()]
    public sealed class GroupUpdatePermission : IIdentityPermission
    {
    }
}
