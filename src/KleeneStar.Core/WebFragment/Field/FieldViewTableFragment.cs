using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Field
{
    /// <summary>
    /// Represents a fragment control for managing field tables, providing functionality to
    /// render the fragment as HTML.
    /// </summary>
    [Section<SectionViewItemPrimary>]
    //[Policy<FieldViewPolicy>]
    [Scope<FieldViewFragment>]
    [Cache]
    public sealed class FieldViewTableFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Gets the table of control view items used to display
        /// workspace data.
        /// </summary>
        public ControlDataTable Table { get; } = new ControlDataTable()
        {
            PageSize = _ => 25,
            ServiceFactory = _ => WebExpress.WebApp.WebData.DataServiceDescriptor.TableData
            (
                CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields._classid_.Table>().ToString()
            )
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public FieldViewTableFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconTable(TypeIconTheme.Light);
            Title = _ => "kleenestar.core:view.table.title";
            Table.Bind = _ => new Binding()
                .Add(new BindSearch() { Source = FieldViewSearchFragment.ContentId })
                .Add(new BindFilter())
                .Add(new BindPaging() { Source = FieldViewPaginationFragment.ContentId });

            Add(Table);
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
