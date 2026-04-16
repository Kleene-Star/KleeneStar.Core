using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the permanent deletion of a form.
    /// </summary>
    [Name("form_delete")]
    [Policy<FormAdminPolicy>()]
    public sealed class FormDeletePermission : IIdentityPermission
    {
    }

}
