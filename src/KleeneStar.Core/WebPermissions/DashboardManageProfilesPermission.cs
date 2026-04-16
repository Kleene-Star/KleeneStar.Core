using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting access to manage dashboard profiles,
    /// including assignment of policies to global groups.
    /// </summary>
    [Name("dashboard_manage_profiles")]
    [Policy<DashboardAdminPolicy>()]
    public sealed class DashboardManageProfilesPermission : IIdentityPermission
    {
    }

}
