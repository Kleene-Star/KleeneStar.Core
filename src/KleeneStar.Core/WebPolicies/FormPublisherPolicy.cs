using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling lifecycle control of forms without granting modification rights.
    /// Includes reading, archiving, restoring, and exporting forms.
    /// </summary>
    [Name("form_publisher_policy")]
    [Permission<FormReadPermission>()]
    [Permission<FormArchivePermission>()]
    [Permission<FormRestorePermission>()]
    [Permission<FormExportPermission>()]
    public sealed class FormPublisherPolicy : IIdentityPolicy
    {
    }
}
