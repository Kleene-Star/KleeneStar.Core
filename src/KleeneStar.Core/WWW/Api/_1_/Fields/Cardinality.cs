using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Fields
{
    /// <summary>
    /// Represents a selectable cardinality for use in REST API selection scenarios.
    /// </summary>
    [Title("Field cardinality")]
    public sealed class Cardinality : RestApiSelection<Model.Entities.Field>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Cardinality()
        {
        }

        /// <summary>
        /// Retrieves a queryable collection of index items that match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// An object containing the query parameters used to filter and select index items. Cannot 
        /// be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// An enumerable collection of selection items that satisfy the query 
        /// criteria. The collection is empty if no items match.
        /// </returns>
        protected override IQuery<Model.Entities.Field> Filter(string filter, IQuery<Model.Entities.Field> query, IRequest request)
        {
            return query;
        }

        /// <summary>
        /// Retrieves a queryable collection of index items that match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// An object containing the query parameters used to filter and select index items. Cannot 
        /// be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// An enumerable collection of selection items that satisfy the query 
        /// criteria. The collection is empty if no items match.
        /// </returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<Model.Entities.Field> query, IQueryContext context, IRequest request)
        {
            var list = new List<RestApiSelectionItem>()
            {
                new()
                {
                    Id = FieldCardinality.Single.Id(),
                    Text = I18N.Translate(request, FieldCardinality.Single.Text()),
                    Color = FieldCardinality.Single.Color()
                },
                new()
                {
                    Id = FieldCardinality.Multiple.Id(),
                    Text = I18N.Translate(request, FieldCardinality.Multiple.Text()),
                    Color = FieldCardinality.Multiple.Color()
                }
            };

            return list.AsQueryable();
        }
    }
}
