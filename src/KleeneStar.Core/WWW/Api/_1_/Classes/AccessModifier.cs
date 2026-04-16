using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Classes
{
    /// <summary>
    /// Represents a selectable access modifier for use in REST API selection scenarios.
    /// </summary>
    [Title("Class access modifier")]
    public sealed class AccessModifier : RestApiSelection<Model.Entities.Class>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AccessModifier()
        {
        }

        /// <summary>
        /// Applies the specified filter criteria to the given query object.
        /// </summary>
        /// <param name="filter">
        /// A string representing the filter expression to apply.
        /// </param>
        /// <param name="query">
        /// The query object to which the filter will be applied.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// A query representing the filtered set of items.
        /// </returns>
        protected override IQuery<Model.Entities.Class> Filter(string filter, IQuery<Model.Entities.Class> query, IRequest request)
        {
            return query;
        }

        /// <summary>
        /// Retrieves a queryable collection of access modifier items.
        /// </summary>
        /// <param name="query">
        /// An object containing the query parameters used to filter and select index items.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// An enumerable collection of selection items.
        /// </returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<Model.Entities.Class> query, IQueryContext context, IRequest request)
        {
            var list = new List<RestApiSelectionItem>()
            {
                new() { Id = Guid.Empty, Text = "Private" },
                new() { Id = Guid.Empty, Text = "Protected" },
                new() { Id = Guid.Empty, Text = "Public" },
                new() { Id = Guid.Empty, Text = "Internal" }
            };

            return list.AsQueryable();
        }
    }
}
