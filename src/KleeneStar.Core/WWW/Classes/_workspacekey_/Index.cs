using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebIcon;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebUri;
using KleeneStar.Model.Entities;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;

namespace KleeneStar.Core.WWW.Classes._workspacekey_
{
    /// <summary>
    /// Represents the main class management page within the kleenestar web application.
    /// </summary>
    [WebIcon<ClassIcon>]
    [Title("kleenestar.core:class.manage.label")]
    [WorkspaceKeySegment<WorkspaceKeyParameter>()]
    [Scope<IScopeGeneral>]
    [Domain<Class>]
    [Cache]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            var uri = renderContext.PageContext.ApplicationContext.Route
                .Concat(new WorkspaceKeyUriPathSegmentVariable<WorkspaceKeyParameter>()
                {
                    Value = workspace?.Key,
                    Uri = CoreHub.GetUri<WWW.Objects._workspacekey_.Index>()
                    .BindParameters(renderContext.Request)
                })
                .Concat(new ClassIdUriPathSegmentVariable<ObjectKeyParameter>()
                {
                    Uri = renderContext.Request.Uri
                })
                .ToUri()
                .BindParameters(renderContext.Request);

            visualTree.BreadcrumbUri = uri;
        }
    }
}
