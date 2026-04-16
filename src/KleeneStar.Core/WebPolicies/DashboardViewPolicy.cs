using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting read-only access to dashboard metadata and content.
    /// </summary>
    [Name("dashboard_view_policy")]
    [Permission<DashboardReadPermission>()]
    [Permission<DashboardReadContentPermission>()]
    public sealed class DashboardViewPolicy : IIdentityPolicy
    {
    }
}
