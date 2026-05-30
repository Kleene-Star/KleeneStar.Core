using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
using KleeneStar.Model.Entities;
using System;
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
    /// Workspace-scoped property card that groups the structural configuration of the
    /// workspace (access modifier, sealed flag, inheritance chain) inside a single
    /// <see cref="ControlPanelCard"/>.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Policy<WorkspaceViewPolicy>]
    [Order(10)]
    [Cache]
    public sealed class WorkspacePropertyConfigurationCardFragment : FragmentControlPanel
    {
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WorkspacePropertyConfigurationCardFragment(IFragmentContext fragmentContext, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _workspaceManager = workspaceManager;
        }

        /// <summary>
        /// Renders the configuration card. Returns <c>null</c> when no workspace can be
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

            var card = new ControlPanelCard("workspace-property-configuration-card")
            {
                Header = _ => "kleenestar.core:workspace.property.configuration.header",
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two)
            };

            card.Add(new ControlAttribute("workspace-property-accessmodifier")
            {
                Icon = _ => new IconLock(),
                Key = _ => "kleenestar.core:workspace.accessmodifier.label",
                Value = ctx => I18N.Translate(ctx, workspace.AccessModifier.Text())
            });

            card.Add(new ControlAttribute("workspace-property-sealed")
            {
                Icon = _ => new IconLock(),
                Key = _ => "kleenestar.core:workspace.sealed.label",
                Value = ctx => I18N.Translate(ctx, workspace.Sealed
                    ? "kleenestar.core:workspace.property.yes"
                    : "kleenestar.core:workspace.property.no")
            });

            card.Add(new ControlAttribute("workspace-property-inherited")
            {
                Icon = _ => new IconCodeBranch(),
                Key = _ => "kleenestar.core:workspace.inherited.label",
                Value = ctx => ResolveInheritedName(ctx, workspace)
            });

            return card.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Resolves the display name of the workspace this workspace inherits from.
        /// Returns the localized "none" placeholder when no inheritance is configured.
        /// </summary>
        private string ResolveInheritedName(IRenderControlContext renderContext, Model.Entities.Workspace workspace)
        {
            if (workspace.InheritedId is null || workspace.InheritedId.Value == Guid.Empty)
            {
                return I18N.Translate(renderContext, "kleenestar.core:workspace.property.none");
            }

            var inherited = _workspaceManager.GetWorkspace(workspace.InheritedId.Value);
            return inherited?.Name ?? I18N.Translate(renderContext, "kleenestar.core:workspace.property.none");
        }
    }
}
