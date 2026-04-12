using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Settings.Group._groupid_
{
    /// <summary>
    /// Represents a page for cloning a group.
    /// </summary>
    [WebIcon<IconCopy>]
    [Title("kleenestar.core:setting.group.clone.title")]
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
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
        }
    }
}
