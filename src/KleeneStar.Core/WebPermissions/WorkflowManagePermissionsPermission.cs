using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting access to manage workflow-wide and transition-specific
    /// permission assignments.
    /// </summary>
    [Name("workflow_manage_permissions")]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowManagePermissionsPermission : IIdentityPermission
    {
    }

}
