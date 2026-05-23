using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Calendar._calendarid_
{
    /// <summary>
    /// Form page for editing an existing <see cref="Model.Entities.Calendar"/>.
    /// </summary>
    [WebIcon<IconPen>]
    [Title("kleenestar.core:calendar.edit.title")]
    [Scope<IScopeGeneral>]
    public sealed class Edit : IPage<VisualTreeWebApp>, IScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Edit()
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
