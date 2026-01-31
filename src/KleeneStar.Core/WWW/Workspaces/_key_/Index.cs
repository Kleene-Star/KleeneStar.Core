using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter.Workspace;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Workspaces._key_
{
    /// <summary>
    /// Provides functionality for managing the current workspace page.
    /// </summary>
    [WebIcon<IconGlobe>]
    [SegmentKey<KeyParameter>()]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="workspaceManager">
        /// The workspace manager used to retrieve workspace information. Cannot be null.
        /// </param>
        public Index(IWorkspaceManager workspaceManager)
        {
            _workspaceManager = workspaceManager;
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<KeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(keyParameter.Value);

            visualTree.Title = workspace?.Name;
            visualTree.Content.MainPanel.Headline.Title = workspace?.Name;
        }
    }
}
