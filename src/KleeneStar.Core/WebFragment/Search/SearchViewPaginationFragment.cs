using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Search
{
    /// <summary>
    /// Provides the pagination footer of the global search results table.
    /// </summary>
    [Section<SectionViewFooterPrimary>]
    [Scope<SearchViewFragment>]
    [Cache]
    public sealed class SearchViewPaginationFragment : FragmentControlViewFooter
    {
        /// <summary>
        /// Represents the unique identifier for the pagination content. Referenced by the
        /// table fragment's <c>BindPaging.Source</c>.
        /// </summary>
        public static readonly string ContentId = "id_5C8F2DAB7E3041649203B4C5D6E7F801";

        /// <summary>
        /// Gets the pagination settings for controlling how the results are divided into pages.
        /// </summary>
        public ControlPagination Pagination { get; } = new ControlPagination(ContentId)
        {
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public SearchViewPaginationFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Pagination);
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
