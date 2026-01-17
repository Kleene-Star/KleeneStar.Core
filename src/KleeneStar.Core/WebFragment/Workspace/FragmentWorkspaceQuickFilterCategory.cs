using KleeneStar.Core.WebParameter.Workspace;
using System.Collections.Generic;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workspace
{
    /// <summary>
    /// Represents a sidebar link fragment for quick filtering workspace categories within the workspace manager
    /// interface.
    /// </summary>
    [Section<SectionSidebarPreferences>]
    [Scope<WWW.Workspace.Index>]
    [Cache]
    public sealed class WorkspaceQuickFilertCategoryFragment : FragmentControlSidebarItemLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public WorkspaceQuickFilertCategoryFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = "kleenestar.core:workspace.quickfilter.category.label";
            Uri = CoreHub.GetUri<WWW.Workspace.Index>();
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var categoryParameter = renderContext.Request.GetParameter<CategoryParameter>();
            var list = new List<IHtmlNode>();

            foreach (var category in CoreHub.WorkspaceManager.WorkspaceCategories)
            {
                var label = category.Trim().ToLower();
                var uri = CoreHub.GetUri<WWW.Workspace.Index>();

                list.Add(new ControlSidebarItemLink()
                {
                    Text = category,
                    Active = label.Equals(categoryParameter?.Value, System.StringComparison.InvariantCultureIgnoreCase)
                        ? TypeActive.Active
                        : TypeActive.None,
                    Uri = uri.Add(new UriQuery("category", category))
                }
                    .Render(renderContext, visualTree));
            }

            return new HtmlList(list);
        }
    }
}
