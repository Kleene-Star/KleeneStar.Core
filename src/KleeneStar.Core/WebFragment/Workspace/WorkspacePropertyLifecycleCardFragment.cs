using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
using KleeneStar.Model.Entities;
using System.Globalization;
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
    /// Workspace-scoped property card that groups the lifecycle attributes of the
    /// workspace (creation timestamp, last update timestamp, and lifecycle state)
    /// inside a single <see cref="ControlPanelCard"/>.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Policy<WorkspaceViewPolicy>]
    [Order(9)]
    [Cache]
    public sealed class WorkspacePropertyLifecycleCardFragment : FragmentControlPanel
    {
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WorkspacePropertyLifecycleCardFragment(IFragmentContext fragmentContext, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _workspaceManager = workspaceManager;
        }

        /// <summary>
        /// Renders the lifecycle card. Returns <c>null</c> when no workspace can be
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

            var card = new ControlPanelCard("workspace-property-lifecycle-card")
            {
                Header = _ => "kleenestar.core:workspace.property.lifecycle.header",
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two)
            };

            card.Add(new ControlAttribute("workspace-property-created")
            {
                Icon = _ => new IconCalendarPlus(),
                Key = _ => "kleenestar.core:workspace.created.label",
                Value = _ => workspace.Created.ToString("g", CultureInfo.InvariantCulture)
            });

            card.Add(new ControlAttribute("workspace-property-updated")
            {
                Icon = _ => new IconClockRotateLeft(),
                Key = _ => "kleenestar.core:workspace.updated.label",
                Value = _ => workspace.Updated.ToString("g", CultureInfo.InvariantCulture)
            });

            card.Add(new ControlAttribute("workspace-property-state")
            {
                Icon = _ => new IconTrafficLight(),
                Key = _ => "kleenestar.core:workspace.state.label",
                Value = ctx => I18N.Translate(ctx, workspace.State.Text())
            });

            return card.Render(renderContext, visualTree);
        }
    }
}
