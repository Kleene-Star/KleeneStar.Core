using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Classes._workspacekey_._classid_
{
    /// <summary>
    /// Represents the page for deleting a workspace within the web application. Provides access to the
    /// workspace edit form and handles form processing and rendering.
    /// </summary>
    [WebIcon<IconTrashAlt>]
    [Title("kleenestar.core:calss.delete.title")]
    [Scope<IScopeGeneral>]
    public sealed class Delete : IPage<VisualTreeWebApp>, IScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Delete()
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
