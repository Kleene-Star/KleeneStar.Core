using KleeneStar.Core.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Class
{
    /// <summary>
    /// Represents a fragment control for managing class list, providing functionality to 
    /// render the fragment as HTML.
    /// </summary>
    [Section<SectionViewItemPrimary>]
    //[Policy<ClassViewPolicy>]
    [Scope<ClassViewFragment>]
    [Order(2)]
    [Cache]
    public sealed class ClassViewListFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Gets the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ListDetailControl List { get; } = new ListDetailControl()
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._workspacekey_.List>().ToString()),
            Bind = _ => new Binding()
                .Add(new BindSearch() { Source = ClassViewSearchFragment.ContentId })
                .Add(new BindFilter())
                .Add(new BindPaging() { Source = ClassViewPaginationFragment.ContentId })
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ClassViewListFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconSplitFunction(TypeIconTheme.Light);
            Title = _ => "kleenestar.core:view.split.title";

            Add(List);
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

        private IUri GetRestUri(IRenderControlContext renderContext)
        {
            return null;
        }
    }
}
