using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a fragment control for managing class tables, providing functionality to 
    /// render the fragment as HTML.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<WWW.Workspaces._key_.Classes.Index>]
    [Cache]
    public sealed class ClassViewFragment : FragmentControlView
    {
        /// <summary>
        /// Returns the search control used to query and filter data.
        /// </summary>
        public ControlAdvancedSearch Search { get; } = new ControlAdvancedSearch()
        {
            RestUri = CoreHub.GetUri<WWW.Api._1_.Workspaces._key_.Classes.Wql>()
        };

        /// <summary>
        /// Returns the table of control view items used to display 
        /// workspace data.
        /// </summary>
        public ControlRestTable Table { get; } = new ControlRestTable()
        {
            RestUri = CoreHub.GetUri<WWW.Api._1_.Workspaces._key_.Classes.Table>()
        };

        /// <summary>
        /// Returns the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ControlRestTile Tile { get; } = new ControlRestTile()
        {
            RestUri = CoreHub.GetUri<WWW.Api._1_.Workspaces._key_.Classes.Tile>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ClassViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(new ControlViewHeader().Add(Search));
            Add(new ControlViewItem().Add(Table));
            Add(new ControlViewItem().Add(Tile));
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            //var key = renderContext.Request.GetParameter<KeyParameter>();
            //var uri = RestUri.SetParameters(key);

            return base.Render(renderContext, visualTree);
        }
    }
}
