using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling the export of priority definitions,
    /// including rules and metadata.
    /// </summary>
    [Name("priority_exporter_policy")]
    [Permission<PriorityExportPermission>()]
    public sealed class PriorityExporterPolicy : IIdentityPolicy
    {
    }
}
