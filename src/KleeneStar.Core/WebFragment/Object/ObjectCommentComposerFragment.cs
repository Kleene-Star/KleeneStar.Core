using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Object-scoped fragment that renders the new-comment composer at the bottom of
    /// the object detail page.
    /// </summary>
    /// <remarks>
    /// The composer posts via <c>POST /api/1/comments/{objectkey}</c> to the same REST
    /// endpoint the <see cref="ObjectCommentFragment"/> reads from
    /// (<see cref="WWW.Api._1_.Comments._objectkey_.Index"/>). The
    /// <see cref="ControlRestCommentComposer.RestUri"/> is bound to the current
    /// request's <see cref="ObjectKeyParameter"/>; the
    /// <see cref="ControlRestCommentComposer.Placeholder"/> is sourced from the
    /// <c>kleenestar.core:comment.composer.placeholder</c> translation key.
    /// </remarks>
    [Section<SectionContentSecondary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Order(99)]
    [Cache]
    public sealed class ObjectCommentComposerFragment : FragmentControlPanel
    {
        /// <summary>
        /// Gets the REST-backed comment composer control.
        /// </summary>
        public ControlRestCommentComposer Composer { get; } = new("object-comment-composer")
        {
            Placeholder = renderContext => I18N.Translate(renderContext, "kleenestar.core:comment.composer.placeholder"),
            RestUri = renderContext => CoreHub
                .GetUri<global::KleeneStar.Core.WWW.Api._1_.Comments._objectkey_.Index>()
                .BindParameters(renderContext.Request),
            CurrentUser = _ => "Admin User"
        };

        /// <summary>
        /// Initializes a new instance of the class and attaches the composer control to
        /// the fragment.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public ObjectCommentComposerFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Composer);
        }

        /// <summary>
        /// Renders the fragment.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>The HTML node, or <c>null</c> when the fragment's render conditions
        /// exclude it.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            return base.Render(renderContext, visualTree);
        }
    }
}
