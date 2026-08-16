using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
using System;
using System.Collections.Generic;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object.Blogs
{
    /// <summary>
    /// Main-panel content of the blog overview: the stream of the workspace's latest
    /// blog posts, newest first, each rendered as a card with its creation date, its
    /// description, and a link to the full post. The chronological navigation lives in
    /// the sidebar (<see cref="BlogSidebarTimelineFragment"/>).
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Blogs._workspacekey_.Index>]
    [Policy<WorkspaceViewPolicy>]
    [Cache]
    public sealed class BlogStreamFragment : FragmentControlPanel
    {
        /// <summary>
        /// The maximum number of posts shown in the stream.
        /// </summary>
        private const int MaxItems = 10;

        private readonly IObjectManager _objectManager;
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to fetch the workspace posts.</param>
        /// <param name="workspaceManager">The workspace manager used to resolve the workspace from the request.</param>
        public BlogStreamFragment(IFragmentContext fragmentContext, IObjectManager objectManager, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _workspaceManager = workspaceManager;
        }

        /// <summary>
        /// Renders the post stream. Returns <c>null</c> when the fragment's render
        /// conditions exclude it or when no workspace can be resolved from the request.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>The HTML node, or <c>null</c>.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var keyParameter = renderContext?.Request?.GetParameter<WorkspaceKeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            if (workspace is null)
            {
                return null;
            }

            var posts = GetPosts(workspace.Id);

            if (posts.Count == 0)
            {
                var empty = new ControlText("blog-stream-empty")
                {
                    Text = _ => "kleenestar.core:object.kind.blogs.empty",
                    Format = _ => TypeFormatText.Paragraph
                };

                return empty.Render(renderContext, visualTree);
            }

            var stream = new HtmlElementTextContentDiv()
            {
                Id = "blog-stream"
            };

            foreach (var post in posts)
            {
                stream.Add(BuildPostCard(post, renderContext, visualTree));
            }

            return stream;
        }

        /// <summary>
        /// Fetches the workspace's newest blog-kind objects, capped at <see cref="MaxItems"/>.
        /// </summary>
        /// <param name="workspaceId">The owning workspace id.</param>
        /// <returns>The capped, newest-first set of posts. The list may be empty.</returns>
        private IReadOnlyList<Model.Entities.Object> GetPosts(Guid workspaceId)
        {
            var query = new Query<Model.Entities.Object>()
                .WhereEquals(x => x.WorkspaceId, workspaceId)
                .WhereEquals(x => x.Kind, Model.Entities.ObjectKind.Blog)
                .OrderByDesc(x => x.Created)
                .WithPaging(0, MaxItems);

            return [.. _objectManager.GetObjects(query)];
        }

        /// <summary>
        /// Builds the card of a single post: the summary as header, the creation date
        /// and description as body, and a link to the full post.
        /// </summary>
        /// <param name="post">The post to render.</param>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>The rendered card.</returns>
        private static IHtmlNode BuildPostCard(Model.Entities.Object post, IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var id = post.Id.ToString("N");

            var card = new ControlPanelCard("blog-post-" + id)
            {
                Header = _ => post.Summary
            };

            card.Add(new ControlText("blog-post-date-" + id)
            {
                Text = _ => post.Created.ToString("yyyy-MM-dd"),
                Format = _ => TypeFormatText.Small
            });

            card.Add(new ControlText("blog-post-text-" + id)
            {
                Text = _ => post.Description,
                Format = _ => TypeFormatText.Paragraph
            });

            card.Add(new ControlLink("blog-post-open-" + id)
            {
                Text = _ => "kleenestar.core:object.kind.blogs.open.label",
                Icon = _ => (IIcon)post.Icon ?? new IconBlog(TypeIconTheme.Light),
                Uri = _ => ObjectKindCatalog.ResolveDetailUri(post)
            });

            return card.Render(renderContext, visualTree);
        }
    }
}
