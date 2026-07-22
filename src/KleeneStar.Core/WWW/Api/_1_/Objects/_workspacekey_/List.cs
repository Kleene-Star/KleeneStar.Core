using WebExpress.WebCore.WebAttribute;

namespace KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_
{
    /// <summary>
    /// List endpoint of the issue overview's classic view: the workspace's issues as a
    /// vertical frame list. The list logic lives in
    /// <see cref="global::KleeneStar.Core.WebRestApi.RestApiObjectKindList"/>; this
    /// endpoint only scopes it to the issue kind.
    /// </summary>
    [Title("kleenestar.core:object.list.header")]
    [Cache]
    public sealed class List : global::KleeneStar.Core.WebRestApi.RestApiObjectKindList
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public List()
        {
        }

        /// <summary>
        /// Gets the object kind the list is scoped to: issues.
        /// </summary>
        protected override string Kind => Model.Entities.ObjectKind.Issue;
    }
}
