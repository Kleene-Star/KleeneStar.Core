using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling the import of external workflow definitions.
    /// </summary>
    [Name("workflow_importer_policy")]
    [Permission<WorkflowImportPermission>()]
    public sealed class WorkflowImporterPolicy : IIdentityPolicy
    {
    }
}
