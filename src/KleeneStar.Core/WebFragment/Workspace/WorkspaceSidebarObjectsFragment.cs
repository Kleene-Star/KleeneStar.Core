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

namespace KleeneStar.Core.WebFragment.Workspace
{
    /// <summary>
    /// Sidebar group that renders the objects of the current workspace as a
    /// <see cref="ControlTree"/> mirroring their parent/child containment. It replaces
    /// the workspace property cards that previously filled the page's property section.
    /// </summary>
    /// <remarks>
    /// The workspace's objects are fetched once, capped at <see cref="MaxItems"/> (the
    /// "top 200") to keep the sidebar responsive, and assembled into a tree in memory:
    /// every object whose parent is absent from the fetched set is promoted to a root
    /// node. A visited set guards the recursion against cycles persisted by older data.
    /// The fragment self-suppresses when the workspace holds no objects.
    /// </remarks>
    [Section<SectionSidebarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>]
    [Policy<WorkspaceViewPolicy>]
    [Order(10)]
    [Cache]
    public sealed class WorkspaceSidebarObjectsFragment : FragmentControlSidebarItemDynamic
    {
        /// <summary>
        /// The maximum number of objects fetched for the tree ("top 200").
        /// </summary>
        private const int MaxItems = 200;

        private readonly IObjectManager _objectManager;
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Gets the tree control used to render the workspace objects.
        /// </summary>
        public ControlTree Tree { get; } = new("workspace-sidebar-objects-tree")
        {
            Layout = _ => TypeLayoutTree.Default
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation.
        /// Cannot be null.
        /// </param>
        /// <param name="objectManager">
        /// The object manager used to retrieve the workspace objects. Cannot be null.
        /// </param>
        /// <param name="workspaceManager">
        /// The workspace manager used to resolve the workspace from the request. Cannot be null.
        /// </param>
        public WorkspaceSidebarObjectsFragment(IFragmentContext fragmentContext, IObjectManager objectManager, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _workspaceManager = workspaceManager;

            Icon = _ => new IconSitemap();
            Tooltip = _ => "kleenestar.core:workspace.sidebar.objects.label";

            RenderControl = (renderContext, visualTree) =>
            {
                return Tree.Render(renderContext, visualTree, BuildNodes(renderContext));
            };
        }

        /// <summary>
        /// Renders the group, suppressing it when the workspace holds no objects.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragment, or <c>null</c> when suppressed.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request) || GetObjects(renderContext).Count == 0)
            {
                return null;
            }

            return base.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Fetches the workspace's objects, ordered by key and capped at <see cref="MaxItems"/>.
        /// Returns an empty list when no workspace can be resolved from the request.
        /// </summary>
        /// <param name="renderContext">The render context carrying the workspace key parameter.</param>
        /// <returns>The capped, ordered set of workspace objects. The list may be empty.</returns>
        private IReadOnlyList<Model.Entities.Object> GetObjects(IRenderControlContext renderContext)
        {
            var keyParameter = renderContext?.Request?.GetParameter<WorkspaceKeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            if (workspace is null)
            {
                return [];
            }

            var query = new Query<Model.Entities.Object>()
                .WhereEquals(x => x.WorkspaceId, workspace.Id)
                .OrderByAsc(x => x.Key)
                .WithPaging(0, MaxItems);

            return [.. _objectManager.GetObjects(query)];
        }

        /// <summary>
        /// Builds the root tree nodes from the fetched objects, grouping every object under
        /// its parent (when the parent is part of the fetched set) and promoting the rest to
        /// roots. The traversal is cycle-safe.
        /// </summary>
        /// <param name="renderContext">The render context carrying the workspace key parameter.</param>
        /// <returns>The root tree items, each carrying its descendant sub-tree.</returns>
        private IEnumerable<ControlTreeItem> BuildNodes(IRenderControlContext renderContext)
        {
            var objects = GetObjects(renderContext);
            var ids = new HashSet<Guid>(objects.Select(x => x.Id));

            var childrenByParent = objects
                .Where(x => x.ParentId.HasValue && x.ParentId.Value != x.Id && ids.Contains(x.ParentId.Value))
                .GroupBy(x => x.ParentId.Value)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<Model.Entities.Object>)[.. g]);

            var visited = new HashSet<Guid>();
            var roots = new List<ControlTreeItem>();

            foreach (var root in objects.Where(x => !x.ParentId.HasValue || !ids.Contains(x.ParentId.Value)))
            {
                if (visited.Add(root.Id))
                {
                    roots.Add(BuildNode(root, childrenByParent, visited, 0));
                }
            }

            return roots;
        }

        /// <summary>
        /// Builds a single tree node for the supplied object and, recursively, its children.
        /// </summary>
        /// <param name="obj">The object to render as a node.</param>
        /// <param name="childrenByParent">The parent-id to children lookup built from the fetched set.</param>
        /// <param name="visited">The set of already-rendered object ids, guarding against cycles.</param>
        /// <param name="depth">The current nesting depth; the root level is expanded by default.</param>
        /// <returns>The tree item representing <paramref name="obj"/> and its sub-tree.</returns>
        private ControlTreeItem BuildNode(Model.Entities.Object obj, IReadOnlyDictionary<Guid, IReadOnlyList<Model.Entities.Object>> childrenByParent, ISet<Guid> visited, int depth)
        {
            var children = new List<ControlTreeItem>();

            if (childrenByParent.TryGetValue(obj.Id, out var list))
            {
                foreach (var child in list)
                {
                    if (visited.Add(child.Id))
                    {
                        children.Add(BuildNode(child, childrenByParent, visited, depth + 1));
                    }
                }
            }

            return new ControlTreeItem("obj-" + obj.Id.ToString("N"), [.. children])
            {
                Text = _ => $"{obj.Key}  {obj.Summary}",
                Tooltip = _ => obj.Summary,
                Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Object._objectkey_.Index>()
                    .BindParameters(new ObjectKeyParameter(obj.Key)),
                Icon = _ => (IIcon)obj.Icon ?? new IconObject(),
                Expand = _ => depth == 0
            };
        }
    }
}
