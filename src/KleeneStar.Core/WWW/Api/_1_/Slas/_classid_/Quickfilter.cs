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

        /// <inheritdoc/>
        protected override IEnumerable<RestApiQuickfilterItem> RetrieveItems(IQueryContext context, IRequest request)
        {
            yield return new RestApiQuickfilterItem { Id = "qf_active",    Name = "Active" };
            yield return new RestApiQuickfilterItem { Id = "qf_draft",     Name = "Draft" };
            yield return new RestApiQuickfilterItem { Id = "qf_inactive",  Name = "Inactive" };
            yield return new RestApiQuickfilterItem { Id = "qf_atrisk",    Name = "Critical" };
        }
    }
}
