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

namespace KleeneStar.Core.WebFragment.Sla
{
    /// <summary>
    /// Renders the SLA-policy table inside the SLA <see cref="SlaViewFragment"/>.
    /// </summary>
    [Section<SectionViewItemPrimary>]
    [Scope<SlaViewFragment>]
    [Cache]
    public sealed class SlaViewTableFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Gets the REST-backed table control.
        /// </summary>
        public ControlDataTable Table { get; } = new()
        {
            ServiceFactory = _ => DataServiceDescriptor.TableData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Slas._classid_.Table>().ToString())};

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public SlaViewTableFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconTable(TypeIconTheme.Light);
            Title = _ => "kleenestar.core:view.table.title";

            Table.Bind = _ => new Binding()
                .Add(new BindSearch() { Source = SlaViewSearchFragment.ContentId })
                .Add(new BindFilter())
                .Add(new BindPaging() { Source = SlaViewPaginationFragment.ContentId });

            Add(Table);
        }

        /// <summary>
        /// Renders the fragment.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
