using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the import of external class schemas into the workspace.
    /// </summary>
    [Name("class_import")]
    [Policy<ClassImporterPolicy>()]
    [Policy<ClassAdminPolicy>()]
    public sealed class ClassImportPermission : IIdentityPermission
    {
    }

}
