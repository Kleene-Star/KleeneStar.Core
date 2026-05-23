using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Calendar._calendarid_
{
    /// <summary>
    /// Page for cloning an existing <see cref="Model.Entities.Calendar"/>.
    /// </summary>
    [WebIcon<IconClone>]
    [Title("kleenestar.core:calendar.clone.title")]
    [Scope<IScopeGeneral>]
    public sealed class Clone : IPage<VisualTreeWebApp>, IScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Clone()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
        }
    }
}
