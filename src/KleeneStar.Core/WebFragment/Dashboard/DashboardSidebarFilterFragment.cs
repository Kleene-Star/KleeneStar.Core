using System.Collections.Generic;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Dashboard
{
    /// <summary>
    /// Displays the list of all available dashboards in the sidebar of the home page, 
    /// allowing the user to switch between dashboards.
    /// </summary>
    [Section<SectionSidebarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Index>]
    [Cache]
    public sealed class DashboardSidebarFilterFragment : FragmentControlSidebarItemLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its
        /// operation. Cannot be null.
        /// </param>
        public DashboardSidebarFilterFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragment, or <c>null</c> if the
        /// fragment conditions are not met.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var list = new List<IHtmlNode>();

            foreach (var dashboard in CoreHub.DashboardManager.GetDashboards(new Query<Model.Entities.Dashboard>()))
            {
                list.Add(new ControlSidebarItemLink($"dashboard-{dashboard.Id}")
                {
                    Text = _ => dashboard.Name,
                    PrimaryAction = _ => new ActionFilter()
                    {
                        Exclusive = true,
                        Group = "dashboard"
                    }
                }
                    .Render(renderContext, visualTree));
            }

            return new HtmlList(list);
        }
    }
}
