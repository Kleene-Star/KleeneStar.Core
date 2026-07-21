using WebExpress.WebCore.WebAttribute;

namespace KleeneStar.Core.WWW.Api._1_.Assets._workspacekey_
{
    /// <summary>
    /// Tile endpoint of the asset overview's classic view. Reuses the object tile logic
    /// of <see cref="global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Tile"/>
    /// but scopes it to the asset kind.
    /// </summary>
    [Title("kleenestar.core:object.tile.header")]
    [Cache]
    public sealed class Tile : global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Tile
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Tile()
        {
        }

        /// <summary>
        /// Gets the object kind the tile view is scoped to: assets.
        /// </summary>
        protected override string Kind => Model.Entities.ObjectKind.Asset;
    }
}
