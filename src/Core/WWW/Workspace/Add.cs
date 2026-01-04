using KleeneStar.Core.WebControl.Workspace;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Workspace
{
    /// <summary>
    /// Represents a page that provides a form for adding a new workspace within the application.
    /// </summary>
    [WebIcon<IconPlus>]
    [Title("kleenestar.core:workspace.edit.label")]
    [Scope<IScopeGeneral>]
    public sealed class Add : IPage<VisualTreeWebApp>
    {
        /// <summary>
        /// Returns the form used to add a new workspace.
        /// </summary>
        public ControlWorkspaceFormEdit Form { get; } = new();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Add()
        {
            Form.Mode = TypeRestFormMode.New;
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
