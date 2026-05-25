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
    /// Sidebar link that takes the user to the class detail page of the current object.
    /// </summary>
    [Section<SectionSidebarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Order(11)]
    [Cache]
    public sealed class ObjectSidebarClassLinkFragment : FragmentControlSidebarItemLink
    {
        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to resolve the class id
        /// from the current object.</param>
        public ObjectSidebarClassLinkFragment(IFragmentContext fragmentContext, IObjectManager objectManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;

            Icon = _ => new IconClass();
            Text = _ => "kleenestar.core:object.sidebar.class.label";
            Uri = renderContext => GetUri(renderContext);
        }

        /// <summary>
        /// Renders the link.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }

        private IUri GetUri(IRenderControlContext renderContext)
        {
            var keyParameter = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            var @object = _objectManager.GetObjectByKey(keyParameter?.Value);

            if (@object is null)
            {
                return null;
            }

            return CoreHub.GetUri<global::KleeneStar.Core.WWW.Class._classid_.Index>()
                .BindParameters(new ClassIdParameter(@object.ClassId));
        }
    }
}
