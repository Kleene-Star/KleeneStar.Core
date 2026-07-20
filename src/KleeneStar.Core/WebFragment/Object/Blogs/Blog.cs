using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebFragment.Object.Blogs
{
    /// <summary>
    /// The built-in blog kind: chronological posts that are presented as a
    /// timeline grouped by year and month, newest first.
    /// </summary>
    public sealed class Blog : IObjectKind
    {
        /// <summary>
        /// Gets the persisted kind key of blog posts.
        /// </summary>
        public string Key => Model.Entities.ObjectKind.Blog;

        /// <summary>
        /// Gets the internationalization key of the plural display name.
        /// </summary>
        public string Label => "kleenestar.core:object.kind.blogs.label";

        /// <summary>
        /// Gets the icon representing blog posts.
        /// </summary>
        public IIcon Icon => new IconBlog();

        /// <summary>
        /// Gets the display order; blogs follow the documents.
        /// </summary>
        public int Order => 2;

        /// <summary>
        /// Gets the unbound route of the blog overview page (the blog timeline).
        /// </summary>
        public IUri OverviewUri => CoreHub.GetUri<global::KleeneStar.Core.WWW.Blogs._workspacekey_.Index>();
    }
}
