using KleeneStar.Core.WebAttribute;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Settings.Group._groupid_
{
    /// <summary>
    /// Represents the main page for a group.
    /// </summary>
    [WebIcon<IconLayerGroup>]
    [Title("kleenestar.core:setting.group.manage.label")]
    [GroupIdSegment]
    [Scope<IScopeGeneral>]
    [Domain<Model.Entities.Group>]
    [Cache]
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
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
        }
    }
}
