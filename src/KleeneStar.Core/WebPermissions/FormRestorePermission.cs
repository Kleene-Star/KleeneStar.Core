using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the restoration of an archived form,
    /// creating a new active version.
    /// </summary>
    [Name("form_restore")]
    [Policy<FormPublisherPolicy>()]
    [Policy<FormAdminPolicy>()]
    public sealed class FormRestorePermission : IIdentityPermission
    {
    }

}
