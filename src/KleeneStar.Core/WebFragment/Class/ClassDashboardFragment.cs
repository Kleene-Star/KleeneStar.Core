using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Class
{
    /// <summary>
    /// Renders the class dashboard on the content area of the class index page.
    /// Displays key metrics (forms, fields, priorities, statuses, and workflows)
    /// as <c>RestApiDashboardWidgetBigNumber</c> widgets, each linking to the
    /// corresponding configuration page.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Class._classid_.Index>]
    [Cache]
    public sealed class ClassDashboardFragment : FragmentControlDataDashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its
        /// operation. Cannot be null.
        /// </param>
        public ClassDashboardFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            ServiceFactory = renderContext => DataServiceDescriptor.QueryData(GetRestUri(renderContext).ToString());
        }

        /// <summary>
        /// Renders the control as an HTML node.
        /// </summary>
        /// <param name="renderContext">
        /// The context in which the control is rendered.
        /// </param>
        /// <param name="visualTree">
        /// The visual tree representing the control's structure.
        /// </param>
        /// <returns>
        /// An HTML node representing the rendered control.
        /// </returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }



            return base.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Retrieves a REST API URI for class statistics based on the current render context.
        /// </summary>
        /// <param name="renderContext">
        /// The render context containing the request parameters used to determine the class identifier.
        /// </param>
        /// <returns>
        /// An <see cref="IUri"/> representing the REST API endpoint for the specified class statistics, 
        /// or <see langword="null"/> if the class identifier is not available.
        /// </returns>
        private static IUri GetRestUri(IRenderControlContext renderContext)
        {
            var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._classid_.Stats>();
            var classId = renderContext.Request.GetParameter<ClassIdParameter>()?.Value;

            if (classId == null)
            {
                return null;
            }

            return uri?.BindParameters(new ClassIdParameter(classId));
        }
    }
}
