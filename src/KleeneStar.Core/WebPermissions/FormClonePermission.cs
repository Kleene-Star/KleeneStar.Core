using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing the cloning of an existing form,
    /// producing a new active version.
    /// </summary>
    [Name("form_clone")]
    [Policy<FormEditPolicy>()]
    [Policy<FormAdminPolicy>()]
    public sealed class FormClonePermission : IIdentityPermission
    {
    }

}
