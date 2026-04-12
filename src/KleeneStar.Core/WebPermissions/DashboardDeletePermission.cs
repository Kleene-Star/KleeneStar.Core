using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the permanent deletion of a dashboard.
    /// </summary>
    [Name("dashboard_delete")]
    [Policy<DashboardAdminPolicy>()]
    public sealed class DashboardDeletePermission : IIdentityPermission
    {
    }

}
