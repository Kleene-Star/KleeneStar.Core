using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPolicies;
using System;
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
    /// The document-tree section of the document overview sidebar: a section header
    /// ("Documents") followed by the workspace's documents as hierarchical sidebar
    /// links mirroring the parent/child containment. The section sits below the kind
    /// links (Documents, Blogs, Issues), which stay ordinary flat links; every tree
    /// node links to the object detail page.
    /// </summary>
    /// <remarks>
    /// The documents are fetched once per render, capped at <see cref="MaxItems"/>
    /// (the "top 200") to keep the sidebar responsive, and assembled into nested links
    /// in memory: every document whose parent is absent from the fetched set is
    /// promoted to a root entry. A visited set guards the recursion against cycles
    /// persisted by older data. The header is always visible; without documents a
    /// disabled empty entry is shown instead of the tree, so the section communicates
    /// where new documents will appear. The fragment renders header and root entries
    /// as siblings via <see cref="HtmlList"/>, so the sidebar parser picks each of
    /// them up as a regular sidebar item.
    /// </remarks>
    [Section<SectionSidebarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Documents._workspacekey_.Index>]
    [Policy<WorkspaceViewPolicy>]
    [Order(10)]
    [Cache]
    public sealed class DocumentSidebarTreeFragment : FragmentControlSidebarItemLink
    {
        /// <summary>
        /// The maximum number of documents fetched for the tree ("top 200").
        /// </summary>
        private const int MaxItems = 200;

        private readonly IObjectManager _objectManager;
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services
        /// for its operation. Cannot be null.
        /// </param>
        /// <param name="objectManager">
        /// The object manager used to retrieve the workspace documents. Cannot be null.
        /// </param>
        /// <param name="workspaceManager">
        /// The workspace manager used to resolve the workspace from the request. Cannot be null.
        /// </param>
        public DocumentSidebarTreeFragment(IFragmentContext fragmentContext, IObjectManager objectManager, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _workspaceManager = workspaceManager;
        }

        /// <summary>
        /// Renders the section: the header followed by the root entries of the document
        /// tree, or — when the workspace holds no documents — by a disabled empty
        /// entry. Returns <c>null</c> only when the fragment's render conditions
        /// exclude it.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragment, or <c>null</c> when suppressed.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var documents = GetDocuments(renderContext);

            var header = new ControlSidebarItemHeader(Id + "-header")
            {
                Text = _ => "kleenestar.core:object.kind.documents.label"
            };

            var nodes = new HtmlList(header.Render(renderContext, visualTree));

            if (documents.Count == 0)
            {
                var empty = new ControlSidebarItemLink("document-empty")
                {
                    Text = _ => "kleenestar.core:object.kind.documents.none.label",
                    Active = _ => TypeActive.Disabled
                };

                nodes.Add(empty.Render(renderContext, visualTree));

                return nodes;
            }

            foreach (var entry in BuildEntries(documents))
            {
                nodes.Add(entry.Render(renderContext, visualTree));
            }

            return nodes;
        }

        /// <summary>
        /// Builds the root link entries from the fetched documents, grouping every
        /// document under its parent (when the parent is part of the fetched set) and
        /// promoting the rest to root entries. The traversal is cycle-safe.
        /// </summary>
        /// <param name="documents">The fetched workspace documents.</param>
        /// <returns>The root entries, each carrying its descendant subtree.</returns>
        private static IEnumerable<IControlSidebarItem> BuildEntries(IReadOnlyList<Model.Entities.Object> documents)
        {
            var ids = new HashSet<Guid>(documents.Select(x => x.Id));

            var childrenByParent = documents
                .Where(x => x.ParentId.HasValue && x.ParentId.Value != x.Id && ids.Contains(x.ParentId.Value))
                .GroupBy(x => x.ParentId.Value)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<Model.Entities.Object>)[.. g]);

            var visited = new HashSet<Guid>();

            foreach (var root in documents.Where(x => !x.ParentId.HasValue || !ids.Contains(x.ParentId.Value)))
            {
                if (visited.Add(root.Id))
                {
                    yield return BuildEntry(root, childrenByParent, visited, 0);
                }
            }
        }

        /// <summary>
        /// Fetches the workspace's document-kind objects, ordered by summary and capped
        /// at <see cref="MaxItems"/>. Returns an empty list when no workspace can be
        /// resolved from the request.
        /// </summary>
        /// <param name="renderContext">The render context carrying the workspace key parameter.</param>
        /// <returns>The capped, ordered set of documents. The list may be empty.</returns>
        private IReadOnlyList<Model.Entities.Object> GetDocuments(IRenderControlContext renderContext)
        {
            var keyParameter = renderContext?.Request?.GetParameter<WorkspaceKeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            if (workspace is null)
            {
                return [];
            }

            var query = new Query<Model.Entities.Object>()
                .WhereEquals(x => x.WorkspaceId, workspace.Id)
                .WhereEquals(x => x.Kind, Model.Entities.ObjectKind.Document)
                .OrderByAsc(x => x.Summary)
                .WithPaging(0, MaxItems);

            return [.. _objectManager.GetObjects(query)];
        }

        /// <summary>
        /// Builds a single link entry for the supplied document and, recursively, its
        /// children. Documents show their summary as the page title; root entries start
        /// expanded so the first tree level is visible without a click.
        /// </summary>
        /// <param name="document">The document to render as an entry.</param>
        /// <param name="childrenByParent">The parent-id to children lookup built from the fetched set.</param>
        /// <param name="visited">The set of already-rendered object ids, guarding against cycles.</param>
        /// <param name="depth">The current nesting depth; the root level is expanded by default.</param>
        /// <returns>The link entry representing <paramref name="document"/> and its subtree.</returns>
        private static IControlSidebarItem BuildEntry(Model.Entities.Object document, IReadOnlyDictionary<Guid, IReadOnlyList<Model.Entities.Object>> childrenByParent, ISet<Guid> visited, int depth)
        {
            var entry = new ControlSidebarItemLink("doc-" + document.Id.ToString("N"))
            {
                Text = _ => document.Summary,
                Tooltip = _ => document.Key,
                Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Object._objectkey_.Index>()
                    .BindParameters(new ObjectKeyParameter(document.Key)),
                Icon = _ => (IIcon)document.Icon ?? new IconFileLines(),
                Expanded = _ => depth == 0
            };

            if (childrenByParent.TryGetValue(document.Id, out var list))
            {
                foreach (var child in list)
                {
                    if (visited.Add(child.Id))
                    {
                        entry.Add(BuildEntry(child, childrenByParent, visited, depth + 1));
                    }
                }
            }

            return entry;
        }
    }
}
