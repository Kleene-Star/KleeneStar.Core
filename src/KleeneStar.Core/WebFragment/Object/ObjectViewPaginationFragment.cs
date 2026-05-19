using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Provides pagination functionality as a footer fragment for object views.
    /// </summary>
    [Section<SectionViewFooterPrimary>]
    [Scope<ObjectViewFragment>]
    [Cache]
    public sealed class ObjectViewPaginationFragment : FragmentControlViewFooter
    {
        /// <summary>
        /// Unique id that <see cref="ObjectViewTableFragment"/> binds its paging source to.
        /// </summary>
        public static readonly string ContentId = "id_401A8BF9454D4448B735B80C99EB1C9E";

        /// <summary>
        /// Gets the pagination control.
        /// </summary>
        public ControlPagination Pagination { get; } = new ControlPagination(ContentId)
        {
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectViewPaginationFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Pagination);
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
