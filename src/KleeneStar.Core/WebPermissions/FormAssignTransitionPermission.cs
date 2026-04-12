using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the maintenance of form assignments
    /// as transition screens within workflows.
    /// </summary>
    [Name("form_assign_transition")]
    [Policy<FormAdminPolicy>()]
    public sealed class FormAssignTransitionPermission : IIdentityPermission
    {
    }

}
