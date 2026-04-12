using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to form metadata, structure (layout/tree),
    /// and form-to-workflow assignments.
    /// </summary>
    [Name("form_read")]
    [Policy<FormViewPolicy>()]
    [Policy<FormEditPolicy>()]
    [Policy<FormPublisherPolicy>()]
    [Policy<FormAdminPolicy>()]
    public sealed class FormReadPermission : IIdentityPermission
    {
    }

}
