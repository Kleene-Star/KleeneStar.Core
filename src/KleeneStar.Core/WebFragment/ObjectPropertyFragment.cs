using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a control fragment that provides a button link for adding a new object within the workspace.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Cache]
    public sealed class ObjectPropertyFragment : FragmentControlAttribute
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public ObjectPropertyFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Key = _ => "Created";
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var objectKey = renderContext.Request.GetParameter<ObjectKeyParameter>();
            var @object = CoreHub.ObjectManager.GetObjectByKey(objectKey);

            return base.Render(renderContext, visualTree);
        }
    }
}
