using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Slas._classid_
{
    /// <summary>
    /// Quick-filter selection for SLA-policy rows. Exposes the lifecycle states as one-tap
    /// filter chips.
    /// </summary>
    public sealed class Quickfilter : RestApiQuickfilter<SlaPolicy>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Quickfilter()
        {
        }

        /// <summary>
        /// Returns the quick-filter chips displayed above the SLA-policy table. Selecting
        /// a chip narrows the table to one of the lifecycle states or to the critical
        /// priority bucket.
        /// </summary>
        /// <param name="context">The query context. Ignored — the chips are fixed.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The fixed set of quick-filter items.</returns>
        protected override IEnumerable<RestApiQuickfilterItem> RetrieveItems(IQueryContext context, IRequest request)
        {
            yield return new RestApiQuickfilterItem { Id = "qf_active",    Name = "Active" };
            yield return new RestApiQuickfilterItem { Id = "qf_draft",     Name = "Draft" };
            yield return new RestApiQuickfilterItem { Id = "qf_inactive",  Name = "Inactive" };
            yield return new RestApiQuickfilterItem { Id = "qf_atrisk",    Name = "Critical" };
        }
    }
}
