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

namespace KleeneStar.Core.WebFragment.Tenant
{
    /// <summary>
    /// Represents a fragment control for managing tenant tables, providing functionality to
    /// render the fragment as HTML.
    /// </summary>
    [Section<SectionViewItemPrimary>]
    [Scope<TenantViewFragment>]
    [Cache]
    public sealed class TenantViewTableFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Gets the table of control view items used to display
        /// tenant data.
        /// </summary>
        public ControlDataTable Table { get; } = new ControlDataTable()
        {
            ServiceFactory = _ => DataServiceDescriptor.TableData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Tenants.Table>().ToString())
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public TenantViewTableFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconTable(TypeIconTheme.Light);
            Title = _ => "kleenestar.core:view.table.title";
            Table.Bind = _ => new Binding()
                .Add(new BindSearch() { Source = TenantViewSearchFragment.ContentId })
                .Add(new BindFilter())
                .Add(new BindPaging() { Source = TenantViewPaginationFragment.ContentId });

            Add(Table);
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
