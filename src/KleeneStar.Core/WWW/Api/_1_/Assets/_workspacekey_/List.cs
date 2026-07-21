using WebExpress.WebCore.WebAttribute;

namespace KleeneStar.Core.WWW.Api._1_.Assets._workspacekey_
{
    /// <summary>
    /// List endpoint of the asset overview's classic view. Reuses the object list logic
    /// of <see cref="global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.List"/>
    /// but scopes it to the asset kind.
    /// </summary>
    [Title("kleenestar.core:object.list.header")]
    [Cache]
    public sealed class List : global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.List
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public List()
        {
        }

        /// <summary>
        /// Gets the object kind the list is scoped to: assets.
        /// </summary>
        protected override string Kind => Model.Entities.ObjectKind.Asset;
    }
}
