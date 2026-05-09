using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a fragment control for managing priority tables, providing functionality to 
    /// render the fragment as HTML.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Priorities._classid_.Index>]
    [Cache]
    public sealed class PriorityViewFragment : FragmentControlView
    {
        /// <summary>
        /// Gets the search control used to query and filter data.
        /// </summary>
        public ControlAdvancedSearch Search { get; } = new ControlAdvancedSearch()
        {
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Priorities.Wql>()
        };

        /// <summary>
        /// Gets the quick filter control for REST-based field queries.
        /// </summary>
        public ControlRestQuickfilter Quickfilter { get; } = new ControlRestQuickfilter()
        {
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Priorities._classid_.Quickfilter>()
        };

        /// <summary>
        /// Gets the table of control view items used to display 
        /// workspace data.
        /// </summary>
        public ControlRestTable Table { get; } = new ControlRestTable()
        {
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Priorities._classid_.Table>()
        };

        /// <summary>
        /// Gets the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ControlRestTile Tile { get; } = new ControlRestTile()
        {
        };

        /// <summary>
        /// Gets the pagination settings for controlling how data is divided into pages.
        /// </summary>
        public ControlPagination Pagination { get; } = new ControlPagination("id_EA9600F9D85F4E988D2360C686B33DB1")
        {
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public PriorityViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Table.Bind = _ => new Binding()
                .Add(new BindSearch()
                {
                    Source = Search.Id
                })
                .Add(new BindFilter())
                .Add(new BindPaging()
                {
                    Source = Pagination.Id
                });

            Tile.Bind = _ => new Binding()
                .Add(new BindSearch()
                {
                    Source = Search.Id
                })
                .Add(new BindFilter())
                .Add(new BindPaging()
                {
                    Source = Pagination.Id
                });

            Add(new ControlViewHeader().Add(Search, Quickfilter));
            Add(new ControlViewItem()
            {
                Icon = _ => new IconTable()
            }
                .Add(Table));
            //Add(new ControlViewItem()
            //{
            //    Icon = new IconTableCellsLarge()
            //}
            //    .Add(Tile));
            Add(new ControlViewFooter().Add(Pagination));
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
