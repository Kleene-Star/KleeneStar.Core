using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Represents a fragment that displays objects in a REST-enabled table with 
    /// search, filtering, and pagination capabilities.
    /// </summary>
    [Section<SectionViewItemPrimary>]
    [Scope<ObjectViewFragment>]
    [Order(0)]
    [Cache]
    public sealed class ObjectViewTableFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Gets the rest table that displays the object rows.
        /// </summary>
        public ControlDataTable Table { get; } = new ControlDataTable()
        {
            ServiceFactory = _ => DataServiceDescriptor.TableData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Table>().ToString())};

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectViewTableFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconTable(TypeIconTheme.Light);
            Title = _ => "kleenestar.core:view.table.title";
            Table.Bind = _ => new Binding()
                .Add(new BindSearch() { Source = ObjectViewSearchFragment.ContentId })
                .Add(new BindFilter())
                .Add(new BindPaging() { Source = ObjectViewPaginationFragment.ContentId });

            Add(Table);
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
