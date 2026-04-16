using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing modifications to an existing form,
    /// including layout, rules, bindings, and metadata.
    /// </summary>
    [Name("form_update")]
    [Policy<FormEditPolicy>()]
    [Policy<FormAdminPolicy>()]
    public sealed class FormUpdatePermission : IIdentityPermission
    {
    }

}
