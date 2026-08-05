using WebExpress.WebCore.WebCondition;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WebFragment.Maintenance
{
    /// <summary>
    /// Represents a condition that determines whether no maintenance notice is currently being
    /// made to the users.
    /// </summary>
    /// <remarks>
    /// It is the exact complement of <see cref="MaintenanceNoticeCondition"/> and is derived from
    /// it rather than repeating the question, so the two can never disagree and leave the toast
    /// both filled and suppressed, or neither.
    /// </remarks>
    internal class MaintenanceNoNoticeCondition : ICondition
    {
        private readonly MaintenanceNoticeCondition _notice = new();

        /// <summary>
        /// Determines whether no instruction text is to be shown.
        /// </summary>
        /// <param name="request">The request the condition is evaluated for.</param>
        /// <returns>True when there is nothing to announce.</returns>
        public bool Fulfillment(IRequest request)
        {
            return !_notice.Fulfillment(request);
        }
    }
}
