using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;

namespace KleeneStar.Core.WebFragment.Object.Assets
{
    /// <summary>
    /// Provides the dashboard content of the asset overview, rendered inside the
    /// <see cref="AssetTabDashboardTemplateFragment"/> tab template. The fragment IS the
    /// dashboard control — it derives from <see cref="FragmentControlDataDashboard"/> and
    /// registers in <see cref="SectionTabTemplatePrimary"/>, the section the tab template
    /// collects its content from. Its data comes from the asset dashboard endpoint.
    /// </summary>
    [Section<SectionTabTemplatePrimary>]
    [Scope<AssetTabDashboardTemplateFragment>]
    [Order(0)]
    [Cache]
    public sealed class AssetTabDashboardFragment : FragmentControlDataDashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public AssetTabDashboardFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            ServiceFactory = _ => DataServiceDescriptor.Data(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Assets._workspacekey_.Dashboard>().ToString());
        }
    }
}
