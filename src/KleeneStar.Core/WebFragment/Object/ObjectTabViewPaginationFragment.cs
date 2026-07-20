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
    /// Provides pagination functionality as a footer fragment for object views. Rendered
    /// as a view footer inside the <see cref="ObjectTabViewFragment"/> tab template and the
    /// standalone tab template.
    /// </summary>
    [Section<SectionViewFooterPrimary>]
    [Scope<ObjectTabViewFragment>]
    [Cache]
    public sealed class ObjectTabViewPaginationFragment : FragmentControlViewFooter
    {
        /// <summary>
        /// Unique id that <see cref="ObjectTabViewTableFragment"/> binds its paging source to.
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
        public ObjectTabViewPaginationFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Pagination);
        }

        /// <summary>
        /// Renders the control as an HTML node.
        /// </summary>
        /// <param name="renderContext">
        /// The context in which the control is rendered.
        /// </param>
        /// <param name="visualTree">
        /// The visual tree representing the control's structure.
        /// </param>
        /// <returns>
        /// An HTML node representing the rendered control.
        /// </returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
