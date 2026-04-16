using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing the duplication of an existing dashboard.
    /// </summary>
    [Name("dashboard_clone")]
    [Policy<DashboardAdminPolicy>()]
    public sealed class DashboardClonePermission : IIdentityPermission
    {
    }

}
