using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy providing full administrative control over a dashboard,
    /// including creation, modification, deletion, lifecycle operations,
    /// content management, and profile administration.
    /// </summary>
    [Name("dashboard_admin_policy")]
    [Permission<DashboardCreatePermission>()]
    [Permission<DashboardReadPermission>()]
    [Permission<DashboardUpdatePermission>()]
    [Permission<DashboardDeletePermission>()]
    [Permission<DashboardArchivePermission>()]
    [Permission<DashboardRestorePermission>()]
    [Permission<DashboardClonePermission>()]
    [Permission<DashboardManageProfilesPermission>()]
    [Permission<DashboardReadContentPermission>()]
    [Permission<DashboardWriteContentPermission>()]
    public sealed class DashboardAdminPolicy : IIdentityPolicy
    {
    }
}
