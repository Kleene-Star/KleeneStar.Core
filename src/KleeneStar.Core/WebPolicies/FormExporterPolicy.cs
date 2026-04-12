using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling the export of form definitions,
    /// including structure and metadata.
    /// </summary>
    [Name("form_exporter_policy")]
    [Permission<FormExportPermission>()]
    public sealed class FormExporterPolicy : IIdentityPolicy
    {
    }
}
