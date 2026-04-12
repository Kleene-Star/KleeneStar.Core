using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing modifications to dashboard metadata.
    /// </summary>
    [Name("dashboard_update")]
    [Policy<DashboardAdminPolicy>()]
    public sealed class DashboardUpdatePermission : IIdentityPermission
    {
    }

}
