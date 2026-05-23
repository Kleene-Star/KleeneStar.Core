using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Calendar
{
    /// <summary>
    /// Pagination footer fragment for the calendar view.
    /// </summary>
    [Section<SectionViewFooterPrimary>]
    [Scope<CalendarViewFragment>]
    [Cache]
    public sealed class CalendarViewPaginationFragment : FragmentControlViewFooter
    {
        /// <summary>
        /// Stable HTML content id used by the table to bind paging.
        /// </summary>
        public static readonly string ContentId = "id_5E8C92E8D3D34F2AB2E8BF8C69C4DDB2";

        /// <summary>
        /// Gets the pagination control.
        /// </summary>
        public ControlPagination Pagination { get; } = new(ContentId);

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public CalendarViewPaginationFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Pagination);
        }

        /// <summary>
        /// Converts the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>The HTML node.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
