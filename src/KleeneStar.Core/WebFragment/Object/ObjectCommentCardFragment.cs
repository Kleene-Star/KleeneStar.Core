using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using System.Globalization;
using System.Linq;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebApp.WebData;
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
    /// The communication-history section of the object view: the comment thread for the object
    /// currently displayed on <see cref="WWW.Issue._objectkey_.Index"/>.
    /// </summary>
    /// <remarks>
    /// The section hosts a single <see cref="ControlDataComment"/> whose
    /// <see cref="ControlDataComment.RestUri"/> points at the class-scoped
    /// <see cref="WWW.Api._1_.Comments._objectkey_.Index"/> REST endpoint with the
    /// <see cref="ObjectKeyParameter"/> bound from the request. The control
    /// asynchronously fetches the comments via <c>GET</c>, supports edit / delete /
    /// reply via the matching REST verbs, and renders soft-deleted comments as
    /// placeholders. The matching new-comment composer is rendered separately, always
    /// at the end of the page, by <see cref="ObjectCommentComposerCardFragment"/>.
    /// </remarks>
    [Section<SectionContentSecondary>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Asset._objectkey_.Index>]
    [Cache]
    public sealed class ObjectCommentCardFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;
        private readonly ICommentManager _commentManager;

        /// <summary>
        /// Gets the REST-backed comment list control.
        /// </summary>
        public ControlDataComment Comments { get; } = new("object-comments")
        {
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
        /// <param name="commentManager">The comment manager, read for the count the header
        /// reports.</param>
        public ObjectCommentCardFragment(IFragmentContext fragmentContext, IObjectManager objectManager, ICommentManager commentManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _commentManager = commentManager;
        }

        /// <summary>
        /// Renders the comment section for the current object. Returns <c>null</c> when the
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

            // the thread itself arrives from the rest endpoint, but the count is cheap to read
            // here - and it is what makes a folded conversation still say whether there is one
            var count = _commentManager.GetComments(@object.Id).Count();

            var section = new ControlSection("object-comments-section")
            {
                Header = _ => "kleenestar.core:object.comments.card.header",
                HeaderIcon = _ => new IconComments(TypeIconTheme.Light),
                Layout = _ => TypeLayoutSection.Rule,
                Badge = count > 0 ? _ => count.ToString(CultureInfo.InvariantCulture) : null
            };

            section.Add(Comments);

            return section.Render(renderContext, visualTree);
        }
    }
}
