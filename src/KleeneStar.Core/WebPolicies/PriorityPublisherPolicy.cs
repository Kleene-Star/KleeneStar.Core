using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling lifecycle control of priorities without granting modification rights.
    /// Includes reading, archiving, restoring, and exporting priorities.
    /// </summary>
    [Name("priority_publisher_policy")]
    [Permission<PriorityReadPermission>()]
    [Permission<PriorityArchivePermission>()]
    [Permission<PriorityRestorePermission>()]
    [Permission<PriorityExportPermission>()]
    public sealed class PriorityPublisherPolicy : IIdentityPolicy
    {
    }
}
