using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing export of workflow definitions and versions.
    /// </summary>
    [Name("workflow_export")]
    [Policy<WorkflowExporterPolicy>()]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowExportPermission : IIdentityPermission
    {
    }

}
