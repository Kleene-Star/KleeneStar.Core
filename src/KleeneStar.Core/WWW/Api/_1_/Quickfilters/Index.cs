using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Quickfilters
{
    /// <summary>
    /// Provides CRUD operations for the quickfilters the users defined themselves.
    /// </summary>
    /// <remarks>
    /// The endpoint is shared by every view: which bar a filter belongs to is carried in the
    /// payload rather than in the route, so a further view needs no endpoint of its own.
    /// </remarks>
    [Cache]
    public sealed class Index : RestApiCrud<CustomQuickfilter>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Creates a new instance of an object that implements the IQueryContext interface.
        /// </summary>
        /// <returns>
        /// An IQueryContext instance that can be used to execute queries.
        /// </returns>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Retrieves the quickfilters the calling identity may see.
        /// </summary>
        /// <remarks>
        /// The result is narrowed to the caller's own and the shared filters, so a listing can
        /// never hand out what another identity defined for itself.
        /// </remarks>
        /// <param name="query">
        /// An object containing the query parameters used to filter and select the quickfilters.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// A collection representing the filtered set of quickfilters.
        /// </returns>
        protected override IEnumerable<CustomQuickfilter> Retrieve(IQuery<CustomQuickfilter> query, IQueryContext context, IRequest request)
        {
            var identityId = CoreHub.SessionManager.GetCurrentIdentityId(request);

            return CoreHub.CustomQuickfilterManager
                .GetCustomQuickfilters(query, context)
                .Where(x => x.Shared || x.OwnerId == identityId);
        }

        /// <summary>
        /// Persists a newly defined quickfilter owned by the calling identity.
        /// </summary>
        /// <param name="fieldMap">
        /// The form payload carrying the name, the expression, the view it belongs to and whether
        /// it is shared.
        /// </param>
        /// <param name="request">
        /// The HTTP request providing additional context for the creation process.
        /// </param>
        /// <param name="newItem">
        /// When the method returns, contains the newly created quickfilter.
        /// </param>
        /// <returns>
        /// A result object containing information about the create operation.
        /// </returns>
        protected override IRestApiCrudResultCreate Create(RestApiCrudFormData fieldMap, IRequest request, out CustomQuickfilter newItem)
        {
            var now = DateTime.UtcNow;

            newItem = new CustomQuickfilter(Guid.NewGuid())
            {
                OwnerId = CoreHub.SessionManager.GetCurrentIdentityId(request),
                Created = now,
                Updated = now
            };

            fieldMap.BindTo(newItem);

            // which bar the filter belongs to comes from the address the dialog was opened for,
            // not from the form: it is not the user's to choose, and binding it from the payload
            // would let a filter be planted in a view the user never opened
            newItem.ViewKey = request?.GetParameter("view")?.Value;
            var context = request?.GetParameter("context")?.Value;
            newItem.ContextKey = string.IsNullOrWhiteSpace(context) ? null : context;

            if (string.IsNullOrWhiteSpace(newItem.ViewKey))
            {
                return null;
            }

            CoreHub.CustomQuickfilterManager.Add(newItem);

            return new RestApiCrudResultCreate();
        }

        /// <summary>
        /// Clones an existing quickfilter into a new one owned by the calling identity.
        /// </summary>
        /// <param name="existingItem">
        /// The source quickfilter. Not modified.
        /// </param>
        /// <param name="fieldMap">
        /// The form payload containing the field values to bind to the new instance.
        /// </param>
        /// <param name="request">
        /// The current request context for the operation.
        /// </param>
        /// <param name="newItem">
        /// When this method returns, contains the newly created quickfilter.
        /// </param>
        /// <returns>
        /// A result object indicating the outcome of the create operation.
        /// </returns>
        protected override IRestApiCrudResultCreate Clone(CustomQuickfilter existingItem, RestApiCrudFormData fieldMap, IRequest request, out CustomQuickfilter newItem)
        {
            var now = DateTime.UtcNow;

            newItem = new CustomQuickfilter(Guid.NewGuid())
            {
                OwnerId = CoreHub.SessionManager.GetCurrentIdentityId(request),
                ViewKey = existingItem?.ViewKey,
                ContextKey = existingItem?.ContextKey,
                Ordinal = existingItem?.Ordinal ?? 0,
                Created = now,
                Updated = now
            };

            fieldMap.BindTo(newItem);

            CoreHub.CustomQuickfilterManager.Add(newItem);

            return new RestApiCrudResultCreate();
        }

        /// <summary>
        /// Updates a stored quickfilter.
        /// </summary>
        /// <remarks>
        /// The view a filter belongs to and its owner are not taken from the payload, so an edit
        /// cannot move a filter into another bar or hand it to somebody else.
        /// </remarks>
        /// <param name="existingItem">The currently persisted quickfilter.</param>
        /// <param name="payload">The dynamic payload containing the edited quickfilter.</param>
        /// <param name="request">The HTTP request providing additional context.</param>
        /// <returns>A result object containing information about the update operation.</returns>
        protected override IRestApiCrudResultUpdate Update(CustomQuickfilter existingItem, RestApiCrudFormData payload, IRequest request)
        {
            var viewKey = existingItem.ViewKey;
            var contextKey = existingItem.ContextKey;
            var ownerId = existingItem.OwnerId;

            var res = base.Update(existingItem, payload, request);

            existingItem.ViewKey = viewKey;
            existingItem.ContextKey = contextKey;
            existingItem.OwnerId = ownerId;

            CoreHub.CustomQuickfilterManager.Update(existingItem);

            return res;
        }

        /// <summary>
        /// Deletes a stored quickfilter.
        /// </summary>
        /// <param name="existingItem">The quickfilter that is to be deleted.</param>
        /// <param name="request">The HTTP request providing additional context.</param>
        /// <returns>A result object containing information about the delete operation.</returns>
        protected override IRestApiCrudResultDelete Delete(CustomQuickfilter existingItem, IRequest request)
        {
            CoreHub.CustomQuickfilterManager.Remove(existingItem.Id);

            return base.Delete(existingItem, request);
        }
    }
}
