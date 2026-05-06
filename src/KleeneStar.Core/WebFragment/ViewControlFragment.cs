using KleeneStar.Core.WebControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Provides a base class for fragments that display and manage workspace data using advanced search, quick
    /// filtering, tabular, tile, and list views, as well as pagination controls.
    /// </summary>
    public abstract class ViewControlFragment : FragmentControlView
    {
        /// <summary>
        /// Gets the search control used to query and filter data.
        /// </summary>
        public ControlAdvancedSearch Search { get; } = new ControlAdvancedSearch()
        {
        };

        /// <summary>
        /// Gets the quick filter control for REST-based class queries.
        /// </summary>
        public ControlRestQuickfilter Quickfilter { get; } = new ControlRestQuickfilter()
        {
        };

        /// <summary>
        /// Gets the table of control view items used to display 
        /// workspace data.
        /// </summary>
        public ControlRestTable Table { get; } = new ControlRestTable()
        {
        };

        /// <summary>
        /// Gets the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ControlRestTile Tile { get; } = new ControlRestTile()
        {
        };

        /// <summary>
        /// Gets the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ListDetailControl List { get; } = new ListDetailControl()
        {
        };

        /// <summary>
        /// Gets the pagination settings for controlling how data is divided into pages.
        /// </summary>
        public ControlPagination Pagination { get; } = new ControlPagination("id_55964D2DB5334B8D96EE07A9C9BE450B")
        {
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ViewControlFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Layout = _ => TypeLayoutView.ToggleGroup;

            Table.Bind = new Binding()
                .Add(new BindSearch()
                {
                    Source = Search.Id
                })
                .Add(new BindFilter())
                .Add(new BindPaging()
                {
                    Source = Pagination.Id
                });

            Tile.Bind = new Binding()
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
                Icon = _ => new IconBarsVertical(),
                Title = _ => "kleenestar.core:view.table.title"
            }
                .Add(Table));
            Add(new ControlViewItem()
            {
                Icon = _ => new IconTile(),
                Title = _ => "kleenestar.core:view.tile.title"
            }
                .Add(Tile));
            Add(new ControlViewItem()
            {
                Icon = _ => new IconSplitFunction(TypeIconTheme.Light),
                Title = _ => "kleenestar.core:view.split.title"
            }
                .Add(List));
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
