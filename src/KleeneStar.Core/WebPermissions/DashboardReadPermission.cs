using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to dashboard metadata,
    /// including name, description, and status.
    /// </summary>
    [Name("dashboard_read")]
    [Policy<DashboardViewPolicy>()]
    [Policy<DashboardEditPolicy>()]
    [Policy<DashboardAdminPolicy>()]
    public sealed class DashboardReadPermission : IIdentityPermission
    {
    }

}
