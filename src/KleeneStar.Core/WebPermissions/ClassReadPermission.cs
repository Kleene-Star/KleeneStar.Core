using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to class metadata and field definitions.
    /// </summary>
    [Name("class_read")]
    [Policy<ClassViewPolicy>()]
    [Policy<ClassEditPolicy>()]
    [Policy<ClassAdminPolicy>()]
    public sealed class ClassReadPermission : IIdentityPermission
    {
    }

}
