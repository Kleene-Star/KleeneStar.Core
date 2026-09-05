using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Documents._workspacekey_
{
    /// <summary>
    /// The home picker of a workspace's document overview: which of its documents the overview
    /// opens on. The URL is <c>/documents/{workspacekey}/home</c>.
    /// </summary>
    /// <remarks>
    /// It is a page rather than a control on the overview because it is opened as a modal, and a
    /// modal is addressed by URL: the more menu of the overview points an
    /// <c>ActionModal</c> at this route. The form itself is contributed by
    /// <see cref="WebFragment.Object.Documents.DocumentHomeFormFragment"/>.
    /// <para>
    /// The <c>{workspacekey}</c> segment is declared by the sibling <see cref="Index"/> page, so
    /// this sibling must NOT redeclare it.
    /// </para>
    /// </remarks>
    [WebIcon<IconHouse>]
    [Title("kleenestar.core:workspace.home.title")]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class Home : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Home()
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
