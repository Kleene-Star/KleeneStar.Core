using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Identities
{
    /// <summary>
    /// Provides quick filter for identity entities.
    /// </summary>
    public sealed class Quickfilter : RestApiQuickfilter<Model.Entities.Identity>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Quickfilter()
        {
        }

        /// <summary>
        /// Retrieves quick filter items.
        /// </summary>
        protected override IEnumerable<RestApiQuickfilterItem> RetrieveItems(IQueryContext context, IRequest request)
        {
            yield return new RestApiQuickfilterItem()
            {
                Id = "qf_active",
                Name = "Active"
            };
        }
    }
}
