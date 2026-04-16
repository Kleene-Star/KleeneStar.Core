using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the creation of new forms within a class.
    /// Newly created forms become active immediately after validation.
    /// </summary>
    [Name("form_create")]
    [Policy<FormAdminPolicy>()]
    public sealed class FormCreatePermission : IIdentityPermission
    {
    }

}
