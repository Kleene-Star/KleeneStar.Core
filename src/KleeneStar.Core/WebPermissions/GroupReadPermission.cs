using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to group data.
    /// </summary>
    [Name("group_read")]
    [Policy<GroupViewPolicy>()]
    [Policy<GroupEditPolicy>()]
    [Policy<GroupAdminPolicy>()]
    public sealed class GroupReadPermission : IIdentityPermission
    {
    }
}
