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

namespace KleeneStar.Core.WebFragment.Class
{
    /// <summary>
    /// Class-scoped property card that groups the lifecycle attributes of the class
    /// (creation timestamp, last update timestamp, and lifecycle state) inside a single
    /// <see cref="ControlPanelCard"/>.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Class._classid_.Index>]
    [Order(10)]
    [Cache]
    public sealed class ClassPropertyLifecycleCardFragment : FragmentControlPanel
    {
        private readonly IClassManager _classManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ClassPropertyLifecycleCardFragment(IFragmentContext fragmentContext, IClassManager classManager)
            : base(fragmentContext)
        {
            _classManager = classManager;
        }

        /// <summary>
        /// Renders the lifecycle card. Returns <c>null</c> when no class can be resolved
        /// from the request.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var classId = renderContext?.Request?.GetParameter<ClassIdParameter>();
            var @class = _classManager.GetClass(classId);

            if (@class is null)
            {
                return null;
            }

            var card = new ControlPanelCard("class-property-lifecycle-card")
            {
                Header = _ => "kleenestar.core:class.property.lifecycle.header"
            };

            card.Add(new ControlAttribute("class-property-created")
            {
                Icon = _ => new IconCalendarPlus(),
                Key = _ => "kleenestar.core:class.created.label",
                Value = _ => @class.Created.ToString("g", CultureInfo.InvariantCulture)
            });

            card.Add(new ControlAttribute("class-property-updated")
            {
                Icon = _ => new IconClockRotateLeft(),
                Key = _ => "kleenestar.core:class.updated.label",
                Value = _ => @class.Updated.ToString("g", CultureInfo.InvariantCulture)
            });

            card.Add(new ControlAttribute("class-property-state")
            {
                Icon = _ => new IconTrafficLight(),
                Key = _ => "kleenestar.core:class.state.label",
                Value = ctx => I18N.Translate(ctx, @class.State.Text())
            });

            return card.Render(renderContext, visualTree);
        }
    }
}
