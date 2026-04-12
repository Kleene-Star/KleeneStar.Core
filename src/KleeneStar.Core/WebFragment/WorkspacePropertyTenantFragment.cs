using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
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
    /// Represents a property fragment that displays the assigned tenants of a workspace in the detail view.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Policy<WorkspaceViewPolicy>]
    [Cache]
    public sealed class WorkspacePropertyTenantFragment : FragmentControlAttribute
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation.
        /// Cannot be null.
        /// </param>
        public WorkspacePropertyTenantFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Key = "kleenestar.core:workspace.tenant.label";
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

            var tenantNames = workspace?.Tenants?.Select(t => t.Name).Where(n => !string.IsNullOrEmpty(n));
            var value = tenantNames != null && tenantNames.Any()
                ? string.Join(", ", tenantNames)
                : I18N.Translate(renderContext, "kleenestar.core:workspace.property.none");

            return base.Render(renderContext, visualTree, Key, value, Uri, Icon);
        }
    }
}
