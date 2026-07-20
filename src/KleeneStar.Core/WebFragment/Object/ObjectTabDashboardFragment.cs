using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Provides the dashboard content of the objects index, rendered inside the
    /// <see cref="ObjectTabDashboardTemplateFragment"/> tab template. The fragment IS
    /// the dashboard control — it derives from the fragment-aware
    /// <see cref="FragmentControlDataDashboard"/> base and registers in
    /// <see cref="SectionTabTemplatePrimary"/>, the section the tab template collects
    /// its content from.
    /// </summary>
    [Section<SectionTabTemplatePrimary>]
    [Scope<ObjectTabDashboardTemplateFragment>]
    [Order(0)]
    [Cache]
    public sealed class ObjectTabDashboardFragment : FragmentControlDataDashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectTabDashboardFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            ServiceFactory = _ => DataServiceDescriptor.Data(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Dashboard>().ToString());
        }
    }
}
