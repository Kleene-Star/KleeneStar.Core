using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Sla
{
    /// <summary>
    /// Top-level view container that hosts the SLA-policy table on the overview page.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Slas._classid_.Index>]
    [Cache]
    public sealed class SlaViewFragment : FragmentControlView
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public SlaViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Layout = _ => TypeLayoutView.ToggleGroup;
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
