using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Provides the Scrum sprint overview content of the objects index — the active
    /// iteration with progress and burn-down — rendered inside the
    /// <see cref="ObjectTabScrumSprintTemplateFragment"/> tab template. The fragment IS
    /// the sprint control — it derives from the fragment-aware
    /// <see cref="FragmentControlDataScrumSprint"/> base and registers in
    /// <see cref="SectionTabTemplatePrimary"/>, the section the tab template collects
    /// its content from.
    /// </summary>
    [Section<SectionTabTemplatePrimary>]
    [Scope<ObjectTabScrumSprintTemplateFragment>]
    [Order(0)]
    [Cache]
    public sealed class ObjectTabScrumSprintFragment : FragmentControlDataScrumSprint
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectTabScrumSprintFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.ScrumSprint>().ToString());
        }
    }
}
