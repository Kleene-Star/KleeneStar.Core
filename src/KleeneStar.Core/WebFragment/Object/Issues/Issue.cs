using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebFragment.Object.Issues
{
    /// <summary>
    /// The built-in issue kind: work items such as incidents, problems, or tasks. The
    /// overview lists the most recently updated issues and offers quickfilters for
    /// starred issues and personal scopes. Issues are the default kind — every object
    /// predating the kind partition behaves like a work item.
    /// </summary>
    public sealed class Issue : IObjectKind
    {
        /// <summary>
        /// Gets the persisted kind key of issues.
        /// </summary>
        public string Key => Model.Entities.ObjectKind.Issue;

        /// <summary>
        /// Gets the internationalization key of the plural display name.
        /// </summary>
        public string Label => "kleenestar.core:object.kind.issues.label";

        /// <summary>
        /// Gets the icon representing issues.
        /// </summary>
        public IIcon Icon => new IconListCheck();

        /// <summary>
        /// Gets the display order; issues close the built-in kind listings.
        /// </summary>
        public int Order => 3;

        /// <summary>
        /// Gets the unbound route of the issue overview page (the issue list).
        /// </summary>
        public IUri OverviewUri => CoreHub.GetUri<global::KleeneStar.Core.WWW.Issues._workspacekey_.Index>();
    }
}
