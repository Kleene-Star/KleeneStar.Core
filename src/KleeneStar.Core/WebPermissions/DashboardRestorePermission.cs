using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the restoration of an archived dashboard.
    /// </summary>
    [Name("dashboard_restore")]
    [Policy<DashboardAdminPolicy>()]
    public sealed class DashboardRestorePermission : IIdentityPermission
    {
    }

}
