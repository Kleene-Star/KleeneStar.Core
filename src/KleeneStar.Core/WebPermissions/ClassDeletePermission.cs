using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the permanent deletion of a class definition.
    /// </summary>
    [Name("class_delete")]
    [Policy<ClassAdminPolicy>()]
    public sealed class ClassDeletePermission : IIdentityPermission
    {
    }

}
