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

namespace KleeneStar.Core.WWW.Api._1_.Identities
{
    /// <summary>
    /// Provides CRUD operations for identity items via a REST API.
    /// </summary>
    [Cache]
    public sealed class Index : RestApiCrud<Model.Entities.Identity>
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
        /// <returns>An IQueryContext instance.</returns>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Retrieves identity items matching the query.
        /// </summary>
        protected override IEnumerable<Model.Entities.Identity> Retrieve(IQuery<Model.Entities.Identity> query, IQueryContext context, IRequest request)
        {
            return CoreHub.IdentityManager.GetIdentities(query, context);
        }

        /// <summary>
        /// Retrieves data for creating a new identity.
        /// </summary>
        protected override IRestApiCrudResultRetrieve RetrieveForCreate(IRequest request)
        {
            return base.RetrieveForCreate(request);
        }

        /// <summary>
        /// Retrieves data for cloning an identity.
        /// </summary>
        protected override IRestApiCrudResultRetrieve RetrieveForClone(IQuery<Model.Entities.Identity> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.IdentityManager.GetIdentities(query, context)
                .FirstOrDefault();

            var newItem = new Model.Entities.Identity()
            {
                Name = data.Name + " (Copy)",
                Email = data.Email,
                Avatar = data.Avatar,
                State = IdentityState.Active
            };

            return RetrieveForClone(request, newItem);
        }

        /// <summary>
        /// Retrieves data for updating an identity.
        /// </summary>
        protected override IRestApiCrudResultRetrieve RetrieveForUpdate(IQuery<Model.Entities.Identity> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.IdentityManager.GetIdentities(query, context)
                .FirstOrDefault();

            return RetrieveForUpdate(request, data);
        }

        /// <summary>
        /// Retrieves data for deleting an identity.
        /// </summary>
        protected override IRestApiCrudResultRetrieveDelete RetrieveForDelete(IQuery<Model.Entities.Identity> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.IdentityManager.GetIdentities(query, context)
                .FirstOrDefault();

            return RetrieveForDelete(request, data, data?.Id.ToString());
        }

        /// <summary>
        /// Validates the data for create or update operations.
        /// </summary>
        protected override IRestApiValidationResult Validate(Model.Entities.Identity existingItem, RestApiCrudFormData payload, IRequest request)
        {
            return base.Validate(existingItem, payload, request);
        }

        /// <summary>
        /// Creates a new identity.
        /// </summary>
        protected override IRestApiCrudResultCreate Create(RestApiCrudFormData fieldMap, IRequest request, out Model.Entities.Identity newItem)
        {
            var id = Guid.NewGuid();
            newItem = new Model.Entities.Identity(id)
            {
                Avatar = CoreHub.GenerateIcon(id),
                State = IdentityState.Active
            };

            fieldMap.BindTo(newItem);

            CoreHub.IdentityManager.Add(newItem);

            return new RestApiCrudResultCreate();
        }

        /// <summary>
        /// Clones an existing identity.
        /// </summary>
        protected override IRestApiCrudResultCreate Clone(Model.Entities.Identity existingItem, RestApiCrudFormData fieldMap, IRequest request, out Model.Entities.Identity newItem)
        {
            var id = Guid.NewGuid();
            newItem = new Model.Entities.Identity(id)
            {
                Avatar = CoreHub.GenerateIcon(id),
                State = IdentityState.Active
            };

            fieldMap.BindTo(newItem);

            CoreHub.IdentityManager.Add(newItem);

            return new RestApiCrudResultCreate();
        }

        /// <summary>
        /// Updates an existing identity.
        /// </summary>
        protected override IRestApiCrudResultUpdate Update(Model.Entities.Identity existingItem, RestApiCrudFormData payload, IRequest request)
        {
            var res = base.Update(existingItem, payload, request);

            CoreHub.IdentityManager.Update(existingItem);

            return res;
        }

        /// <summary>
        /// Deletes an identity.
        /// </summary>
        protected override IRestApiCrudResultDelete Delete(Model.Entities.Identity existingItem, IRequest request)
        {
            CoreHub.IdentityManager.Remove(existingItem.Id);

            return base.Delete(existingItem, request);
        }
    }
}
