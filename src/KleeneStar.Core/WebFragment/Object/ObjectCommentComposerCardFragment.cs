using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebApp.WebData;
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
    /// Object-scoped content card that renders the new-comment composer for the object
    /// currently displayed on <see cref="WWW.Issue._objectkey_.Index"/>. The card is
    /// pinned to the end of the page via the maximum <see cref="OrderAttribute"/> value
    /// so it always renders below every other content fragment.
    /// </summary>
    /// <remarks>
    /// The composer posts via <c>POST /api/1/comments/{objectkey}</c> to the same REST
    /// endpoint the <see cref="ObjectCommentCardFragment"/> reads from
    /// (<see cref="WWW.Api._1_.Comments._objectkey_.Index"/>). The
    /// <see cref="ControlDataCommentComposer.RestUri"/> is bound to the current
    /// request's <see cref="ObjectKeyParameter"/>; the
    /// <see cref="ControlDataCommentComposer.Placeholder"/> is sourced from the
    /// <c>kleenestar.core:comment.composer.placeholder</c> translation key.
    /// </remarks>
    [Section<SectionContentSecondary>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Asset._objectkey_.Index>]
    [Order(int.MaxValue)]
    [Cache]
    public sealed class ObjectCommentComposerCardFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Gets the REST-backed comment composer control.
        /// </summary>
        public ControlDataCommentComposer Composer { get; } = new("object-comment-composer")
        {
            Placeholder = renderContext => I18N.Translate(renderContext, "kleenestar.core:comment.composer.placeholder"),
            ServiceFactory = renderContext => DataServiceDescriptor.QueryData
            (
                CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Comments._objectkey_.Index>()
                    .BindParameters(renderContext.Request)
                    .ToString()
            ),
            CurrentUser = _ => "Admin User"
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to resolve the current
        /// object from the URL-bound object key.</param>
        public ObjectCommentComposerCardFragment(IFragmentContext fragmentContext, IObjectManager objectManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
        }

        /// <summary>
        /// Renders the composer card for the current object. Returns <c>null</c> when the
        /// fragment's render conditions exclude it or when no object can be resolved from
        /// the request.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>The HTML node, or <c>null</c>.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var keyParameter = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            var @object = _objectManager.GetObjectByKey(keyParameter?.Value);

            if (@object is null)
            {
                return null;
            }

            var card = new ControlPanelCard("object-comment-composer-card")
            {
                Header = _ => "kleenestar.core:object.comment.composer.card.header",
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two)
            };

            card.Add(Composer);

            return card.Render(renderContext, visualTree);
        }
    }
}
