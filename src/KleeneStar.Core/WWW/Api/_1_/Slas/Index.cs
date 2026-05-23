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

namespace KleeneStar.Core.WWW.Api._1_.Slas
{
    /// <summary>
    /// Provides CRUD operations for SLA-policy items via a REST API.
    /// </summary>
    [Cache]
    public sealed class Index : RestApiCrud<SlaPolicy>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <inheritdoc/>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <inheritdoc/>
        protected override IEnumerable<SlaPolicy> Retrieve(IQuery<SlaPolicy> query, IQueryContext context, IRequest request)
        {
            return CoreHub.SlaManager.GetSlas(query, context);
        }

        /// <inheritdoc/>
        protected override IRestApiCrudResultRetrieve RetrieveForCreate(IRequest request)
        {
            return base.RetrieveForCreate(request);
        }

        /// <inheritdoc/>
        protected override IRestApiCrudResultRetrieve RetrieveForClone(IQuery<SlaPolicy> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.SlaManager.GetSlas(query, context).FirstOrDefault();

            if (data is null)
            {
                return RetrieveForClone(request, new SlaPolicy());
            }

            var newItem = new SlaPolicy
            {
                Name = data.Name + " (Copy)",
                Description = data.Description,
                Icon = data.Icon,
                ClassId = data.ClassId,
                State = SlaPolicyState.Draft,
                Priority = data.Priority,
                Calendar = data.Calendar,
                Notifications = data.Notifications,
                PauseOn = data.PauseOn,
                OwnerId = data.OwnerId
            };

            return RetrieveForClone(request, newItem);
        }

        /// <inheritdoc/>
        protected override IRestApiCrudResultRetrieve RetrieveForUpdate(IQuery<SlaPolicy> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.SlaManager.GetSlas(query, context).FirstOrDefault();

            return RetrieveForUpdate(request, data);
        }

        /// <inheritdoc/>
        protected override IRestApiCrudResultRetrieveDelete RetrieveForDelete(IQuery<SlaPolicy> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.SlaManager.GetSlas(query, context).FirstOrDefault();

            return RetrieveForDelete(request, data, data?.Id.ToString());
        }

        /// <inheritdoc/>
        protected override IRestApiValidationResult Validate(SlaPolicy existingItem, RestApiCrudFormData payload, IRequest request)
        {
            return base.Validate(existingItem, payload, request);
        }

        /// <inheritdoc/>
        protected override IRestApiCrudResultCreate Create(RestApiCrudFormData fieldMap, IRequest request, out SlaPolicy newItem)
        {
            var id = Guid.NewGuid();
            newItem = new SlaPolicy(id)
            {
                Icon = CoreHub.GenerateIcon(id),
                State = SlaPolicyState.Draft
            };

            fieldMap.BindTo(newItem);

            CoreHub.SlaManager.Add(newItem);

            CoreHub.AddNotification("Create", "success", 5000);

            return new RestApiCrudResultCreate();
        }

        /// <inheritdoc/>
        protected override IRestApiCrudResultCreate Clone(SlaPolicy existingItem, RestApiCrudFormData fieldMap, IRequest request, out SlaPolicy newItem)
        {
            var id = Guid.NewGuid();
            newItem = new SlaPolicy(id)
            {
                Icon = CoreHub.GenerateIcon(id),
                State = SlaPolicyState.Draft,
                ClassId = existingItem.ClassId,
                Priority = existingItem.Priority,
                Calendar = existingItem.Calendar,
                Notifications = existingItem.Notifications,
                PauseOn = existingItem.PauseOn,
                OwnerId = existingItem.OwnerId
            };

            fieldMap.BindTo(newItem);

            CoreHub.SlaManager.Add(newItem);

            CoreHub.AddNotification("Clone", "success", 5000);

            return new RestApiCrudResultCreate();
        }

        /// <inheritdoc/>
        protected override IRestApiCrudResultUpdate Update(SlaPolicy existingItem, RestApiCrudFormData payload, IRequest request)
        {
            var res = base.Update(existingItem, payload, request);

            CoreHub.SlaManager.Update(existingItem);

            CoreHub.AddNotification("Update", "success", 5000);

            return res;
        }

        /// <inheritdoc/>
        protected override IRestApiCrudResultDelete Delete(SlaPolicy existingItem, IRequest request)
        {
            CoreHub.SlaManager.Remove(existingItem.Id);

            return base.Delete(existingItem, request);
        }
    }
}
