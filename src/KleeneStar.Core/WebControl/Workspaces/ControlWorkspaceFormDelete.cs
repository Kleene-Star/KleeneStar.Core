using KleeneStar.Core.WebParameter.Workspace;
using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebControl.Workspace
{
    /// <summary>
    /// Represents a form for a workspace.
    /// </summary>
    public class ControlWorkspaceFormDelete : ControlRestFormDelete
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ControlWorkspaceFormDelete()
            : this("kleenestar-workspace-form")
        {
            Content.Text = null;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The unique identifier for the form control.</param>
        public ControlWorkspaceFormDelete(string id)
            : base(id)
        {
            Enable = false;
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
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var key = renderContext.Request.GetParameter<KeyParameter>();
            var id = KleeneStar.WorkspaceManager.GetWorkspaceByKey(key?.Value)?
                .Id.ToString();

            return base.Render(renderContext, visualTree, Items, id);
        }
    }
}
