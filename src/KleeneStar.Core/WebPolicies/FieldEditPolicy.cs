using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy authorizing the management of field values,
    /// including reading and writing values, as well as reading field metadata.
    /// </summary>
    [Name("field_edit_policy")]
    [Permission<FieldReadPermission>()]
    [Permission<FieldReadValuesPermission>()]
    [Permission<FieldWriteValuesPermission>()]
    public sealed class FieldEditPolicy : IIdentityPolicy
    {
    }
}
