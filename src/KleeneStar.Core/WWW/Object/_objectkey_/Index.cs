using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebIcon;
using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebUri;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;

namespace KleeneStar.Core.WWW.Object._objectkey_
{
    /// <summary>
    /// Provides functionality for a object view.
    /// </summary>
    [WebIcon<WorkspaceIcon>]
    [ObjectKeySegment<ObjectKeyParameter>()]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="objectManager">
        /// The object manager used to retrieve object information. Cannot be null.
        /// </param>
        public Index(IObjectManager objectManager)
        {
            _objectManager = objectManager;
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var objectParameter = renderContext.Request.GetParameter<ObjectKeyParameter>();
            var @object = _objectManager.GetObjectByKey(objectParameter?.Value);

            var uri = renderContext.PageContext.ApplicationContext.Route
                .Concat(new WorkspaceKeyUriPathSegmentVariable<WorkspaceKeyParameter>()
                {
                    Value = @object.Workspace?.Key,
                    Uri = CoreHub.GetUri<WWW.Objects._workspacekey_.Index>()
                        .BindParameters(new WorkspaceKeyParameter(@object?.Workspace?.Key))
                })
                .Concat(new ObjectKeyUriPathSegmentVariable<ObjectKeyParameter>()
                {
                    Value = @object?.Key,
                    Uri = CoreHub.GetUri<WWW.Object._objectkey_.Index>()
                        .BindParameters(new ObjectKeyParameter(@object?.Key))
                })
                .ToUri()
                .BindParameters(renderContext.Request);

            visualTree.BreadcrumbUri = uri;
            visualTree.Title = @object?.Summary;
            visualTree.Content.MainPanel.Headline.Title = @object?.Summary;
        }
    }
}
