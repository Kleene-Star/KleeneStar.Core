using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy providing full administrative control over the status catalog of a class.
    /// Includes creation, reading, updating, cloning, deletion, and usage analysis.
    /// </summary>
    [Name("status_admin_policy")]
    [Permission<StatusReadPermission>()]
    [Permission<StatusCreatePermission>()]
    [Permission<StatusUpdatePermission>()]
    [Permission<StatusClonePermission>()]
    [Permission<StatusDeletePermission>()]
    [Permission<StatusUsageReadPermission>()]
    public sealed class StatusAdminPolicy : IIdentityPolicy
    {
    }
}
