using System.Net.Http;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;

namespace KleeneStar.Core.WebFragment.Object.Issues
{
    /// <summary>
    /// Provides the Kanban board content of the objects index, rendered inside the
    /// <see cref="IssueTabKanbanTemplateFragment"/> tab template.
    /// </summary>
    /// <remarks>
    /// The board is the master side of a <see cref="ControlMasterDetail"/> whose frame shows
    /// the selected object's reading view, so a card can be inspected without leaving the
    /// board — the same detail the list view and the scrum views offer. Selecting a card is
    /// the board's own behaviour; the composite only owns the detail side.
    ///
    /// The fragment also owns the <see cref="ControlViewState"/> that carries the tab's
    /// query surface. The search (<see cref="IssueTabKanbanSearchFragment"/>) and the
    /// quickfilter (<see cref="IssueTabKanbanQuickfilterFragment"/>) write into it and the
    /// board renders the <see cref="IssueKanbanResource"/> it feeds, so a chip or a search
    /// term re-queries the board — the Kanban control has no <c>BindFilter</c> of its own.
    /// All three resolve the ViewState by resource type, so they may sit in sibling
    /// fragments.
    /// </remarks>
    [Section<SectionTabTemplatePrimary>]
    [Scope<IssueTabKanbanTemplateFragment>]
    [Order(2)]
    [Cache]
    public sealed class IssueTabKanbanFragment : FragmentControlPanel
    {
        /// <summary>
        /// Gets the board control forming the master side of the view.
        /// </summary>
        public ControlDataKanban Board { get; } = new ControlDataKanban()
        {
            // enable the full board editing surface; the endpoint persists every column and
            // swimlane change to the workspace's issue Kanban board configuration
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

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public IssueTabKanbanFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            var id = fragmentContext?.FragmentId?.ToString()?.Replace(".", "-");

            // the ViewState declares the board resource and maps the shared state onto the
            // query parameters the endpoint reads: "q" for the search, "f" for the chips
            var viewState = new ControlViewState<DataQueryState>(id + "-viewstate")
                .State(_ => { })
                .Service<WWW.Api._1_.Objects._workspacekey_.Kanban>(service => service.Method(HttpMethod.Get))
                .Resource<IssueKanbanResource>(resource => resource
                    .Service<WWW.Api._1_.Objects._workspacekey_.Kanban>()
                    .Param("f", "filter")
                    .Param("q", "search"));

            Board.Resource<IssueKanbanResource>();

            // the detail is addressed by object id, which is the card id the board reports
            // in its selection event; "{id}" is substituted by the master-detail controller
            var detailUri = $"{CoreHub.GetUri<WWW.Objects.Detail>()}?id={{id}}";

            var masterDetail = new ControlMasterDetail(id + "-masterdetail", Board)
            {
                DetailUriTemplate = _ => detailUri,

                // a board needs its width: the columns have a 280px minimum, so a
                // permanently open detail would push them into a horizontal scroll. The
                // view therefore starts as the full-width board, a double click brings the
                // detail in, and closing it gives the width back.
                DetailVisible = _ => false,
                Reveal = _ => TypeMasterDetailReveal.DoubleClick,
                MasterInitialSize = _ => 62,
                Styles = ["--wx-master-detail-height: 100%;", "min-height: calc(100vh - 16rem);"],
                Detail = new ControlFrame(id + "-frame")
                {
                    Selector = _ => "#wx-content-main"
                }
            };

            Add(viewState, masterDetail);
        }
    }
}
