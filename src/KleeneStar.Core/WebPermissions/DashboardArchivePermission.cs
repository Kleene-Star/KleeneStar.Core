using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling the archiving of an active dashboard.
    /// </summary>
    [Name("dashboard_archive")]
    [Policy<DashboardAdminPolicy>()]
    public sealed class DashboardArchivePermission : IIdentityPermission
    {
    }

}
