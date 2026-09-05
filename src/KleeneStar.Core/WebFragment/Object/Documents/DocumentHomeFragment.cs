using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object.Documents
{
    /// <summary>
    /// Main-panel content of the document overview: shows the workspace's home document with
    /// its description and a link to the full document. The tree itself lives in the sidebar
    /// (<see cref="DocumentSidebarTreeFragment"/>), so the page opens like a wiki space:
    /// navigation on the left, the home page in the middle.
    /// </summary>
    /// <remarks>
    /// Which document that is belongs to <see cref="IWorkspaceManager.GetHome"/>: the one
    /// chosen through the document's own more menu, and failing that the first root of the page
    /// tree by summary — which is what this fragment used to decide on its own, and is an
    /// accident of alphabetical order. It moved to the manager because the choice is now asked
    /// about in two places, and a second implementation of "which one is it" would answer
    /// differently the first time somebody renamed a page.
    /// </remarks>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Documents._workspacekey_.Index>]
    [Policy<WorkspaceViewPolicy>]
    [Cache]
    public sealed class DocumentHomeFragment : FragmentControlPanel
    {
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="workspaceManager">The workspace manager used to resolve the workspace from the request.</param>
        public DocumentHomeFragment(IFragmentContext fragmentContext, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _workspaceManager = workspaceManager;
        }

        /// <summary>
        /// Renders the home document. Returns <c>null</c> when the fragment's render
        /// conditions exclude it or when no workspace can be resolved from the request.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>The HTML node, or <c>null</c>.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var keyParameter = renderContext?.Request?.GetParameter<WorkspaceKeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            if (workspace is null)
            {
                return null;
            }

            var home = _workspaceManager.GetHome(workspace.Id);

            if (home is null)
            {
                var empty = new ControlText("document-home-empty")
                {
                    Text = _ => "kleenestar.core:object.kind.documents.empty",
                    Format = _ => TypeFormatText.Paragraph
                };

                return empty.Render(renderContext, visualTree);
            }

            var card = new ControlPanelCard("document-home-card")
            {
                Header = _ => home.Summary
            };

            // opening the page is a headline button beside the more menu
            // (DocumentHomeOpenButtonFragment), not a link in the body of the card: it is the
            // one thing a reader of this preview wants next, and an action buried in a card is
            // found late
            card.Add(new ControlText("document-home-description")
            {
                Text = _ => home.Description,
                Format = _ => TypeFormatText.Paragraph
            });

            return card.Render(renderContext, visualTree);
        }
    }
}
