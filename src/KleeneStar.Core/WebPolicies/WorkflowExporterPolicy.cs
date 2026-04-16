using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling the export of workflow definitions and versions.
    /// </summary>
    [Name("workflow_exporter_policy")]
    [Permission<WorkflowExportPermission>()]
    public sealed class WorkflowExporterPolicy : IIdentityPolicy
    {
    }
}
