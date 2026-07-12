using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Sidebar link that points at the parent of the current object, when one is set.
    /// </summary>
    /// <remarks>
    /// The link self-suppresses (returns <c>null</c>) when the object has no parent,
    /// so the sidebar row only shows for objects that actually live inside a hierarchy.
    /// </remarks>
    [Section<SectionSidebarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Order(20)]
    [Cache]
    public sealed class ObjectSidebarParentLinkFragment : FragmentControlSidebarItemLink
    {
        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to resolve the parent.</param>
        public ObjectSidebarParentLinkFragment(IFragmentContext fragmentContext, IObjectManager objectManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;

            Icon = _ => new IconArrowUp();
            Text = renderContext => GetParent(renderContext)?.Summary
                ?? "kleenestar.core:object.sidebar.parent.label";
            Uri = renderContext => GetUri(renderContext);
        }

        /// <summary>
        /// Renders the link, but only when the current object has a parent.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (GetParent(renderContext) is null)
            {
                return null;
            }

            return base.Render(renderContext, visualTree);
        }

        private Model.Entities.Object GetParent(IRenderControlContext renderContext)
        {
            var keyParameter = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            var current = _objectManager.GetObjectByKey(keyParameter?.Value);

            if (current?.ParentId is null)
            {
                return null;
            }

            return _objectManager.GetObject(current.ParentId.Value);
        }

        private IUri GetUri(IRenderControlContext renderContext)
        {
            var parent = GetParent(renderContext);

            if (parent is null)
            {
                return null;
            }

            return CoreHub.GetUri<global::KleeneStar.Core.WWW.Object._objectkey_.Index>()
                .BindParameters(new ObjectKeyParameter(parent.Key));
        }
    }
}
