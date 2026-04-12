using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Global policy allowing the creation of new dashboards.
    /// </summary>
    [Name("dashboard_creator_policy")]
    [Permission<DashboardCreatePermission>()]
    public sealed class DashboardCreatorPolicy : IIdentityPolicy
    {
    }
}
