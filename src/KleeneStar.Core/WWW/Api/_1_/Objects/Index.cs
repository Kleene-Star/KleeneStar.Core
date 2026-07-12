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

namespace KleeneStar.Core.WWW.Api._1_.Objects
{
    /// <summary>
    /// Provides CRUD operations for object items via a REST API.
    /// </summary>
    [Cache]
    public sealed class Index : RestApiCrud<Model.Entities.Object>
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
        /// A collection representing the filtered set of index items. 
        /// The collection may be empty if no items match the query.
        /// </returns>
        protected override IEnumerable<Model.Entities.Object> Retrieve(IQuery<Model.Entities.Object> query, IQueryContext context, IRequest request)
        {
            return CoreHub.ObjectManager.GetObjects(query, context);
        }

        /// <summary>
        /// Retrieves the data required to create a new workspace entity.
        /// </summary>
        /// <param name="request">
        /// The request context containing parameters and metadata for the retrieval operation.
        /// </param>
        /// <returns>
        /// An object containing the information necessary to initialize a new workspace for creation.
        /// </returns>
        protected override IRestApiCrudResultRetrieve RetrieveForCreate(IRequest request)
        {
            return base.RetrieveForCreate(request);
        }

        /// <summary>
        /// Retrieves a result object containing default values and metadata for
        /// cloning a item.
        /// </summary>
        /// <remarks>
        /// In addition to the system properties of the source object, the response also
        /// carries the persisted per-field <see cref="Model.Entities.Value"/> rows, keyed
        /// by the field name. This lets the dynamic form inputs built from the active
        /// edit form pre-populate via the form's REST data binding instead of starting
        /// blank.
        /// </remarks>
        /// <param name="query">
        /// An object containing the query parameters used to filter and select index items. Cannot
        /// be null.
        /// </param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A result instance representing the data and metadata required
        /// to initialize a new item for creation.
        /// </returns>
        protected override IRestApiCrudResultRetrieve RetrieveForClone(IQuery<Model.Entities.Object> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.ObjectManager.GetObjects(query, context)
                .FirstOrDefault();

            if (data is null)
            {
                return RetrieveForClone(request, null);
            }

            var newItem = new Model.Entities.Object()
            {
                Summary = data.Summary + " (Copy)",
                Description = data.Description,
                Icon = data.Icon,
                State = WorkspaceState.Active,
                WorkspaceId = data.WorkspaceId,
                ClassId = data.ClassId,
                ParentId = data.ParentId
            };

            var result = RetrieveForClone(request, newItem);
            MergeFieldValues(result, data.Id, data.ClassId);
            return result;
        }

        /// <summary>
        /// Retrieves a workspace identified by the specified key for update operations.
        /// </summary>
        /// <remarks>
        /// In addition to the system properties of the object, the response also carries
        /// the persisted per-field <see cref="Model.Entities.Value"/> rows, keyed by the
        /// field name. This lets the dynamic form inputs built from the active edit form
        /// pre-populate via the form's REST data binding instead of starting blank.
        /// </remarks>
        /// <param name="query">
        /// An object containing the query parameters used to filter and select index items. Cannot
        /// be null.
        /// </param>
        /// <param name="request">
        /// The request context containing additional information for the retrieval operation.
        /// </param>
        /// <returns>
        /// An object containing the workspace associated with the specified key.
        /// </returns>
        protected override IRestApiCrudResultRetrieve RetrieveForUpdate(IQuery<Model.Entities.Object> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.ObjectManager.GetObjects(query, context)
                .FirstOrDefault();

            var result = RetrieveForUpdate(request, data);

            if (data is not null)
            {
                MergeFieldValues(result, data.Id, data.ClassId);
            }

            return result;
        }

        /// <summary>
        /// Adds the persisted field values of the specified object to the JSON data
        /// dictionary returned by the base CRUD retrieval, keyed by field name so the
        /// dynamic form inputs can bind them by name. Inactive or deprecated fields are
        /// skipped to match the structure rendered by the edit form. Existing entries
        /// in the dictionary (system properties such as <c>Summary</c>, <c>Description</c>)
        /// are left untouched.
        /// </summary>
        /// <param name="result">The retrieve result whose <c>Data</c> dictionary is to
        /// be augmented. No-op when the data is not a string-keyed dictionary.</param>
        /// <param name="objectId">The id of the object whose values to merge.</param>
        /// <param name="classId">The id of the object's class, used to look up the
        /// field definitions for name + filtering.</param>
        private static void MergeFieldValues(IRestApiCrudResultRetrieve result, Guid objectId, Guid classId)
        {
            if (result?.Data is not IDictionary<string, object> data)
            {
                return;
            }

            var fields = CoreHub.FieldManager
                .GetFields(new WebParameter.ClassIdParameter(classId))
                .Where(f => !f.Deprecated && f.State == FieldState.Active)
                .ToDictionary(f => f.Id);

            foreach (var value in CoreHub.ValueManager.GetValues(objectId))
            {
                if (!fields.TryGetValue(value.FieldId, out var field))
                {
                    continue;
                }

                data[field.Name] = value.Data;
            }
        }

        /// <summary>
        /// Retrieves the workspace entity identified by the specified ID in preparation for deletion.
        /// </summary>
        /// <param name="query">
        /// An object containing the query parameters used to filter and select index items. Cannot 
        /// be null.
        /// </param>
        /// <param name="request">
        /// The request context containing additional information for 
        /// the retrieval operation.
        /// </param>
        /// <returns>
        /// An object containing the workspace entity and related information required 
        /// for the delete operation.
        /// </returns>
        protected override IRestApiCrudResultRetrieveDelete RetrieveForDelete(IQuery<Model.Entities.Object> query, IRequest request)
        {
            using var context = ModelHub.CreateDbContext();
            var data = CoreHub.ObjectManager.GetObjects(query, context)
                .FirstOrDefault();

            return RetrieveForDelete(request, data, data?.Id.ToString());
        }

        /// <summary>
        /// Validate the data for create or update operations. When creating, existingItem will 
        /// be null and proposedItem contains the values to create. When updating, existingItem 
        /// is the currently persisted entity and proposedItem contains the incoming values to 
        /// validate.
        /// </summary>
        /// <param name="existingItem">
        /// The currently persisted item (null for create).
        /// </param>
        /// <param name="payload">
        /// The dynamic payload containing updated fields.
        /// </param>
        /// <param name="request">
        /// The HTTP request providing additional context.
        /// </param>
        /// <returns>
        /// An IRestApiValidationResult indicating validation success or errors.
        /// </returns>
        protected override IRestApiValidationResult Validate(Model.Entities.Object existingItem, RestApiCrudFormData payload, IRequest request)
        {
            return base.Validate(existingItem, payload, request);
        }

        /// <summary>
        /// Persists the newly created resource.
        /// Override this method in derived classes to implement the actual
        /// persistence logic and return a result describing the creation.
        /// </summary>
        /// <param name="fieldMap">
        /// The dynamic payload containing the fields required to create the resource.
        /// </param>
        /// <param name="request">
        /// The HTTP request providing additional context for the creation process.
        /// </param>
        /// <param name="newItem">
        /// When the method returns, contains the newly created index item,
        /// or the default value if creation was not successful.
        /// </param>
        /// <returns>
        /// A result object containing information about the create operation,
        /// including the created resource.
        /// </returns>
        protected override IRestApiCrudResultCreate Create(RestApiCrudFormData fieldMap, IRequest request, out Model.Entities.Object newItem)
        {
            var id = Guid.NewGuid();
            var currentUser = CoreHub.SessionManager.GetCurrentIdentityId(request);
            newItem = new Model.Entities.Object(id)
            {
                Icon = CoreHub.GenerateIcon(id),
                State = WorkspaceState.Active,
                CreatorId = currentUser,
                UpdaterId = currentUser
            };

            fieldMap.BindTo(newItem);

            CoreHub.ObjectManager.Add(newItem);

            UpsertFieldValues(newItem, fieldMap);

            return new RestApiCrudResultCreate();
        }

        /// <summary>
        /// Creates a new instance by cloning data from the specified form fields and 
        /// adds it to the class manager.
        /// </summary>
        /// <param name="existingItem">
        /// The existing item to use as a reference for the clone operation. This parameter 
        /// is not modified.
        /// </param>
        /// <param name="fieldMap">
        /// The form data containing field values to bind to the new instance. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The current request context for the operation. Provides additional information or 
        /// services required during cloning.
        /// </param>
        /// <param name="newItem">
        /// When this method returns, contains the newly created instance populated 
        /// with the provided form data.
        /// </param>
        /// <returns>
        /// A result object indicating the outcome of the create operation.
        /// </returns>
        protected override IRestApiCrudResultCreate Clone(Model.Entities.Object existingItem, RestApiCrudFormData fieldMap, IRequest request, out Model.Entities.Object newItem)
        {
            var id = Guid.NewGuid();
            var currentUser = CoreHub.SessionManager.GetCurrentIdentityId(request);
            newItem = new Model.Entities.Object(id)
            {
                Icon = CoreHub.GenerateIcon(id),
                State = WorkspaceState.Active,
                CreatorId = currentUser,
                UpdaterId = currentUser
            };

            fieldMap.BindTo(newItem);

            CoreHub.ObjectManager.Add(newItem);

            UpsertFieldValues(newItem, fieldMap);

            return new RestApiCrudResultCreate();
        }

        /// <summary>
        /// Updates the data record.
        /// </summary>
        /// <param name="existingItem">
        /// The currently persisted item.
        /// </param>
        /// <param name="payload">
        /// The dynamic payload containing updated fields.
        /// </param>
        /// <param name="request">
        /// The HTTP request providing additional context.
        /// </param>
        protected override IRestApiCrudResultUpdate Update(Model.Entities.Object existingItem, RestApiCrudFormData payload, IRequest request)
        {
            var res = base.Update(existingItem, payload, request);

            // stamp the identity that performed this update (best-effort; keep the prior
            // updater when the request is unauthenticated so the FK never points at an
            // empty identity).
            var currentUser = CoreHub.SessionManager.GetCurrentIdentityId(request);
            if (currentUser != Guid.Empty)
            {
                existingItem.UpdaterId = currentUser;
            }

            CoreHub.ObjectManager.Update(existingItem);

            UpsertFieldValues(existingItem, payload);

            return res;
        }

        /// <summary>
        /// Persists every payload entry that maps to a configured <see cref="Field"/> of
        /// the object's class as a <see cref="Model.Entities.Value"/> row.
        /// </summary>
        /// <remarks>
        /// The base <see cref="RestApiCrudFormData"/> binder only writes payload entries
        /// that match a public property of <see cref="Model.Entities.Object"/>; any other
        /// key (typically a field name like <c>AffectedCI</c>) is silently dropped. The
        /// inline <c>ControlSmartEdit</c> on the object detail page (see
        /// <c>ObjectItemDetailFragment</c>) PUTs exactly such payloads — a single
        /// <c>{ "FieldName": "new value" }</c> document per edit — so this method fills
        /// the gap by upserting the matching <see cref="Model.Entities.Value"/> row.
        /// Payload keys arrive in lower case (see
        /// <c>JsonExtensionsFieldMap.ToFieldMap</c>); the lookup honours that by
        /// lowering the field names before comparison.
        /// </remarks>
        private static void UpsertFieldValues(Model.Entities.Object @object, RestApiCrudFormData payload)
        {
            if (@object is null || payload is null || payload.Count == 0)
            {
                return;
            }

            var systemProps = typeof(Model.Entities.Object)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Select(p => p.Name.ToLowerInvariant())
                .ToHashSet();

            var fieldsByName = CoreHub.FieldManager
                .GetFields(new WebParameter.ClassIdParameter(@object.ClassId))
                .Where(f => !f.Deprecated && f.State == FieldState.Active)
                .ToDictionary(f => f.Name.ToLowerInvariant(), f => f);

            // load the object's existing values once and index them by field, rather than
            // issuing one ValueManager.GetValue(objectId, fieldId) query per payload entry.
            var existingByField = CoreHub.ValueManager
                .GetValues(@object.Id)
                .GroupBy(v => v.FieldId)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var kv in payload)
            {
                if (systemProps.Contains(kv.Key))
                {
                    // already handled by RestApiCrudFormData.BindTo
                    continue;
                }

                if (!fieldsByName.TryGetValue(kv.Key, out var field))
                {
                    // unknown / removed / deprecated field — drop silently
                    continue;
                }

                var raw = SerializePayloadValue(kv.Value);
                existingByField.TryGetValue(field.Id, out var existing);

                if (existing is null)
                {
                    if (string.IsNullOrEmpty(raw))
                    {
                        continue;
                    }

                    CoreHub.ValueManager.Add(new Model.Entities.Value
                    {
                        ObjectId = @object.Id,
                        FieldId = field.Id,
                        Data = raw,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    });
                }
                else
                {
                    existing.Data = raw;
                    existing.Updated = DateTime.UtcNow;
                    CoreHub.ValueManager.Update(existing);
                }
            }
        }

        /// <summary>
        /// Serializes a single field-payload value into the string form persisted in
        /// <see cref="Model.Entities.Value.Data"/>. Tag-style list payloads collapse to
        /// a comma-separated representation that matches the parse logic of
        /// <c>ObjectItemDetailFragment.BuildInputValue</c>.
        /// </summary>
        private static string SerializePayloadValue(object value)
        {
            return value switch
            {
                null => null,
                string s => s,
                bool b => b ? "true" : "false",
                System.Collections.IEnumerable list and not string => string.Join
                (
                    ",",
                    list.Cast<object>().Where(x => x is not null).Select(x => x.ToString())
                ),
                _ => value.ToString()
            };
        }

        /// <summary>
        /// Deletes the specified resource.
        /// </summary>
        /// <param name="existingItem">
        /// The currently persisted item that is to be deleted.
        /// </param>
        /// <param name="request">
        /// The HTTP request providing additional context for the delete operation.
        /// </param>
        /// <returns>
        /// A result object containing information about the delete operation.
        /// </returns>
        protected override IRestApiCrudResultDelete Delete(Model.Entities.Object existingItem, IRequest request)
        {
            CoreHub.ObjectManager.Remove(existingItem.Id);

            return base.Delete(existingItem, request);
        }
    }
}
