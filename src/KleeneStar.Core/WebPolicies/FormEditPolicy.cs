using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy authorizing form model maintenance,
    /// including reading, updating, cloning, and related operations.
    /// </summary>
    [Name("form_edit_policy")]
    [Permission<FormReadPermission>()]
    [Permission<FormUpdatePermission>()]
    [Permission<FormClonePermission>()]
    public sealed class FormEditPolicy : IIdentityPolicy
    {
    }
}
