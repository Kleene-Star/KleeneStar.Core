using WebExpress.WebCore.WebAttribute;

namespace KleeneStar.Core.WWW.Api._1_.Assets._workspacekey_
{
    /// <summary>
    /// Kanban endpoint of the asset overview's classic view. Reuses the object board
    /// logic of
    /// <see cref="global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Kanban"/>
    /// but scopes the cards to the asset kind.
    /// </summary>
    [Title("kleenestar.core:object.view.kanban.title")]
    [Cache]
    public sealed class Kanban : global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Kanban
    {
        /// <summary>
        /// Gets the object kind the board is scoped to: assets.
        /// </summary>
        protected override string Kind => Model.Entities.ObjectKind.Asset;
    }
}
