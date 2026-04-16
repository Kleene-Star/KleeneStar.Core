using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy authorizing management of dashboard content,
    /// including reading and writing content.
    /// </summary>
    [Name("dashboard_edit_policy")]
    [Permission<DashboardReadPermission>()]
    [Permission<DashboardReadContentPermission>()]
    [Permission<DashboardWriteContentPermission>()]
    public sealed class DashboardEditPolicy : IIdentityPolicy
    {
    }
}
