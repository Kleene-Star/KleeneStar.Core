using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Group
{
    /// <summary>
    /// Represents a delete form fragment for a group.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Settings.Group._groupid_.Delete>]
    [Cache]
    public sealed class GroupDeleteFormFragment : FragmentControlRestFormDelete
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GroupDeleteFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Mode = _ => TypeRestFormMode.Delete;
            Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Groups.Index>();
        }

        /// <summary>
        /// Renders the control as HTML.
        /// </summary>
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            var param = renderContext.Request.GetParameter<GroupIdParameter>();

            return base.Render(renderContext, visualTree);
        }
    }
}
