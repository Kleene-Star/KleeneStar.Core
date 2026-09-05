using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.Internationalization;
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
    /// The composer of the communication history: the new-comment form for the object
    /// currently displayed on <see cref="WWW.Issue._objectkey_.Index"/>. The section is
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
    /// <para>
    /// The composer shows its <b>WYSIWYG form right away</b> rather than the framework's one-line
    /// trigger, so unfolding this section is the only gesture between reading an issue and writing
    /// on it. The control offers no option for that, which is what
    /// <see cref="CommentComposerExpandScript"/> exists for - it also carries the remedy that
    /// would make it unnecessary.
    /// </para>
    /// </remarks>
    [Section<SectionContentSecondary>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Asset._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Document._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Blog._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Preview>]
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
            // the composer would otherwise mount on its one-line trigger and only build the
            // editor once that is clicked; the class asks the companion script to open it
            Classes = [CommentComposerExpandScript.OptInClass],
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
        /// Renders the composer for the current object. Returns <c>null</c> when the
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

            var script = CommentComposerExpandScript.Value;

            if (!string.IsNullOrEmpty(script))
            {
                visualTree.AddHeaderScript(script);
            }

            var section = new ControlSection("object-comment-composer-section")
            {
                Header = _ => "kleenestar.core:object.comment.composer.card.header",
                HeaderIcon = _ => new IconPenToSquare(),
                Expanded = _ => false,
                Layout = _ => TypeLayoutSection.Rule
            };

            section.Add(Composer);

            return section.Render(renderContext, visualTree);
        }
    }
}
