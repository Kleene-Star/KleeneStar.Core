using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Describes an object kind (subtype) such as document, blog, or issue. A kind
    /// partitions the objects of a workspace and decides which overview view presents
    /// them: documents form a hierarchical page tree, blog posts a chronological
    /// timeline, and issues a filterable work-item list.
    /// </summary>
    /// <remarks>
    /// The set of kinds is open: add-ons introduce a new kind by implementing this
    /// interface, registering the descriptor in the <see cref="ObjectKindCatalog"/>
    /// (typically from their plugin initialization), contributing an overview page,
    /// and deriving a sidebar link from <see cref="ObjectKindSidebarLinkFragment"/>.
    /// The persisted counterpart of a descriptor is the plain string key stored in
    /// <see cref="Model.Entities.Object.Kind"/>.
    /// </remarks>
    public interface IObjectKind
    {
        /// <summary>
        /// Gets the kind key persisted in <see cref="Model.Entities.Object.Kind"/>.
        /// Keys are lower-case and compared case-insensitively; the core keys are
        /// defined in <see cref="Model.Entities.ObjectKind"/>.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the internationalization key of the kind's plural display name
        /// (e.g. "Documents"), used for sidebar links and headlines.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the icon representing the kind.
        /// </summary>
        IIcon Icon { get; }

        /// <summary>
        /// Gets the display order of the kind within kind listings such as the
        /// objects sidebar. Lower values are listed first.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Gets the unbound route of the kind's overview page. The route carries the
        /// workspace-key segment, so callers bind the current request (or an explicit
        /// workspace-key parameter) before navigating.
        /// </summary>
        IUri OverviewUri { get; }
    }
}
