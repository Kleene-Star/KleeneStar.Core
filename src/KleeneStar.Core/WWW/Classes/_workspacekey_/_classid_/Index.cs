using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebUri;
using System;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Classes._workspacekey_._classid_
{
    /// <summary>
    /// Provides functionality for managing the current class page.
    /// </summary>
    [WebIcon<IconGlobe>]
    [ClassSegment<ClassIdParameter>()]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        private readonly IClassManager _classManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="classManager">
        /// The class manager used to retrieve class information. Cannot be null.
        /// </param>
        public Index(IClassManager classManager)
        {
            _classManager = classManager;
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var classParameter = renderContext.Request.GetParameter<ClassIdParameter>();
            var guid = Guid.TryParse(classParameter.Value, out var id) ? id : Guid.Empty;
            var @class = _classManager.GetClass(guid);

            visualTree.Title = @class?.Name;
            visualTree.Content.MainPanel.Headline.Title = @class?.Name;

            var uri = renderContext.PageContext.ApplicationContext.Route
                .Concat(new WorkspaceKeyUriPathSegmentVariable<WorkspaceKeyParameter>()
                {
                    Value = @class?.Workspace?.Key,
                    Uri = CoreHub.GetUri<WWW.Objects._workspacekey_.Index>()
                        .BindParameters(renderContext.Request)
                })
                .Concat(new UriPathSegmentConstant("classes")
                {
                    Uri = CoreHub.GetUri<WWW.Classes._workspacekey_.Index>()
                        .BindParameters(renderContext.Request)
                })
                .Concat(new ClassIdUriPathSegmentVariable<ClassIdParameter>()
                {
                    Uri = renderContext.Request.Uri
                })
                .ToUri()

                .BindParameters(renderContext.Request);

            visualTree.BreadcrumbUri = uri;
        }
    }
}
