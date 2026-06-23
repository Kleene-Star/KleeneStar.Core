using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.SavedSearches
{
    // The entity type SavedSearch collides with the sibling WWW.SavedSearch namespace;
    // alias it (inside the namespace block) so the bare name binds to the entity.
    using SavedSearch = KleeneStar.Model.Entities.SavedSearch;

    /// <summary>
    /// Provides CRUD operations for saved-search items via a REST API. Backs the add, edit,
    /// and delete modal forms reached from the search-page sidebar.
    /// </summary>
    [Cache]
    public sealed class Index : RestApiCrud<SavedSearch>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Retrieves the response for the specified request using the configured retrieval logic.
        /// </summary>
        /// <param name="request">The request object. Must not be null.</param>
        /// <returns>The retrieval response.</returns>
        [Method(RequestMethod.GET)]
        public override IResponse Retrieve(IRequest request)
        {
            return base.Retrieve(request);
        }

        /// <summary>
        /// Creates a new query context backed by the application database.
        /// </summary>
        /// <returns>An <see cref="IQueryContext"/> instance.</returns>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Retrieves the saved searches matching the specified query.
        /// </summary>
        /// <param name="query">The query parameters. Cannot be null.</param>
        /// <param name="context">The context in which the query is executed. Cannot be null.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The matching saved searches.</returns>
        protected override IEnumerable<SavedSearch> Retrieve(IQuery<SavedSearch> query, IQueryContext context, IRequest request)
        {
            return CoreHub.SavedSearchManager.GetSavedSearches(query, context);
        }

        /// <summary>
        /// Retrieves the data required to create a new saved search.
        /// </summary>
        /// <param name="request">The request context.</param>
        /// <returns>The data required to initialize a new saved search for creation.</returns>
        protected override IRestApiCrudResultRetrieve RetrieveForCreate(IRequest request)
        {
            return base.RetrieveForCreate(request);
        }

        /// <summary>
        /// Retrieves a saved search identified by the query for update operations.
        /// </summary>
        /// <param name="query">The query parameters. Cannot be null.</param>
        /// <param name="request">The request context.</param>
        /// <returns>The saved search for update.</returns>
        protected override IRestApiCrudResultRetrieve RetrieveForUpdate(IQuery<SavedSearch> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.SavedSearchManager.GetSavedSearches(query, context)
                .FirstOrDefault();

            return RetrieveForUpdate(request, data);
        }

        /// <summary>
        /// Retrieves the saved search identified by the query in preparation for deletion.
        /// </summary>
        /// <param name="query">The query parameters. Cannot be null.</param>
        /// <param name="request">The request context.</param>
        /// <returns>The saved search and metadata for deletion.</returns>
        protected override IRestApiCrudResultRetrieveDelete RetrieveForDelete(IQuery<SavedSearch> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.SavedSearchManager.GetSavedSearches(query, context)
                .FirstOrDefault();

            return RetrieveForDelete(request, data, data?.Name);
        }

        /// <summary>
        /// Validates the data for create or update operations.
        /// </summary>
        /// <param name="existingItem">The currently persisted item (null for create).</param>
        /// <param name="payload">The dynamic payload containing the fields.</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The validation result.</returns>
        protected override IRestApiValidationResult Validate(SavedSearch existingItem, RestApiCrudFormData payload, IRequest request)
        {
            return base.Validate(existingItem, payload, request);
        }

        /// <summary>
        /// Persists a newly created saved search owned by the calling identity.
        /// </summary>
        /// <param name="fieldMap">The form payload (Name, Query, Description, Starred).</param>
        /// <param name="request">The HTTP request.</param>
        /// <param name="newItem">The created saved search.</param>
        /// <returns>The create result.</returns>
        protected override IRestApiCrudResultCreate Create(RestApiCrudFormData fieldMap, IRequest request, out SavedSearch newItem)
        {
            var now = DateTime.UtcNow;
            newItem = new SavedSearch(Guid.NewGuid())
            {
                OwnerId = CoreHub.SessionManager.GetCurrentIdentityId(request),
                State = SavedSearchState.Active,
                LastUsed = now,
                Created = now,
                Updated = now
            };

            fieldMap.BindTo(newItem);

            CoreHub.SavedSearchManager.Add(newItem);

            return new RestApiCrudResultCreate();
        }

        /// <summary>
        /// Clones an existing saved search into a new one owned by the calling identity.
        /// </summary>
        /// <param name="existingItem">The source saved search.</param>
        /// <param name="fieldMap">The form payload.</param>
        /// <param name="request">The HTTP request.</param>
        /// <param name="newItem">The created saved search.</param>
        /// <returns>The create result.</returns>
        protected override IRestApiCrudResultCreate Clone(SavedSearch existingItem, RestApiCrudFormData fieldMap, IRequest request, out SavedSearch newItem)
        {
            var now = DateTime.UtcNow;
            newItem = new SavedSearch(Guid.NewGuid())
            {
                OwnerId = CoreHub.SessionManager.GetCurrentIdentityId(request),
                State = SavedSearchState.Active,
                LastUsed = now,
                Created = now,
                Updated = now
            };

            fieldMap.BindTo(newItem);

            CoreHub.SavedSearchManager.Add(newItem);

            return new RestApiCrudResultCreate();
        }

        /// <summary>
        /// Updates an existing saved search.
        /// </summary>
        /// <param name="existingItem">The currently persisted item.</param>
        /// <param name="payload">The dynamic payload containing updated fields.</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The update result.</returns>
        protected override IRestApiCrudResultUpdate Update(SavedSearch existingItem, RestApiCrudFormData payload, IRequest request)
        {
            var res = base.Update(existingItem, payload, request);

            CoreHub.SavedSearchManager.Update(existingItem);

            return res;
        }

        /// <summary>
        /// Deletes the specified saved search.
        /// </summary>
        /// <param name="existingItem">The currently persisted item to delete.</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The delete result.</returns>
        protected override IRestApiCrudResultDelete Delete(SavedSearch existingItem, IRequest request)
        {
            CoreHub.SavedSearchManager.Remove(existingItem.Id);

            return base.Delete(existingItem, request);
        }
    }
}
