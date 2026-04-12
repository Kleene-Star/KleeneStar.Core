using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting delete access for groups.
    /// </summary>
    [Name("group_delete")]
    [Policy<GroupAdminPolicy>()]
    public sealed class GroupDeletePermission : IIdentityPermission
    {
    }
}
