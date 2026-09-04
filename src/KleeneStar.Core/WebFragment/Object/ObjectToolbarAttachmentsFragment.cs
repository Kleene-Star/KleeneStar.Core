using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Toolbar button over the text of a document or a post that opens the object's files
    /// (<see cref="WWW.Issue._objectkey_.Attachments"/>).
    /// </summary>
    /// <remarks>
    /// The same destination as <see cref="ObjectItemAttachmentsMoreFragment"/>, offered twice on
    /// purpose: the overflow menu is where everything an object can do is findable, the toolbar
    /// is where the two things a reader of a document actually reaches for stay in sight.
    /// </remarks>
    [Section<SectionToolbarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Document._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Blog._objectkey_.Index>]
    [Order(0)]
    [Cache]
    public sealed class ObjectToolbarAttachmentsFragment : FragmentControlToolbarItemButton
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public ObjectToolbarAttachmentsFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconPaperClip();
            Text = renderContext => ObjectSidePageLink.Label
            (
                I18N.Translate(renderContext, "kleenestar.core:object.attachment.card.header"),
                ObjectSidePageLink.CountAttachments(ObjectSidePageLink.ResolveObject(renderContext))
            );
            Tooltip = _ => "kleenestar.core:object.attachment.card.header";
            Uri = ObjectSidePageLink.ResolveAttachmentsUri;
        }

        /// <summary>
        /// Convert the fragment to HTML. Returns <c>null</c> when the request addresses no
        /// object.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragment, or <c>null</c>.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return ObjectSidePageLink.ResolveAttachmentsUri(renderContext) is null
                ? null
                : base.Render(renderContext, visualTree);
        }
    }
}
