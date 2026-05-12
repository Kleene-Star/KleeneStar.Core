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

namespace KleeneStar.Core.WebFragment.Priority
{
    /// <summary>
    /// Represents a fragment control for managing priority tables, providing functionality to 
    /// render the fragment as HTML.
    /// </summary>
    [Section<SectionViewItemPrimary>]
    //[Policy<PriorityViewPolicy>]
    [Scope<PriorityViewFragment>]
    [Cache]
    public sealed class PriorityViewTableFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Gets the table of control view items used to display 
        /// workspace data.
        /// </summary>
        public ControlRestTable Table { get; } = new ControlRestTable()
        {
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Priorities._classid_.Table>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public PriorityViewTableFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconTable(TypeIconTheme.Light);
            Title = _ => "kleenestar.core:view.table.title";
            Table.Bind = _ => new Binding()
                .Add(new BindSearch() { Source = PriorityViewSearchFragment.ContentId })
                .Add(new BindFilter())
                .Add(new BindPaging() { Source = PriorityViewPaginationFragment.ContentId });

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
