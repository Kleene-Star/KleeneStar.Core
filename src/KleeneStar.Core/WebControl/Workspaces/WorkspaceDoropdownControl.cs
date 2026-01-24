using WebExpress.WebApp.WebApiControl;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebControl.Workspace
{
    /// <summary>
    /// Represents a dropdown control for selecting a workspace.
    /// </summary>
    public class WorkspaceDoropdownControl : ControlRestDropdown
    {
        /// <summary>
        /// Returns the control link for adding a new workspace.
        /// </summary>
        public ControlDropdownItemLink AddWorkspace { get; } = new()
        {
            Text = "kleenestar.core:workspace.add.label",
            Icon = new IconPlus(),
            Modal = new ModalTarget("modal-form", TypeModalSize.ExtraLarge),
            Uri = CoreHub.GetUri<WWW.Workspaces.Add>()
        };

        /// <summary>
        /// Returns the control link for managing workspaces.
        /// </summary>
        public ControlDropdownItemLink ManageWorkspace { get; } = new()
        {
            Text = "kleenestar.core:workspace.manage.label",
            Uri = CoreHub.GetUri<WWW.Workspaces.Index>(),
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The unique identifier for the dropdown control.</param>
        public WorkspaceDoropdownControl(string id)
            : base(id)
        {
            RestUri = CoreHub.GetUri<WWW.Api._1_.Workspaces.Dropdown>();

            Add(AddWorkspace);
            Add(ManageWorkspace);
        }

        /// <summary>
        /// Converts the control to an HTML representation.
        /// </summary>
        /// <param name="renderContext">The context in which the control is rendered.</param>
        /// <param name="visualTree">The visual tree representing the control's structure.</param>
        /// <returns>An HTML node representing the rendered control.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
