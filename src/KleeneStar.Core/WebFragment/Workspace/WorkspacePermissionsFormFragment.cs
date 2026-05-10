using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workspace
{
    /// <summary>
    /// Represents the permissions management form fragment for a workspace.
    /// Provides controls for selecting groups and policies and assigning permission profiles.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Permissions>]
    [Policy<WorkspaceAdminPolicy>]
    [Cache]
    public sealed class WorkspaceEditPermissionsFragment : FragmentControlRestFormEdit
    {
        /// <summary>
        /// Gets the input selection control for the group assignment.
        /// </summary>
        public ControlRestFormItemInputSelection GroupSelection { get; } = new()
        {
            Name = _ => "Group",
            Label = _ => "kleenestar.core:workspace.permissions.group.label",
            Placeholder = _ => "kleenestar.core:workspace.permissions.group.placeholder",
            Help = _ => "kleenestar.core:workspace.permissions.group.help",
            Required = _ => true,
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces._workspacekey_.Groups>()
        };

        /// <summary>
        /// Gets the input selection control for the policy assignment.
        /// </summary>
        public ControlRestFormItemInputSelection PolicySelection { get; } = new()
        {
            Name = _ => "Policy",
            Label = _ => "kleenestar.core:workspace.permissions.policy.label",
            Placeholder = _ => "kleenestar.core:workspace.permissions.policy.placeholder",
            Help = _ => "kleenestar.core:workspace.permissions.policy.help",
            Required = _ => true,
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces._workspacekey_.Policies>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public WorkspaceEditPermissionsFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(GroupSelection);
            Add(PolicySelection);

            Mode = _ => TypeRestFormMode.Edit;
            Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces.Index>();
        }

        /// <summary>
        /// Renders the control as an HTML node.
        /// </summary>
        /// <param name="renderContext">
        /// The context in which the control is rendered.
        /// </param>
        /// <param name="visualTree">
        /// The visual tree representing the control's structure.
        /// </param>
        /// <returns>
        /// An HTML node representing the rendered control.
        /// </returns>
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            var key = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(key?.Value);
            var id = workspace?.Id.ToString();

            return base.Render(renderContext, visualTree);
        }
    }
}
