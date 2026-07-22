using WebExpress.WebCore.WebAttribute;

namespace KleeneStar.Core.WWW.Api._1_.Assets._workspacekey_
{
    /// <summary>
    /// Kanban endpoint of the asset overview's classic view: the workspace's assets as a
    /// board grouped by workflow status. The board logic lives in
    /// <see cref="global::KleeneStar.Core.WebRestApi.RestApiObjectKindKanban"/>; this
    /// endpoint only scopes it to the asset kind. It is an independent sibling of the
    /// issue Kanban endpoint (not a subclass), so both keep their own route.
    /// </summary>
    [Title("kleenestar.core:object.view.kanban.title")]
    [Cache]
    public sealed class Kanban : global::KleeneStar.Core.WebRestApi.RestApiObjectKindKanban
    {
        /// <summary>
        /// Gets the object kind the board is scoped to: assets.
        /// </summary>
        protected override string Kind => Model.Entities.ObjectKind.Asset;
    }
}
