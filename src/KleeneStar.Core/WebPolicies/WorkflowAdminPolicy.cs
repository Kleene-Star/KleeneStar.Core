using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy providing full administrative control over workflow definitions,
    /// transitions, lifecycle operations, import/export, and permission assignments.
    /// </summary>
    [Name("workflow_admin_policy")]
    [Permission<WorkflowReadPermission>()]
    [Permission<WorkflowUpdatePermission>()]
    [Permission<WorkflowValidatePermission>()]
    [Permission<WorkflowPublishPermission>()]
    [Permission<WorkflowArchivePermission>()]
    [Permission<WorkflowRestorePermission>()]
    [Permission<WorkflowClonePermission>()]
    [Permission<WorkflowDeletePermission>()]
    [Permission<WorkflowVersionsReadPermission>()]
    [Permission<WorkflowImportPermission>()]
    [Permission<WorkflowExportPermission>()]
    [Permission<WorkflowManagePermissionsPermission>()]
    [Permission<TransitionExecutePermission>()]
    public sealed class WorkflowAdminPolicy : IIdentityPolicy
    {
    }
}
