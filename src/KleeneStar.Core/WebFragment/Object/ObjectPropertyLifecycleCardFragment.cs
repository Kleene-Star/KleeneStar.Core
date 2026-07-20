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
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>]
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

            card.Add(new ControlPanelFlex
            (
                "object-property-state",
                new ControlIcon
                {
                    Icon = _ => new IconTrafficLight(),
                    Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.One, PropertySpacing.Space.None, PropertySpacing.Space.None)
                },
                new ControlText
                {
                    Text = ctx => I18N.Translate(ctx, "kleenestar.core:object.state.label") + ":",
                    Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.One, PropertySpacing.Space.None, PropertySpacing.Space.None)
                },
                new ControlBadge("object-property-state-badge")
                {
                    Value = ctx => I18N.Translate(ctx, @object.State.Text()),
                    Pill = _ => TypePillBadge.Pill,
                    BackgroundColor = _ => new PropertyColorBackgroundBadge(MapStateBadgeColor(@object.State))
                }
            )
            {
                Layout = _ => TypeLayoutFlex.Default,
                Align = _ => TypeAlignFlex.Center,
                Justify = _ => TypeJustifiedFlex.Start
            });

            return card.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Maps a workspace lifecycle state to the badge background color used to render it
        /// in the lifecycle card: green for active, grey for archived.
        /// </summary>
        /// <param name="state">The lifecycle state.</param>
        /// <returns>The badge background color.</returns>
        private static TypeColorBackgroundBadge MapStateBadgeColor(WorkspaceState state)
        {
            return state switch
            {
                WorkspaceState.Active => TypeColorBackgroundBadge.Success,
                WorkspaceState.Archived => TypeColorBackgroundBadge.Secondary,
                _ => TypeColorBackgroundBadge.Default
            };
        }
    }
}
