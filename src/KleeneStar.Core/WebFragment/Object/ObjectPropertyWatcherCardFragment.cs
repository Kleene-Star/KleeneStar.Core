using KleeneStar.Core.WebManager;
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
    /// Object-scoped property card that renders the watcher avatar row of the current
    /// object on <see cref="WWW.Object._objectkey_.Index"/>.
    /// </summary>
    /// <remarks>
    /// The card hosts a single <see cref="ControlRestObserver"/>, wired to the
    /// <see cref="WWW.Api._1_.Watchers._objectkey_.Index"/> REST endpoint for the
    /// list / add / remove operations and the
    /// <see cref="WWW.Api._1_.WatcherUsers._objectkey_.Index"/> endpoint for the user
    /// search in the "+" dropdown. Both URIs are bound to the current request's
    /// <see cref="ObjectKeyParameter"/> so the URL <c>{objectkey}</c> segment resolves
    /// to the object the user is viewing.
    /// </remarks>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Order(5)]
    [Cache]
    public sealed class ObjectPropertyWatcherCardFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to confirm that the
        /// request resolves to a real object before rendering the card.</param>
        public ObjectPropertyWatcherCardFragment(IFragmentContext fragmentContext, IObjectManager objectManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
        }

        /// <summary>
        /// Renders the watcher card for the current object. Returns <c>null</c> when
        /// the fragment's render conditions exclude it or when no object can be
        /// resolved from the request.
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

            var card = new ControlPanelCard("object-property-watcher-card")
            {
                Header = _ => "kleenestar.core:object.property.watcher.header"
            };

            card.Add(new ControlRestObserver("object-property-watcher")
            {
                RestUri = ctx => CoreHub
                    .GetUri<global::KleeneStar.Core.WWW.Api._1_.Watchers._objectkey_.Index>()
                    .BindParameters(ctx.Request),
                UsersUri = ctx => CoreHub
                    .GetUri<global::KleeneStar.Core.WWW.Api._1_.WatcherUsers._objectkey_.Index>()
                    .BindParameters(ctx.Request),
                MaxVisible = _ => 6
            });

            return card.Render(renderContext, visualTree);
        }
    }
}
