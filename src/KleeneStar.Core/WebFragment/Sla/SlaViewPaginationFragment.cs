using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Sla
{
    /// <summary>
    /// Pagination footer fragment for the SLA-policy view.
    /// </summary>
    [Section<SectionViewFooterPrimary>]
    [Scope<SlaViewFragment>]
    [Cache]
    public sealed class SlaViewPaginationFragment : FragmentControlViewFooter
    {
        /// <summary>
        /// Stable HTML content id used by the table to bind paging.
        /// </summary>
        public static readonly string ContentId = "id_C2D7E7DEF3A540A6BC4DAB9AB3E3F2B6";

        /// <summary>
        /// Gets the pagination control.
        /// </summary>
        public ControlPagination Pagination { get; } = new(ContentId);

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public SlaViewPaginationFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Pagination);
        }

        /// <summary>
        /// Renders the fragment.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
