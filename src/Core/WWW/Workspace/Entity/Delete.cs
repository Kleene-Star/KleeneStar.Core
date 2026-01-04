using KleeneStar.Core.WebControl.Workspace;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Workspace.Entity
{
    /// <summary>
    /// Represents the page for creating or editing a workspace within the web application. Provides access to the
    /// workspace edit form and handles form processing and rendering.
    /// </summary>
    [WebIcon<IconTrashAlt>]
    [Title("kleenestar.core:workspace.delete.label")]
    [Scope<IScopeGeneral>]
    public sealed class Delete : IPage<VisualTreeWebApp>
    {
        /// <summary>
        /// Returns the form used to add a new workspace.
        /// </summary>
        public ControlWorkspaceFormDelete Form { get; } = new();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Delete()
        {
            Form.Mode = TypeRestFormMode.Delete;
            Form.Uri = KleeneStar.GetUri<WWW.Api._1.Workspaces.Index>();
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            visualTree.Content.MainPanel.AddPrimary(Form);
        }
    }
}
