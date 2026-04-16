using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting read-only access to the field definition
    /// and the values stored in the field.
    /// </summary>
    [Name("field_view_policy")]
    [Permission<FieldReadPermission>()]
    [Permission<FieldReadValuesPermission>()]
    public sealed class FieldViewPolicy : IIdentityPolicy
    {
    }
}
