using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing creation, modification, and deletion of dashboard content.
    /// </summary>
    [Name("dashboard_write_content")]
    [Policy<DashboardEditPolicy>()]
    [Policy<DashboardAdminPolicy>()]
    public sealed class DashboardWriteContentPermission : IIdentityPermission
    {
    }

}
