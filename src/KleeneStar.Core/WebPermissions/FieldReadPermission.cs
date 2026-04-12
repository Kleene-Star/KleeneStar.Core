using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to the metadata and configuration of a field.
    /// </summary>
    [Name("field_read")]
    [Policy<FieldViewPolicy>()]
    [Policy<FieldEditPolicy>()]
    [Policy<FieldAdminPolicy>()]
    public sealed class FieldReadPermission : IIdentityPermission
    {
    }

}
