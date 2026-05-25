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
    /// Sidebar group that renders the siblings of the current object (other objects in
    /// the same workspace and class) as a flat <see cref="ControlTree"/>.
    /// </summary>
    /// <remarks>
    /// Capped at <see cref="MaxItems"/> entries to keep the sidebar readable; further
    /// siblings are reachable via the workspace overview. The fragment self-suppresses
    /// when no sibling exists.
    /// </remarks>
    [Section<SectionSidebarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Order(31)]
    [Cache]
    public sealed class ObjectSidebarSiblingsFragment : FragmentControlSidebarItemDynamic
    {
        private const int MaxItems = 20;

        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Gets the tree control used to render the siblings.
        /// </summary>
        public ControlTree Tree { get; } = new("object-sidebar-siblings-tree")
        {
            Layout = _ => TypeLayoutTree.Default,
            DisableIndicator = _ => true
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObjectSidebarSiblingsFragment(IFragmentContext fragmentContext, IObjectManager objectManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;

            Icon = _ => new IconLayerGroup();
            Tooltip = _ => "kleenestar.core:object.sidebar.siblings.label";

            RenderControl = (renderContext, visualTree) =>
            {
                return Tree.Render(renderContext, visualTree, BuildNodes(renderContext));
            };
        }

        /// <summary>
        /// Renders the group, suppressing it when no siblings exist.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!GetSiblings(renderContext).Any())
            {
                return null;
            }

            return base.Render(renderContext, visualTree);
        }

        private IEnumerable<Model.Entities.Object> GetSiblings(IRenderControlContext renderContext)
        {
            var keyParameter = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            var current = _objectManager.GetObjectByKey(keyParameter?.Value);

            return current is null
                ? []
                : _objectManager.GetSiblings(current.Id).Take(MaxItems);
        }

        private IEnumerable<ControlTreeItem> BuildNodes(IRenderControlContext renderContext)
        {
            foreach (var sibling in GetSiblings(renderContext))
            {
                yield return new ControlTreeItem("obj-" + sibling.Id.ToString("N"))
                {
                    Text = _ => $"{sibling.Key}  {sibling.Summary}",
                    Tooltip = _ => sibling.Summary,
                    Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Object._objectkey_.Index>()
                        .BindParameters(new ObjectKeyParameter(sibling.Key)),
                    Icon = _ => (IIcon)sibling.Icon ?? new IconObject()
                };
            }
        }
    }
}
