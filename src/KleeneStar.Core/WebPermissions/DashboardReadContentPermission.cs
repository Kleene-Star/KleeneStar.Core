using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to dashboard content,
    /// such as entities, attributes, and widgets.
    /// </summary>
    [Name("dashboard_read_content")]
    [Policy<DashboardViewPolicy>()]
    [Policy<DashboardEditPolicy>()]
    [Policy<DashboardAdminPolicy>()]
    public sealed class DashboardReadContentPermission : IIdentityPermission
    {
    }

}
