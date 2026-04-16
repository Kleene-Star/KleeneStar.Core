using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing read access to the actual values stored in a field
    /// for the associated objects.
    /// </summary>
    [Name("field_read_values")]
    [Policy<FieldViewPolicy>()]
    [Policy<FieldEditPolicy>()]
    [Policy<FieldAdminPolicy>()]
    public sealed class FieldReadValuesPermission : IIdentityPermission
    {
    }

}
