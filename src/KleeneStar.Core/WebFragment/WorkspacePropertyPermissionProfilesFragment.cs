using KleeneStar.Core.WebParameter;
using System.Linq;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a property fragment that displays the assigned permission profiles of a workspace in the detail view.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Cache]
    public sealed class WorkspacePropertyPermissionProfilesFragment : FragmentControlAttribute
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation.
        /// Cannot be null.
        /// </param>
        public WorkspacePropertyPermissionProfilesFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Key = "kleenestar.core:workspace.permissionprofiles.label";
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            var profileCount = workspace?.PermissionProfiles?.Count() ?? 0;
            var value = profileCount > 0
                ? profileCount.ToString()
                : I18N.Translate(renderContext, "kleenestar.core:workspace.property.none");

            return base.Render(renderContext, visualTree, Key, value, Uri, Icon);
        }
    }
}
