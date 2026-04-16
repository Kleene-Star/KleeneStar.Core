using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy allowing creation and modification of class definitions,
    /// including cloning and updating metadata or fields.
    /// </summary>
    [Name("class_edit_policy")]
    [Permission<ClassCreatePermission>()]
    [Permission<ClassReadPermission>()]
    [Permission<ClassUpdatePermission>()]
    [Permission<ClassClonePermission>()]
    public sealed class ClassEditPolicy : IIdentityPolicy
    {
    }
}
