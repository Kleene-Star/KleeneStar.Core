using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebFragment.Object;
using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebUri;
using System;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Blog._objectkey_
{
    /// <summary>
    /// The editing view of a single blog post: a full-page form for the post's title and
    /// rich-text body (contributed by
    /// <see cref="WebFragment.Object.ObjectProseEditFormFragment"/>). The URL is
    /// <c>/blog/{objectkey}/edit</c>. Saving persists through the object REST API; the
    /// headline's back link returns to the reading view (<see cref="Index"/>).
    /// </summary>
    /// <remarks>
    /// The <c>{objectkey}</c> segment is declared by the sibling <see cref="Index"/>
    /// page, so this sibling must NOT redeclare it. As on the reading view, an object
    /// whose kind is not <see cref="Model.Entities.ObjectKind.Blog"/> is redirected to
    /// the detail view matching its own kind.
    /// </remarks>
    [WebIcon<IconPen>]
    [Title("kleenestar.core:object.kind.blog.edit.title")]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class Edit : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="objectManager">
        /// The object manager used to retrieve the addressed blog post. Cannot be null.
        /// </param>
        public Edit(IObjectManager objectManager)
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

            if (@object is not null &&
                !string.Equals(@object.Kind, Model.Entities.ObjectKind.Blog, StringComparison.OrdinalIgnoreCase))
            {
                throw new RedirectException(ObjectKindCatalog.ResolveDetailUri(@object));
            }

            // the breadcrumb mirrors the reading view; its object crumb links back to the
            // reading view so the user can leave the editor without saving
            var uri = renderContext.PageContext.ApplicationContext.Route
                .Concat(new WorkspaceKeyUriPathSegmentVariable<WorkspaceKeyParameter>()
                {
                    Value = @object?.Workspace?.Key,
                    Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Blogs._workspacekey_.Index>()
                        .BindParameters(new WorkspaceKeyParameter(@object?.Workspace?.Key))
                })
                .Concat(new ObjectKeyUriPathSegmentVariable<ObjectKeyParameter>()
                {
                    Value = @object?.Key,
                    Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Blog._objectkey_.Index>()
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
