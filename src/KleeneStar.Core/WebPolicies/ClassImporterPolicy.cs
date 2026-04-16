using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling the import of external class schemas into the workspace.
    /// </summary>
    [Name("class_importer_policy")]
    [Permission<ClassImportPermission>()]
    public sealed class ClassImporterPolicy : IIdentityPolicy
    {
    }
}
