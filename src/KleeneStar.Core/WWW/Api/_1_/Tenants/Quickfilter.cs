using KleeneStar.Core.WebQuickfilter;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Tenants
{
    /// <summary>
    /// Provides a quick filter implementation for tenant entities in the REST API. 
    /// This class enables filtering of filed items using predefined criteria.
    /// </summary>
    public sealed class Quickfilter : RestApiQuickfilter<Model.Entities.Tenant>
    {
        /// <summary>
        /// The key under which the quickfilters a user defined for this view are stored.
        /// </summary>
        /// <remarks>
        /// The bar and the table have to agree on it, so it is named once here and read from both.
        /// </remarks>
        public const string ViewKey = "tenants";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Quickfilter()
        {
        }

        /// <summary>
        /// Retrieves a queryable collection of index items.
        /// </summary>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// An enumerable collection of quick filter items that match the 
        /// specified context and request. The collection may be empty if 
        /// no items are found.
        /// </returns>
        protected override IEnumerable<RestApiQuickfilterItem> RetrieveItems(IQueryContext context, IRequest request)
        {
            yield return new RestApiQuickfilterItem()
            {
                Id = "qf_active",
                Name = "Active"
            };

            // the filters the users defined follow the ones the view ships with, so the familiar
            // chips keep their position as the personal ones come and go
            foreach (var item in CustomQuickfilterSupport.Items(ViewKey, null, request))
            {
                yield return item;
            }
        }
    }
}
