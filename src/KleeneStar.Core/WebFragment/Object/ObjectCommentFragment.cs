using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Object-scoped fragment that renders the existing comment thread for the object
    /// currently displayed on <see cref="WWW.Object._objectkey_.Index"/>.
    /// </summary>
    /// <remarks>
    /// The fragment hosts a single <see cref="ControlRestComment"/> whose
    /// <see cref="ControlRestComment.RestUri"/> points at the class-scoped
    /// <see cref="WWW.Api._1_.Comments._objectkey_.Index"/> REST endpoint with the
    /// <see cref="ObjectKeyParameter"/> bound from the request. The control
    /// asynchronously fetches the comments via <c>GET</c>, supports edit / delete /
    /// reply via the matching REST verbs, and renders soft-deleted comments as
    /// placeholders.
    /// </remarks>
    [Section<SectionContentSecondary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Cache]
    public sealed class ObjectCommentFragment : FragmentControlPanel
    {
        /// <summary>
        /// Gets the REST-backed comment list control.
        /// </summary>
        public ControlRestComment Comments { get; } = new("object-comments")
        {
            RestUri = renderContext => CoreHub
                .GetUri<global::KleeneStar.Core.WWW.Api._1_.Comments._objectkey_.Index>()
                .BindParameters(renderContext.Request),
            CurrentUser = _ => "Admin User"
        };

        /// <summary>
        /// Initializes a new instance of the class and attaches the comment list control
        /// to the fragment.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public ObjectCommentFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Comments);
        }

        /// <summary>
        /// Renders the fragment.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>The HTML node, or <c>null</c> when the fragment's render conditions
        /// (e.g. permissions / scope filters) exclude it.</returns>
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
