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
    /// Entry in the actions menu of a document or a post that opens the object's files
    /// (<see cref="WWW.Issue._objectkey_.Attachments"/>).
    /// </summary>
    /// <remarks>
    /// The files of a prose object live on a page of their own rather than under its text, so
    /// they need a way in; this is the one in the overflow menu, matched by
    /// <see cref="ObjectToolbarAttachmentsFragment"/> in the toolbar. The number of files is
    /// part of the label, because whether there are any is the question the entry is read for.
    /// </remarks>
    [Section<SectionHeadlineMorePrimary>]
    [Scope<global::KleeneStar.Core.WWW.Document._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Blog._objectkey_.Index>]
    [Order(20)]
    [Cache]
    public sealed class ObjectItemAttachmentsMoreFragment : FragmentControlDropdownItemLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public ObjectItemAttachmentsMoreFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconPaperClip();
            Text = renderContext => ObjectSidePageLink.Label
            (
                I18N.Translate(renderContext, "kleenestar.core:object.attachment.card.header"),
                ObjectSidePageLink.CountAttachments(ObjectSidePageLink.ResolveObject(renderContext))
            );
            Uri = ObjectSidePageLink.ResolveAttachmentsUri;
        }

        /// <summary>
        /// Convert the fragment to HTML. Returns <c>null</c> when the request addresses no
        /// object, so the menu of a page that lost its key carries no entry that could only fail.
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
