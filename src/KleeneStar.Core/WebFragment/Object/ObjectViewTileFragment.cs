using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Represents a fragment that renders objects in a tile/card view with search, 
    /// filtering, and pagination support.
    /// </summary>
    [Section<SectionViewItemPrimary>]
    [Scope<ObjectViewFragment>]
    [Order(1)]
    [Cache]
    public sealed class ObjectViewTileFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Gets the tile control rendering the objects as cards.
        /// </summary>
        public ControlRestTile Tile { get; } = new ControlRestTile()
        {
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Tile>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectViewTileFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconTile();
            Title = _ => "kleenestar.core:view.tile.title";
            Tile.Bind = _ => new Binding()
                .Add(new BindSearch() { Source = ObjectViewSearchFragment.ContentId })
                .Add(new BindFilter())
                .Add(new BindPaging() { Source = ObjectViewPaginationFragment.ContentId });

            Add(Tile);
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
