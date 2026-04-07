using KleeneStar.Core.WebIcon;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;

namespace KleeneStar.Core.WWW
{
    /// <summary>
    /// Represents the home page of the application. Displays the list of available dashboards
    /// in the sidebar and, when a dashboard is selected, shows its content on the right.
    /// </summary>
    [WebIcon<KleeneStarIcon>]
    [Title("kleenestar.core:kleenestar.label")]
    [Scope<IScopeGeneral>]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
        }
    }
}
