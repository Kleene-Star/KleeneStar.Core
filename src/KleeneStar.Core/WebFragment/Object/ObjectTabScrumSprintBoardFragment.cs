using System.Net.Http;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebFragment;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Provides the active-sprint board section of the Scrum sprint tab: a personal-scope
    /// quickfilter above the active-sprint Kanban board, rendered inside the
    /// <see cref="ObjectTabScrumSprintTemplateFragment"/> tab template below the scrum team
    /// workload (order 0) and the sprint burn-down (order 1).
    /// </summary>
    /// <remarks>
    /// The Kanban control has no <c>BindFilter</c>, so the quickfilter drives it through a
    /// <see cref="ControlViewState"/>: the ViewState declares the
    /// <see cref="SprintBoardResource"/> backed by the sprint Kanban endpoint, the
    /// quickfilter writes the active chip set into the shared state (the <c>filter</c> path,
    /// mapped to the endpoint's <c>f</c> parameter), and the Kanban renders the resource — so
    /// a chip selection re-queries the board without a bind wire. The quickfilter and the
    /// board resolve the ViewState by resource type, so they may sit beside it in the panel.
    /// </remarks>
    [Section<SectionTabTemplatePrimary>]
    [Scope<ObjectTabScrumSprintTemplateFragment>]
    [Order(2)]
    [Cache]
    public sealed class ObjectTabScrumSprintBoardFragment : FragmentControlPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectTabScrumSprintBoardFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            var id = fragmentContext?.FragmentId?.ToString()?.Replace(".", "-");

            // the ViewState declares the board resource (backed by the sprint Kanban
            // endpoint) and maps the shared state's "filter" onto the endpoint's "f"
            // query parameter
            var viewState = new ControlViewState<DataQueryState>(id + "-viewstate")
                .State(_ => { })
                .Service<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.ScrumSprintKanban>(service => service.Method(HttpMethod.Get))
                .Resource<SprintBoardResource>(resource => resource
                    .Service<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.ScrumSprintKanban>()
                    .Param("f", "filter"));

            // the quickfilter loads its chips from its own service and, bound to the board
            // resource, writes the active filter into the shared state and re-queries it
            var quickfilter = new ControlDataQuickfilter(id + "-quickfilter")
                .DataService<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.ScrumSprintQuickfilter>();
            quickfilter.Resource<SprintBoardResource>().Model("filter");

            // the board renders the resource and re-renders whenever the quickfilter
            // re-queries it; it shares the same workspace+kind (issue) board configuration as
            // the main Kanban tab, so enabling the full editing surface here edits that same
            // persisted board
            var board = new ControlDataKanban(id + "-board")
            {
                EditableColumn = _ => true,
                MovableColumn = _ => true,
                DeletableColumn = _ => true,
                AddableColumn = _ => true,
                AddableSwimlane = _ => true,
                EditableSwimlane = _ => true,
                DeletableSwimlane = _ => true,
                MovableSwimlane = _ => true,
                ConfigurableBoard = _ => true,
                ConfigurableSwimlane = _ => true
            };
            board.Resource<SprintBoardResource>();

            Add(viewState, quickfilter, board);
        }
    }
}
