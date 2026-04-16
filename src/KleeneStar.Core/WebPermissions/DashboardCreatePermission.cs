using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the creation of new, isolated dashboards.
    /// </summary>
    [Name("dashboard_create")]
    [Policy<DashboardCreatorPolicy>()]
    [Policy<DashboardAdminPolicy>()]
    public sealed class DashboardCreatePermission : IIdentityPermission
    {
    }

}
