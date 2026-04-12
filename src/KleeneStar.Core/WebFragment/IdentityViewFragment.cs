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
    /// Represents a fragment control for managing identity tables.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Settings.Identities.Index>]
    [Cache]
    public sealed class IdentityViewFragment : FragmentControlView
    {
        /// <summary>
        /// Gets the search control.
        /// </summary>
        public ControlAdvancedSearch Search { get; } = new ControlAdvancedSearch()
        {
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.Wql>()
        };

        /// <summary>
        /// Gets the quick filter control.
        /// </summary>
        public ControlRestQuickfilter Quickfilter { get; } = new ControlRestQuickfilter()
        {
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.Quickfilter>()
        };

        /// <summary>
        /// Gets the table control.
        /// </summary>
        public ControlRestTable Table { get; } = new ControlRestTable()
        {
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.Table>()
        };

        /// <summary>
        /// Gets the tile control.
        /// </summary>
        public ControlRestTile Tile { get; } = new ControlRestTile()
        {
        };

        /// <summary>
        /// Gets the pagination control.
        /// </summary>
        public ControlPagination Pagination { get; } = new ControlPagination("id_identity_pagination")
        {
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public IdentityViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
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
                Icon = new IconTable()
            }
                .Add(Table));
            Add(new ControlViewFooter().Add(Pagination));
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
