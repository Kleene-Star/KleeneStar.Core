using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling restoration of an archived workflow version.
    /// </summary>
    [Name("workflow_restore")]
    [Policy<WorkflowPublisherPolicy>()]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowRestorePermission : IIdentityPermission
    {
    }

}
