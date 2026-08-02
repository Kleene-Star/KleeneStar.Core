using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using System;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace KleeneStar.Core.WWW.Api._1_.Transitions._objectkey_
{
    /// <summary>
    /// REST endpoint backing the state picker on the object's workflow card
    /// (<see cref="WebFragment.Object.ObjectPropertyWorkflowCardFragment"/>). The URL is
    /// <c>/api/1/transitions/{objectkey}?fieldid={fieldid}&amp;stateid={stateid}</c>; the
    /// <c>{objectkey}</c> URL segment is declared via <see cref="ObjectKeySegmentAttribute"/>
    /// so callers can bind it from an <see cref="ObjectKeyParameter"/>.
    /// </summary>
    /// <remarks>
    /// A <c>GET</c> moves the addressed object's workflow-backed field to the requested state
    /// and then issues a <c>302</c> redirect back to the object detail page, so a plain
    /// navigation link inside a dropdown can drive the change without any client-side
    /// scripting — the same shape the assignee toggle uses. The state machine itself is
    /// enforced by <see cref="IWorkflowManager.ExecuteTransition"/>: this endpoint only
    /// translates the request into that call and turns its outcome into a toast.
    /// </remarks>
    [Title("kleenestar.core:object.transition.api.title")]
    [ObjectKeySegment]
    [Cache]
    public sealed class Index : IRestApi
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Handles <c>GET {base}</c>: moves the addressed object's workflow field to the
        /// requested state and redirects to the object detail page.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <returns>A <c>302</c> redirect to the object detail page.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Execute(IRequest request)
        {
            var keyParameter = request?.GetParameter<ObjectKeyParameter>();
            var @object = CoreHub.ObjectManager.GetObjectByKey(keyParameter?.Value);

            if (@object is not null &&
                Guid.TryParse(request?.GetParameter<FieldIdParameter>()?.Value, out var fieldId) &&
                Guid.TryParse(request?.GetParameter<WorkflowStateIdParameter>()?.Value, out var stateId))
            {
                var identityId = CoreHub.SessionManager.GetCurrentIdentityId(request);
                var result = CoreHub.WorkflowManager.ExecuteTransition(@object.Id, fieldId, stateId, identityId);

                Report(result);
            }

            // dispatch to the detail view matching the object's kind (/issue, /document, …)
            var target = global::KleeneStar.Core.WebFragment.Object.ObjectKindCatalog
                .ResolveDetailUri(@object)?
                .BindParameters(request);

            return new ResponseMovedTemporarily(target);
        }

        /// <summary>
        /// Surfaces a refused state change as a toast, because the redirect would otherwise
        /// return the user to an unchanged page with no explanation. A change that went
        /// through stays silent here: it is visible on the page the redirect lands on, and
        /// stamping the object already raises the "object updated" toast, so reporting it a
        /// second time would only stack notifications. A no-op change is not worth a toast.
        /// </summary>
        /// <param name="result">The outcome reported by the workflow manager.</param>
        private static void Report(WorkflowTransitionResult result)
        {
            if (result is null || result.Succeeded || result.Outcome == WorkflowTransitionOutcome.Unchanged)
            {
                return;
            }

            CoreHub.AddNotification
            (
                "kleenestar.core:notification.title.error",
                result.Message,
                5000
            );
        }
    }
}
