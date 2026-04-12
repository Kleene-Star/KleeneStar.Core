using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing users to add and edit comments on an object.
    /// </summary>
    [Name("object_comment")]
    [Policy<ObjectEditPolicy>()]
    [Policy<ObjectAdminPolicy>()]
    public sealed class ObjectCommentPermission : IIdentityPermission
    {
    }

}
