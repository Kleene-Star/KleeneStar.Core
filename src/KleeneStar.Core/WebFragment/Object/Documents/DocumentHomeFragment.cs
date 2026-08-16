using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
using System.Collections.Generic;
using System.Linq;
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
    /// Main-panel content of the document overview: shows the workspace's home
    /// document — the first root of the document tree, ordered by summary — with its
    /// description and a link to the full document. The tree itself lives in the
    /// sidebar (<see cref="DocumentSidebarTreeFragment"/>), so the page opens like a
    /// wiki space: navigation on the left, the home page in the middle.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Documents._workspacekey_.Index>]
    [Policy<WorkspaceViewPolicy>]
    [Cache]
    public sealed class DocumentHomeFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to fetch the workspace documents.</param>
        /// <param name="workspaceManager">The workspace manager used to resolve the workspace from the request.</param>
        public DocumentHomeFragment(IFragmentContext fragmentContext, IObjectManager objectManager, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
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

            var home = GetHomeDocument(workspace.Id);

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

            card.Add(new ControlText("document-home-description")
            {
                Text = _ => home.Description,
                Format = _ => TypeFormatText.Paragraph
            });

            card.Add(new ControlLink("document-home-open")
            {
                Text = _ => "kleenestar.core:object.kind.documents.open.label",
                Icon = _ => (IIcon)home.Icon ?? new IconFileLines(TypeIconTheme.Light),
                Uri = _ => ObjectKindCatalog.ResolveDetailUri(home)
            });

            return card.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Resolves the workspace's home document: the first root of the document tree,
        /// ordered by summary. Returns <see langword="null"/> when the workspace holds
        /// no documents.
        /// </summary>
        /// <param name="workspaceId">The owning workspace id.</param>
        /// <returns>The home document, or <see langword="null"/>.</returns>
        private Model.Entities.Object GetHomeDocument(System.Guid workspaceId)
        {
            var query = new Query<Model.Entities.Object>()
                .WhereEquals(x => x.WorkspaceId, workspaceId)
                .WhereEquals(x => x.Kind, Model.Entities.ObjectKind.Document)
                .OrderByAsc(x => x.Summary);

            var documents = (IReadOnlyList<Model.Entities.Object>)[.. _objectManager.GetObjects(query)];
            var ids = documents.Select(x => x.Id).ToHashSet();

            return documents.FirstOrDefault(x => !x.ParentId.HasValue || !ids.Contains(x.ParentId.Value))
                ?? documents.FirstOrDefault();
        }
    }
}
