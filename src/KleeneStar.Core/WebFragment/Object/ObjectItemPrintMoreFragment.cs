using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Represents a dropdown item in the object headline 'more' overflow menu that opens the
    /// print dialog for the current object.
    /// </summary>
    [Section<SectionHeadlineMorePrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Cache]
    public sealed class ObjectItemPrintMoreFragment : FragmentControlDropdownItemLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation.
        /// Cannot be null.
        /// </param>
        public ObjectItemPrintMoreFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "kleenestar.core:object.print.label";
            Icon = _ => new IconPrint();
            PrimaryAction = renderContext => new ActionModal
            (
                "modal-form",
                CoreHub.GetUri<global::KleeneStar.Core.WWW.Object._objectkey_.Print>()
                    .BindParameters(renderContext.Request),
                TypeModalSize.Large
            );
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>
        /// An HTML node representing the rendered fragments. Can be null if no nodes are present.
        /// </returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
