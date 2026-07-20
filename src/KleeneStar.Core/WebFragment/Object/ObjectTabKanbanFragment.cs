using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Provides the Kanban board content of the objects index, rendered inside the
    /// <see cref="ObjectTabKanbanTemplateFragment"/> tab template. The fragment IS the
    /// board control — it derives from the fragment-aware
    /// <see cref="FragmentControlDataKanban"/> base and registers in
    /// <see cref="SectionTabTemplatePrimary"/>, the section the tab template collects
    /// its content from.
    /// </summary>
    [Section<SectionTabTemplatePrimary>]
    [Scope<ObjectTabKanbanTemplateFragment>]
    [Order(0)]
    [Cache]
    public sealed class ObjectTabKanbanFragment : FragmentControlDataKanban
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectTabKanbanFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            ServiceFactory = _ => DataServiceDescriptor.Data(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Kanban>().ToString());
        }
    }
}
