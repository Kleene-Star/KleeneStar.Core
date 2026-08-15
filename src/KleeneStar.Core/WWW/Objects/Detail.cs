using KleeneStar.Core.WebFragment.Object;
using System;
using WebExpress.WebApp.WebPage;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Objects
{
    /// <summary>
    /// Resolves an object addressed by id through the <c>id</c> query parameter and
    /// redirects to the reading view of its kind.
    /// </summary>
    /// <remarks>
    /// The page exists to bridge one mismatch. The detail routes of the object kinds
    /// (<c>/issue/{objectkey}</c>, …) are keyed by the object <em>key</em>, while the scrum
    /// backlog reports the object <em>id</em> in its selection event — so the master-detail
    /// of the scrum view cannot address the reading view directly. It points its uri
    /// template here instead, and this page forwards to the real one.
    ///
    /// A redirect rather than a rendered summary, because the detail side is meant to show
    /// the object itself, exactly as the list view's detail frame does. The frame fetches
    /// its uri with the default redirect handling, so it follows the forward transparently
    /// and embeds the reading view it lands on.
    ///
    /// The page is deliberately outside every navigation scope: it is not a destination a
    /// user browses to, but the endpoint the detail frame fetches for the selected row.
    /// </remarks>
    [Title("kleenestar.core:object.scrum.detail.title")]
    public sealed class Detail : IPage<VisualTreeWebApp>
    {
        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var id = renderContext?.Request?.GetParameter("id")?.Value;
            var @object = Guid.TryParse(id, out var objectId)
                ? CoreHub.ObjectManager.GetObject(objectId)
                : null;

            var detailUri = ObjectKindCatalog.ResolveDetailUri(@object);

            if (detailUri is not null)
            {
                throw new RedirectException(detailUri);
            }

            // an id that resolves to nothing is shown rather than redirected, so a stale
            // selection reads as a missing object instead of a broken link
            visualTree.Content.MainPanel.AddPrimary(new ControlEmptyState()
            {
                Icon = _ => new IconMagnifyingGlass(),
                Title = _ => "kleenestar.core:object.scrum.detail.unknown.title",
                Message = _ => "kleenestar.core:object.scrum.detail.unknown.message"
            });
        }
    }
}
