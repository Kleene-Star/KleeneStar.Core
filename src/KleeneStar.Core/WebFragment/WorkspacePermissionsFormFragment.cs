using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents the permissions management form fragment for a workspace.
    /// Provides controls for selecting groups and policies and assigning permission profiles.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Permissions>]
    [Cache]
    public sealed class WorkspaceEditPermissionsFragment : FragmentControlRestFormEdit
    {
        /// <summary>
        /// Gets the input selection control for the group assignment.
        /// </summary>
        public ControlRestFormItemInputSelection GroupSelection { get; } = new()
        {
            Name = "Group",
            Label = "kleenestar.core:workspace.permissions.group.label",
            Placeholder = "kleenestar.core:workspace.permissions.group.placeholder",
            Help = "kleenestar.core:workspace.permissions.group.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces._workspacekey_.Groups>()
        };

        /// <summary>
        /// Gets the input selection control for the policy assignment.
        /// </summary>
        public ControlRestFormItemInputSelection PolicySelection { get; } = new()
        {
            Name = "Policy",
            Label = "kleenestar.core:workspace.permissions.policy.label",
            Placeholder = "kleenestar.core:workspace.permissions.policy.placeholder",
            Help = "kleenestar.core:workspace.permissions.policy.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces._workspacekey_.Policies>()
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

            Mode = TypeRestFormMode.Edit;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces.Index>();
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

            return base.Render(renderContext, visualTree, Items, id, Uri);
        }
    }
}
