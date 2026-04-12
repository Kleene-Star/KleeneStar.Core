using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling the import of external priority definitions.
    /// </summary>
    [Name("priority_importer_policy")]
    [Permission<PriorityImportPermission>()]
    public sealed class PriorityImporterPolicy : IIdentityPolicy
    {
    }
}
