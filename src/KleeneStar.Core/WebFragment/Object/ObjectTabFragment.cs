using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebScope;
using Api = KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Renders the workspace issue overview as a REST-backed tab control. The tab set is
    /// loaded from the <see cref="Api.Tab"/> REST endpoint, so the persisted object views
    /// of the workspace appear as movable, closable tabs and new views can be added from
    /// the template picker. The tab templates attach themselves via
    /// <c>[Scope&lt;ObjectTabFragment&gt;]</c>; the leading one is the
    /// <see cref="Issues.IssueViewTemplateFragment"/> with the curated issue list, followed
    /// by the <see cref="ObjectTabViewTemplateFragment"/>, which composes the classic
    /// object view from the table, tile, list, search, quickfilter and pagination
    /// fragments.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Issues._workspacekey_.Index>]
    [Cache]
    public sealed class ObjectTabFragment : FragmentControlDataTab, IScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectTabFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            MovableTab = _ => true;
            ServiceFactory = _ => DataServiceDescriptor.TabData(CoreHub.GetUri<Api.Tab>().ToString());
        }
    }
}
