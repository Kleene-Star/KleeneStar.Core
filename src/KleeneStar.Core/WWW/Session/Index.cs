using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Session
{
    /// <summary>
    /// Represents the state page within the kleenestar web application.
    /// </summary>
    [WebIcon<IconLocationPinLock>]
    [SegmentHidden]
    [Title("kleenestar.core:login.title")]
    [Scope<IScopeGeneral>]
    [Scope<IScopeAdmin>]
    [Scope<IScopeLogin>]
    [Cache]
    public sealed class Index : PageWebAppLogin, IScopeLogin
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
            //RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Session>();
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public override void Process(IRenderContext renderContext, VisualTreeWebAppLogin visualTree)
        {
            visualTree.LoginUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Session>();
        }
    }
}
