using WebExpress.WebCore.WebAttribute;

namespace KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_
{
    /// <summary>
    /// Kanban endpoint of the issue overview's classic view: the workspace's issues as a
    /// board grouped by workflow status. The board logic lives in
    /// <see cref="global::KleeneStar.Core.WebRestApi.RestApiObjectKindKanban"/>; this
    /// endpoint only scopes it to the issue kind.
    /// </summary>
    [Title("kleenestar.core:object.view.kanban.title")]
    [Cache]
    public sealed class Kanban : global::KleeneStar.Core.WebRestApi.RestApiObjectKindKanban
    {
        /// <summary>
        /// Gets the object kind the board is scoped to: issues.
        /// </summary>
        protected override string Kind => Model.Entities.ObjectKind.Issue;
    }
}
