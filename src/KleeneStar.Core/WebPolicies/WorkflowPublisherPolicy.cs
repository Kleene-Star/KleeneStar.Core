using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling lifecycle control of workflows without granting structural edit rights.
    /// Includes reading, validating, publishing, archiving, restoring, and reading version history.
    /// </summary>
    [Name("workflow_publisher_policy")]
    [Permission<WorkflowReadPermission>()]
    [Permission<WorkflowValidatePermission>()]
    [Permission<WorkflowPublishPermission>()]
    [Permission<WorkflowArchivePermission>()]
    [Permission<WorkflowRestorePermission>()]
    [Permission<WorkflowVersionsReadPermission>()]
    public sealed class WorkflowPublisherPolicy : IIdentityPolicy
    {
    }
}
