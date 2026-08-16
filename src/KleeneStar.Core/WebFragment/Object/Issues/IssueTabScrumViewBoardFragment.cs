using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Object.Issues
{
    /// <summary>
    /// Provides the board view of the Scrum tab — the active-sprint Kanban board — rendered
    /// as a view item of the <see cref="IssueTabScrumViewFragment"/> view control, where it
    /// leads (order 0) ahead of the backlog
    /// (<see cref="IssueTabScrumViewBacklogFragment"/>, order 1).
    /// </summary>
    /// <remarks>
    /// The board carries no query surface of its own: the search and the quickfilter are
    /// header controls of the view, shown for both views, and they write into the shared
    /// state of <see cref="IssueTabScrumViewStateFragment"/>. The board renders the
    /// <see cref="IssueSprintBoardResource"/> that state feeds, so a chip or a search term
    /// re-queries it without a bind wire — which is what the Kanban control needs, having no
    /// <c>BindFilter</c> of its own. It resolves the ViewState by that resource binding, so
    /// it does not have to sit inside the ViewState host.
    ///
    /// The board is the master side of a <see cref="ControlMasterDetail"/> whose frame
    /// shows the selected object's reading view, so a card can be inspected without leaving
    /// the board — the same detail the list view and the backlog offer.
    /// </remarks>
    [Section<SectionViewItemPrimary>]
    [Scope<IssueTabScrumViewFragment>]
    [Order(0)]
    [Cache]
    public sealed class IssueTabScrumViewBoardFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public IssueTabScrumViewBoardFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            var id = fragmentContext?.FragmentId?.ToString()?.Replace(".", "-");

            Icon = _ => new IconTableColumns(TypeIconTheme.Light);
            Title = _ => "kleenestar.core:view.board.title";

            // it shares the same workspace+kind (issue) board configuration as the main
            // Kanban tab, so enabling the full editing surface here edits that same
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
            board.Resource<IssueSprintBoardResource>();

            // the detail is addressed by object id, which is the card id the board reports
            // in its selection event; "{id}" is substituted by the master-detail controller
            var detailUri = $"{CoreHub.GetUri<WWW.Objects.Detail>()}?id={{id}}";

            var masterDetail = new ControlMasterDetail(id + "-masterdetail", board)
            {
                DetailUriTemplate = _ => detailUri,

                // a board needs its width: the columns have a 280px minimum, so a
                // permanently open detail would push them into a horizontal scroll. The
                // view therefore starts as the full-width board and a double click brings
                // the detail in — the backlog beside it opens on a single click, because a
                // row list loses nothing to a narrower master.
                DetailVisible = _ => false,
                Reveal = _ => TypeMasterDetailReveal.DoubleClick,
                MasterInitialSize = _ => 62,
                Styles = ["--wx-master-detail-height: 100%;", "min-height: calc(100vh - 22rem);"]
            };

            masterDetail.Detail = new ControlFrame(id + "-frame")
            {
                Selector = _ => "#wx-content-main"
            };

            Add(masterDetail);
        }

        /// <summary>
        /// Renders the control as an HTML node.
        /// </summary>
        /// <param name="renderContext">
        /// The context in which the control is rendered.
        /// </param>
        /// <param name="visualTree">
        /// The visual tree representing the control's structure.
        /// </param>
        /// <returns>
        /// An HTML node representing the rendered control.
        /// </returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
