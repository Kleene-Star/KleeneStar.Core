using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the export of priority definitions,
    /// including rules and metadata.
    /// </summary>
    [Name("priority_export")]
    [Policy<PriorityPublisherPolicy>()]
    [Policy<PriorityExporterPolicy>()]
    [Policy<PriorityAdminPolicy>()]
    public sealed class PriorityExportPermission : IIdentityPermission
    {
    }

}
