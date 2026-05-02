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

namespace KleeneStar.Core.WWW.Templates._workspacekey_
{
    /// <summary>
    /// Provides functionality for overview states.
    /// </summary>
    [WebIcon<IconCircleDot>]
    [Title("kleenestar.core:template.manage.title")]
    [Description("kleenestar.core:template.manage.description")]
    [WorkspaceKeySegment]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="classManager">
        /// The class manager used to retrieve class information. Cannot be null.
        /// </param>
        public Index(IClassManager classManager)
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var templateParameter = renderContext.Request.GetParameter<TemplateIdParameter>();
            var guid = Guid.TryParse(templateParameter?.Value, out var id) ? id : Guid.Empty;
            var @class = CoreHub.ClassManager.GetClass(guid);
            var workspace = @class?.Workspace;
            var uri = renderContext.PageContext.ApplicationContext.Route
                .Concat(new WorkspaceKeyUriPathSegmentVariable<WorkspaceKeyParameter>()
                {
                    Value = @class?.Workspace?.Key,
                    Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>()
                        .BindParameters(new WorkspaceKeyParameter(workspace?.Key))
                        .BindParameters(renderContext.Request)
                })
                .Concat(new UriPathSegmentConstant("templates")
                {
                    Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Classes._workspacekey_.Index>()
                        .BindParameters(new WorkspaceKeyParameter(workspace?.Key))
                        .BindParameters(renderContext.Request)
                })
                .Concat(new ClassIdUriPathSegmentVariable<ClassIdParameter>()
                {
                    //Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Template._templateid_.Index>()
                    //    .BindParameters(new WorkspaceKeyParameter(workspace?.Key))
                    //    .BindParameters(renderContext.Request)
                })
                .Concat(new UriPathSegmentConstant("template")
                {
                    Uri = renderContext.Request.Uri
                })
                .ToUri()
                .BindParameters(new WorkspaceKeyParameter(workspace?.Key))
                .BindParameters(renderContext.Request);

            visualTree.BreadcrumbUri = uri;
        }
    }
}
