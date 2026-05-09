using WebExpress.WebApp.WebApiControl;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebControl
{
    /// <summary>
    /// Represents a dropdown control for selecting a workspace.
    /// </summary>
    public class WorkspaceDropdownControl : ControlRestDropdown
    {
        /// <summary>
        /// Gets the control link for adding a new workspace.
        /// </summary>
        public ControlDropdownItemLink AddWorkspace { get; } = new()
        {
            Text = _ => "kleenestar.core:workspace.add.label",
            Icon = _ => new IconPlus(TypeIconTheme.Light),
            PrimaryAction = _ => new ActionModal("modal-form", CoreHub.GetUri<WWW.Workspaces.Add>(), TypeModalSize.ExtraLarge),
        };

        /// <summary>
        /// Gets the control link for managing workspaces.
        /// </summary>
        public ControlDropdownItemLink ManageWorkspace { get; } = new()
        {
            Text = _ => "kleenestar.core:workspace.manage.label",
            Uri = _ => CoreHub.GetUri<WWW.Workspaces.Index>(),
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The unique identifier for the dropdown control.</param>
        public WorkspaceDropdownControl(string id)
            : base(id)
        {
            RestUri = _ => CoreHub.GetUri<WWW.Api._1_.Workspaces.Dropdown>();

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
