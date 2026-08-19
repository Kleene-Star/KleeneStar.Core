using KleeneStar.Core.WebFragment.Object;
using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using System;
using System.Globalization;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Issue._objectkey_
{
    /// <summary>
    /// Reapplies the state an object held at one revision, addressed by
    /// <c>?commit={number|id}</c>, and redirects back to the object detail page.
    /// </summary>
    /// <remarks>
    /// The restore is a navigation rather than a background call so that no client-side scripting
    /// is needed to drive it and — more importantly — so that the page the user lands on is the
    /// object itself, showing the values that were put back. A restore that stayed inside the
    /// dialog would leave the object behind it stale until the next reload.
    /// <para>
    /// Nothing is rewound: <see cref="ICommitManager.RestoreCommit"/> appends a new commit of type
    /// <c>Restored</c> describing what it wrote, so the chain still reads forward.
    /// </para>
    /// </remarks>
    [WebIcon<IconArrowRotateLeft>]
    [Title("kleenestar.core:object.history.restore.title")]
    [Scope<IScopeGeneral>]
    // the grant this page needs is object_restore_state, carried by ObjectEditPolicy. It is left
    // inactive here for the same reason every other page-level policy in this application is
    // (see Workspaces/Index.cs and siblings): page-level gating rejects every request while the
    // identity flow does not yet put an authenticated user on it, and the page would answer an
    // empty document rather than act. The gate that does work today sits on
    // ObjectHistoryRestoreFragment, which is what offers the button.
    //[Policy<ObjectEditPolicy>]
    public sealed class HistoryRestore : IPage<VisualTreeWebApp>, IScope
    {
        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="objectManager">
        /// The object manager used to resolve the addressed object. Cannot be null.
        /// </param>
        public HistoryRestore(IObjectManager objectManager)
        {
            _objectManager = objectManager;
        }

        /// <summary>
        /// Processing of the resource: reapplies the addressed revision and redirects to the
        /// object detail view matching the object's kind.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<ObjectKeyParameter>();
            var @object = _objectManager.GetObjectByKey(keyParameter);

            if (@object is not null)
            {
                Restore(@object, renderContext);
            }

            throw new RedirectException
            (
                ObjectKindCatalog.ResolveDetailUri(@object)?.BindParameters(renderContext.Request)
                    ?? CoreHub.GetUri<Index>().BindParameters(new ObjectKeyParameter(keyParameter?.Value))
            );
        }

        /// <summary>
        /// Reapplies the addressed revision and reports the outcome as a notification, so the
        /// user learns what happened on the page the redirect lands on.
        /// </summary>
        /// <param name="object">The object whose state is restored.</param>
        /// <param name="renderContext">The render context carrying the request.</param>
        private static void Restore(Model.Entities.Object @object, IRenderContext renderContext)
        {
            var raw = renderContext.Request.GetParameter(HistoryDetail.CommitParameter)?.Value;
            var number = ResolveNumber(@object.Id, raw);

            if (number is null)
            {
                return;
            }

            var identityId = CoreHub.SessionManager.GetCurrentIdentityId(renderContext.Request);
            var result = CoreHub.CommitManager.RestoreCommit(@object.Id, number.Value, identityId);

            if (result?.Changed != true)
            {
                return;
            }

            CoreHub.AddNotification
            (
                "kleenestar.core:object.history.restore.notification.title",
                "kleenestar.core:object.history.restore.notification.message",
                @object
            );
        }

        /// <summary>
        /// Resolves the revision number from the query parameter, which may carry either the
        /// number itself or the commit's id.
        /// </summary>
        /// <param name="objectId">The id of the object.</param>
        /// <param name="raw">The raw query value.</param>
        /// <returns>The revision number, or <c>null</c> when it names none.</returns>
        private static int? ResolveNumber(Guid objectId, string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            if (Guid.TryParse(raw, out var commitId))
            {
                var commit = CoreHub.CommitManager.GetCommit(commitId);

                return commit?.ObjectId == objectId ? commit.Number : null;
            }

            return int.TryParse(raw.TrimStart('#'), NumberStyles.Integer, CultureInfo.InvariantCulture, out var number) && number > 0
                ? number
                : null;
        }
    }
}
