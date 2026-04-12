using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the creation, modification, and deletion of field values
    /// within objects.
    /// </summary>
    [Name("field_write_values")]
    [Policy<FieldEditPolicy>()]
    [Policy<FieldAdminPolicy>()]
    public sealed class FieldWriteValuesPermission : IIdentityPermission
    {
    }

}
