using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
using System.Linq;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workspace
{
    /// <summary>
    /// Workspace-scoped property card that groups access-related attributes (assigned
    /// permission profiles and tenants) inside a single <see cref="ControlPanelCard"/>.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Policy<WorkspaceViewPolicy>]
    [Order(11)]
    [Cache]
    public sealed class WorkspacePropertyAccessCardFragment : FragmentControlPanel
    {
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WorkspacePropertyAccessCardFragment(IFragmentContext fragmentContext, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _workspaceManager = workspaceManager;
        }

        /// <summary>
        /// Renders the access card. Returns <c>null</c> when no workspace can be
        /// resolved from the request.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var keyParameter = renderContext?.Request?.GetParameter<WorkspaceKeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            if (workspace is null)
            {
                return null;
            }

            var card = new ControlPanelCard("workspace-property-access-card")
            {
                Header = _ => "kleenestar.core:workspace.property.access.header",
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two)
            };

            card.Add(new ControlAttribute("workspace-property-permissionprofiles")
            {
                Icon = _ => new IconUserShield(),
                Key = _ => "kleenestar.core:workspace.permissionprofiles.label",
                Value = ctx => JoinNamesOrNone(ctx, workspace.PermissionProfiles?.Select(p => p.Group?.Name))
            });

            card.Add(new ControlAttribute("workspace-property-tenants")
            {
                Icon = _ => new IconBuilding(),
                Key = _ => "kleenestar.core:workspace.tenant.label",
                Value = ctx => JoinNamesOrNone(ctx, workspace.Tenants?.Select(t => t.Name))
            });

            return card.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Joins a sequence of names into a comma-separated string, falling back to the
        /// localized "none" placeholder when the sequence is null/empty.
        /// </summary>
        private static string JoinNamesOrNone(IRenderControlContext renderContext, System.Collections.Generic.IEnumerable<string> names)
        {
            var filtered = names?.Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
            return filtered is not null && filtered.Count > 0
                ? string.Join(", ", filtered)
                : I18N.Translate(renderContext, "kleenestar.core:workspace.property.none");
        }
    }
}
