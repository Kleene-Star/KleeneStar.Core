using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;

namespace KleeneStar.Core.WebFragment.Object.Assets
{
    /// <summary>
    /// Provides the Kanban board content of the asset overview, rendered inside the
    /// <see cref="AssetTabKanbanTemplateFragment"/> tab template. The fragment IS the board
    /// control — it derives from <see cref="FragmentControlDataKanban"/> and registers in
    /// <see cref="SectionTabTemplatePrimary"/>, the section the tab template collects its
    /// content from. Its data comes from the asset Kanban endpoint.
    /// </summary>
    [Section<SectionTabTemplatePrimary>]
    [Scope<AssetTabKanbanTemplateFragment>]
    [Order(0)]
    [Cache]
    public sealed class AssetTabKanbanFragment : FragmentControlDataKanban
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public AssetTabKanbanFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            ServiceFactory = _ => DataServiceDescriptor.Data(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Assets._workspacekey_.Kanban>().ToString());

            // enable the full board editing surface; the endpoint persists every column and
            // swimlane change to the workspace's asset Kanban board configuration
            EditableColumn = _ => true;
            MovableColumn = _ => true;
            DeletableColumn = _ => true;
            AddableColumn = _ => true;
            AddableSwimlane = _ => true;
            EditableSwimlane = _ => true;
            DeletableSwimlane = _ => true;
            MovableSwimlane = _ => true;
            ConfigurableBoard = _ => true;
            ConfigurableSwimlane = _ => true;
        }
    }
}
