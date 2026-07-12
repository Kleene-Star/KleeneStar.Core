using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Sidebar group that renders the descendants of the current object as a
    /// <see cref="ControlTree"/>.
    /// </summary>
    /// <remarks>
    /// The tree is walked recursively up to <see cref="MaxDepth"/> levels deep so an
    /// object owning sub-objects shows its full nesting without overwhelming the
    /// sidebar. Each node links to the corresponding object detail page. The fragment
    /// self-suppresses when the current object has no children.
    /// </remarks>
    [Section<SectionSidebarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Order(30)]
    [Cache]
    public sealed class ObjectSidebarChildrenFragment : FragmentControlSidebarItemDynamic
    {
        private const int MaxDepth = 3;

        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Gets the tree control used to render the descendants.
        /// </summary>
        public ControlTree Tree { get; } = new("object-sidebar-children-tree")
        {
            Layout = _ => TypeLayoutTree.Default
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObjectSidebarChildrenFragment(IFragmentContext fragmentContext, IObjectManager objectManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;

            Icon = _ => new IconSitemap();
            Tooltip = _ => "kleenestar.core:object.sidebar.children.label";

            RenderControl = (renderContext, visualTree) =>
            {
                var current = ResolveCurrent(renderContext);
                var nodes = current is null
                    ? []
                    : BuildNodes(current.Id, 0);

                return Tree.Render(renderContext, visualTree, nodes);
            };
        }

        /// <summary>
        /// Renders the group, suppressing it when the current object has no children.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var current = ResolveCurrent(renderContext);

            if (current is null || !_objectManager.GetChildren(current.Id).Any())
            {
                return null;
            }

            return base.Render(renderContext, visualTree);
        }

        private Model.Entities.Object ResolveCurrent(IRenderControlContext renderContext)
        {
            var keyParameter = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            return _objectManager.GetObjectByKey(keyParameter?.Value);
        }

        private IEnumerable<ControlTreeItem> BuildNodes(System.Guid parentId, int depth)
        {
            var children = _objectManager.GetChildren(parentId).ToList();

            foreach (var child in children)
            {
                var grandchildren = depth + 1 < MaxDepth
                    ? BuildNodes(child.Id, depth + 1).ToArray()
                    : [];

                var node = new ControlTreeItem("obj-" + child.Id.ToString("N"), grandchildren)
                {
                    Text = _ => $"{child.Key}  {child.Summary}",
                    Tooltip = _ => child.Summary,
                    Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Object._objectkey_.Index>()
                        .BindParameters(new ObjectKeyParameter(child.Key)),
                    Icon = _ => (IIcon)child.Icon ?? new IconObject(),
                    Expand = _ => depth == 0
                };

                yield return node;
            }
        }
    }
}
