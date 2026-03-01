using KleeneStar.Core.WebParameter;
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
    /// Represents a sidebar item link fragment that displays the 'All' quick filter option in the class sidebar.
    /// </summary>
    [Section<SectionSidebarSecondary>]
    [Scope<WWW.Workspaces._key_.Classes.Index>]
    [Cache]
    public sealed class ClassQuickFilertAllFragment : FragmentControlSidebarItemLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public ClassQuickFilertAllFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = "kleenestar.core:class.quickfilter.all.label";
            Uri = CoreHub.GetUri<WWW.Workspaces._key_.Classes.Index>();
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var categoryParameter = renderContext.Request.GetParameter<CategoryParameter>();

            Active = categoryParameter is null
                ? TypeActive.Active
                : TypeActive.None;

            return base.Render(renderContext, visualTree);
        }
    }
}
