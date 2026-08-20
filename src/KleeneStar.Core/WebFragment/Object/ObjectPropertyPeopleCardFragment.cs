using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Object-scoped property card that groups the people related to the current object on
    /// <see cref="WWW.Issue._objectkey_.Index"/>: its creator, its current assignee (with a
    /// one-click "assign to me" / "unassign" link), and the watcher avatar row.
    /// </summary>
    /// <remarks>
    /// Creator and assignee are resolved from the object's <see cref="Model.Entities.Object.CreatorId"/>
    /// and <see cref="Model.Entities.Object.AssigneeId"/> through the
    /// <see cref="IIdentityManager"/>. The assign / unassign link targets the
    /// <see cref="WWW.Api._1_.Assignee._objectkey_.Index"/> REST endpoint, which flips the
    /// assignment for the current identity and redirects back to this page. The watcher row
    /// keeps the previous behaviour: a <see cref="ControlDataWatcher"/> wired to the
    /// <see cref="WWW.Api._1_.Watchers._objectkey_.Index"/> (list / add / remove) and
    /// <see cref="WWW.Api._1_.WatcherUsers._objectkey_.Index"/> (user search) endpoints.
    /// </remarks>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Asset._objectkey_.Index>]
    [Order(5)]
    [Cache]
    public sealed class ObjectPropertyPeopleCardFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;
        private readonly IIdentityManager _identityManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to resolve the current
        /// object from the URL-bound object key.</param>
        /// <param name="identityManager">The identity manager used to resolve the creator
        /// and assignee display names.</param>
        public ObjectPropertyPeopleCardFragment(IFragmentContext fragmentContext, IObjectManager objectManager, IIdentityManager identityManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _identityManager = identityManager;
        }

        /// <summary>
        /// Renders the people card for the current object. Returns <c>null</c> when the
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

            var section = new ControlSection("object-property-people-section")
            {
                Header = _ => "kleenestar.core:object.property.people.header",
                HeaderIcon = _ => new IconUsers(TypeIconTheme.Light),
                Layout = _ => TypeLayoutSection.Rule
            };

            var creatorName = @object.CreatorId.HasValue
                ? _identityManager.GetIdentity(@object.CreatorId.Value)?.Name
                : null;

            section.Add(new ControlAttribute("object-property-creator")
            {
                Icon = _ => new IconUserPen(TypeIconTheme.Light),
                Key = _ => "kleenestar.core:object.creator.label",
                Value = _ => string.IsNullOrWhiteSpace(creatorName) ? "—" : creatorName
            });

            var assigneeName = @object.AssigneeId.HasValue
                ? _identityManager.GetIdentity(@object.AssigneeId.Value)?.Name
                : null;

            section.Add(new ControlAttribute("object-property-assignee")
            {
                Icon = _ => new IconUserCheck(TypeIconTheme.Light),
                Key = _ => "kleenestar.core:object.assignee.label",
                Value = ctx => string.IsNullOrWhiteSpace(assigneeName)
                    ? I18N.Translate(ctx, "kleenestar.core:object.assignee.unassigned.label")
                    : assigneeName
            });

            var currentUserId = CoreHub.SessionManager.GetCurrentIdentityId(renderContext?.Request);
            var assignedToMe = @object.AssigneeId.HasValue && @object.AssigneeId.Value == currentUserId;

            section.Add(new ControlLink("object-property-assignee-action")
            {
                Text = _ => assignedToMe
                    ? "kleenestar.core:object.assignee.unassign.label"
                    : "kleenestar.core:object.assignee.assignme.label",
                Icon = _ => assignedToMe ? new IconUserXmark(TypeIconTheme.Light) : new IconUserPlus(TypeIconTheme.Light),
                Uri = ctx =>
                {
                    var uri = CoreHub
                        .GetUri<global::KleeneStar.Core.WWW.Api._1_.Assignee._objectkey_.Index>()
                        ?.BindParameters(ctx.Request);

                    return assignedToMe ? uri?.Add(new UriQuery("clear", "1")) : uri;
                }
            });

            section.Add(new ControlText("object-property-watcher-label")
            {
                Text = _ => "kleenestar.core:object.property.watcher.header",
                Format = _ => TypeFormatText.Small
            });

            section.Add(new ControlDataWatcher("object-property-watcher")
            {
                MaxVisible = _ => 6
            }
                .DataService<global::KleeneStar.Core.WWW.Api._1_.Watchers._objectkey_.Index>()
                .UsersService<global::KleeneStar.Core.WWW.Api._1_.WatcherUsers._objectkey_.Index>());

            return section.Render(renderContext, visualTree);
        }
    }
}
