using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling import of external workflow definitions, including dry-run checks.
    /// </summary>
    [Name("workflow_import")]
    [Policy<WorkflowImporterPolicy>()]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowImportPermission : IIdentityPermission
    {
    }

}
