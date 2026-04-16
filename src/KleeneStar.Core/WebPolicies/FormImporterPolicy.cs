using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling the import of external form definitions.
    /// </summary>
    [Name("form_importer_policy")]
    [Permission<FormImportPermission>()]
    public sealed class FormImporterPolicy : IIdentityPolicy
    {
    }
}
