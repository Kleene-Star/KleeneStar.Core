using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System.Globalization;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
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
    /// Object-scoped property card that groups the lifecycle attributes of the object
    /// (creation timestamp, last update timestamp, and lifecycle state) inside a single
    /// <see cref="ControlPanelCard"/>.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Order(9)]
    [Cache]
    public sealed class ObjectPropertyLifecycleCardFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObjectPropertyLifecycleCardFragment(IFragmentContext fragmentContext, IObjectManager objectManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
        }

        /// <summary>
        /// Renders the lifecycle card. Returns <c>null</c> when no object can be
        /// resolved from the request.
        /// </summary>
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

            var card = new ControlPanelCard("object-property-lifecycle-card")
            {
                Header = _ => "kleenestar.core:object.property.lifecycle.header",
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two)
            };

            card.Add(new ControlAttribute("object-property-created")
            {
                Icon = _ => new IconCalendarPlus(),
                Key = _ => "kleenestar.core:object.created.label",
                Value = _ => @object.Created.ToString("g", CultureInfo.InvariantCulture)
            });

            card.Add(new ControlAttribute("object-property-updated")
            {
                Icon = _ => new IconClockRotateLeft(),
                Key = _ => "kleenestar.core:object.updated.label",
                Value = _ => @object.Updated.ToString("g", CultureInfo.InvariantCulture)
            });

            card.Add(new ControlAttribute("object-property-state")
            {
                Icon = _ => new IconTrafficLight(),
                Key = _ => "kleenestar.core:object.state.label",
                Value = ctx => I18N.Translate(ctx, @object.State.Text())
            });

            return card.Render(renderContext, visualTree);
        }
    }
}
