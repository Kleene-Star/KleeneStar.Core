namespace KleeneStar.Core.WebFragment.Field
{
    /// <summary>
    /// Represents a fragment control for managing field tiles, providing functionality to 
    /// render the fragment as HTML.
    /// </summary>
    //[Section<SectionViewItemPrimary>]
    ////[Policy<FieldViewPolicy>]
    //[Scope<FieldViewFragment>]
    //[Order(1)]
    //[Cache]
    //public sealed class FieldViewTileFragment : FragmentControlViewItem
    //{
    //    /// <summary>
    //    /// Gets the configuration tile that provides REST access to 
    //    /// workspace data.
    //    /// </summary>
    //    public ControlRestTile Tile { get; } = new ControlRestTile()
    //    {
    //        RestUri = _ => null
    //    };

    //    /// <summary>
    //    /// Initializes a new instance of the class.
    //    /// </summary>
    //    /// <param name="fragmentContext">The context of the fragment.</param>
    //    public FieldViewTileFragment(IFragmentContext fragmentContext)
    //        : base(fragmentContext)
    //    {
    //        Icon = _ => new IconTile();
    //        Title = _ => "kleenestar.core:view.tile.title";
    //        Tile.Bind = _ => new Binding()
    //            .Add(new BindSearch() { Source = FieldViewSearchFragment.ContentId })
    //            .Add(new BindFilter())
    //            .Add(new BindPaging() { Source = FieldViewPaginationFragment.ContentId });

    //        Add(Tile);
    //    }

    //    /// <summary>
    //    /// Convert the fragment to HTML.
    //    /// </summary>
    //    /// <param name="renderContext">The context in which the fragment is rendered.</param>
    //    /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
    //    /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
    //    public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
    //    {
    //        return base.Render(renderContext, visualTree);
    //    }
    //}
}
