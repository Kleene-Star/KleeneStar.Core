using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebFragment.Object.Documents
{
    /// <summary>
    /// The built-in document kind: hierarchical pages that are organized as a tree
    /// via the object parent/child containment. Named "document" rather than "page"
    /// because the term page is already taken by the WebExpress page concept.
    /// </summary>
    public sealed class Document : IObjectKind
    {
        /// <summary>
        /// Gets the persisted kind key of documents.
        /// </summary>
        public string Key => Model.Entities.ObjectKind.Document;

        /// <summary>
        /// Gets the internationalization key of the plural display name.
        /// </summary>
        public string Label => "kleenestar.core:object.kind.documents.label";

        /// <summary>
        /// Gets the icon representing documents.
        /// </summary>
        public IIcon Icon => new IconFileLines(TypeIconTheme.Light);

        /// <summary>
        /// Gets the display order; documents lead the kind listings.
        /// </summary>
        public int Order => 1;

        /// <summary>
        /// Gets the unbound route of the document overview page (the document tree).
        /// </summary>
        public IUri OverviewUri => CoreHub.GetUri<global::KleeneStar.Core.WWW.Documents._workspacekey_.Index>();
    }
}
