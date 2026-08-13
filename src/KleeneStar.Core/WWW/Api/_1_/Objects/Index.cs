using KleeneStar.Core.WebRestApi;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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

            // BindTo drops guid-typed properties, so the references an object cannot exist
            // without are bound explicitly
            BindReferences(fieldMap, newItem);
            BindSystemProperties(newItem, fieldMap);
            EnsureKey(newItem);

            CoreHub.ObjectManager.Add(newItem);

            UpsertFieldValues(newItem, fieldMap);

            ApplyTemplate(newItem, fieldMap, request);

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

            newItem.ClassId = existingItem?.ClassId ?? newItem.ClassId;
            newItem.WorkspaceId = existingItem?.WorkspaceId ?? newItem.WorkspaceId;
            BindReferences(fieldMap, newItem);
            EnsureKey(newItem);

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

            BindSystemProperties(existingItem, payload);

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
        /// Applies the payload entries that name a system property whose type the base
        /// binder cannot convert a string into.
        /// </summary>
        /// <remarks>
        /// <see cref="RestApiCrudFormDataExtensions.BindTo"/> ends in
        /// <c>Convert.ChangeType</c>, which has no conversion from a string to a
        /// <see cref="Guid"/>, to a nullable value type or to an enum; those entries throw
        /// inside the binder and are swallowed, so the property keeps its old value and
        /// the caller is told the update succeeded. The inline cell editors of the object
        /// overview table send exactly such payloads — an assignee is an identity id, a
        /// story point count a nullable int — so the conversions the binder is missing are
        /// done here.
        /// </remarks>
        /// <param name="object">The object being updated.</param>
        /// <param name="payload">The incoming payload; keys arrive lower-cased.</param>
        private static void BindSystemProperties(Model.Entities.Object @object, RestApiCrudFormData payload)
        {
            if (@object is null || payload is null)
            {
                return;
            }

            if (payload.TryGetValue(nameof(Model.Entities.Object.AssigneeId).ToLowerInvariant(), out var assignee))
            {
                var raw = assignee?.ToString();

                @object.AssigneeId = Guid.TryParse(raw, out var assigneeId) ? assigneeId : null;
            }

            if (payload.TryGetValue(nameof(Model.Entities.Object.StoryPoints).ToLowerInvariant(), out var storyPoints))
            {
                var raw = storyPoints?.ToString();

                @object.StoryPoints = int.TryParse(raw, out var points) ? points : null;
            }
        }

        /// <summary>
        /// Binds the guid references of an object from the payload.
        /// </summary>
        /// <remarks>
        /// <c>BindTo</c> converts through <c>Convert.ChangeType</c>, which cannot produce a guid,
        /// so the class and workspace an object cannot exist without would silently stay empty and
        /// the insert would fail on the foreign key. They are therefore bound here, together with
        /// the optional parent reference.
        /// </remarks>
        /// <param name="fieldMap">The payload carrying the references.</param>
        /// <param name="object">The object to bind them to.</param>
        private static void BindReferences(RestApiCrudFormData fieldMap, Model.Entities.Object @object)
        {
            if (fieldMap.TryGetGuid(nameof(Model.Entities.Object.ClassId), out var classId))
            {
                @object.ClassId = classId;
            }

            if (fieldMap.TryGetGuid(nameof(Model.Entities.Object.WorkspaceId), out var workspaceId))
            {
                @object.WorkspaceId = workspaceId;
            }

            // an object created from a template inherits the class the template instantiates
            // when the payload names only the template
            if (@object.ClassId == Guid.Empty && fieldMap.TryGetGuid("TemplateId", out var templateId))
            {
                @object.ClassId = CoreHub.TemplateManager.GetTemplate(templateId)?.ClassId ?? Guid.Empty;
            }

            // an object created from a workspace overview inherits the workspace of its class when
            // the payload names only the class
            if (@object.WorkspaceId == Guid.Empty)
            {
                @object.WorkspaceId = CoreHub.ClassManager.GetClass(@object.ClassId)?.WorkspaceId ?? Guid.Empty;
            }

            if (fieldMap.TryGetGuidReference(nameof(Model.Entities.Object.ParentId), out var parentId))
            {
                @object.ParentId = parentId;
            }
        }

        /// <summary>
        /// Assigns the object a key when the payload carries none.
        /// </summary>
        /// <remarks>
        /// The key is the human-readable handle an object is addressed by (<c>SD-17</c>), so it
        /// has to exist before the record is written, and the create form does not ask for one.
        /// The next free number is derived from the keys already issued in the workspace. Two
        /// creates racing each other could derive the same number; the surrounding code is
        /// likewise single-writer, so the sequence is not guarded any further here.
        /// </remarks>
        /// <param name="object">The object to assign a key to.</param>
        private static void EnsureKey(Model.Entities.Object @object)
        {
            if (!string.IsNullOrWhiteSpace(@object.Key))
            {
                return;
            }

            var workspace = CoreHub.WorkspaceManager.GetWorkspace(@object.WorkspaceId);
            var prefix = workspace?.Key;

            if (string.IsNullOrWhiteSpace(prefix))
            {
                return;
            }

            var query = new Query<Model.Entities.Object>()
                .WhereEquals(x => x.WorkspaceId, @object.WorkspaceId);
            var pattern = new Regex($"^{Regex.Escape(prefix)}-(\\d+)$", RegexOptions.IgnoreCase);

            var next = CoreHub.ObjectManager.GetObjects(query)
                .Select(x => pattern.Match(x.Key ?? string.Empty))
                .Where(m => m.Success)
                .Select(m => int.TryParse(m.Groups[1].Value, out var number) ? number : 0)
                .DefaultIfEmpty(0)
                .Max() + 1;

            @object.Key = $"{prefix}-{next.ToString(CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// Applies the template the payload names to a freshly created object: its presets become
        /// field values, and each of its child templates becomes an object below the created one.
        /// </summary>
        /// <remarks>
        /// A value the caller submitted wins over the preset that would otherwise fill the same
        /// field — a template pre-fills a form, it does not overrule what the user typed into it.
        /// </remarks>
        /// <param name="object">The object that was created.</param>
        /// <param name="fieldMap">The payload, which may name a template.</param>
        /// <param name="request">The request, for resolving the acting identity.</param>
        private static void ApplyTemplate(Model.Entities.Object @object, RestApiCrudFormData fieldMap, IRequest request)
        {
            if (!fieldMap.TryGetGuid("TemplateId", out var templateId))
            {
                return;
            }

            var template = CoreHub.TemplateManager.GetTemplate(templateId);

            if (template is null || template.State != TemplateState.Active)
            {
                return;
            }

            ApplyPresets(@object, templateId, fieldMap);
            CreateChildren(@object, templateId, request, new HashSet<Guid> { templateId });
        }

        /// <summary>
        /// Writes the presets of a template as field values of an object, skipping the fields the
        /// payload already set.
        /// </summary>
        /// <param name="object">The object to write the values to.</param>
        /// <param name="templateId">The template whose presets are applied.</param>
        /// <param name="payload">The payload whose own values take precedence, or null.</param>
        private static void ApplyPresets(Model.Entities.Object @object, Guid templateId, RestApiCrudFormData payload)
        {
            var presets = new RestApiCrudFormData();

            foreach (var preset in CoreHub.TemplateManager.GetPresets(templateId))
            {
                var key = preset.Key.ToLowerInvariant();

                if (payload?.ContainsKey(key) == true)
                {
                    continue;
                }

                presets[key] = preset.Value;
            }

            UpsertFieldValues(@object, presets);
        }

        /// <summary>
        /// Creates one object per active child template below the supplied object, depth first and
        /// in the order the child templates define.
        /// </summary>
        /// <remarks>
        /// A child whose class the parent's class does not allow is skipped rather than created,
        /// so a composite template cannot build a hierarchy the object model would reject. The
        /// visited set carries the templates already instantiated along this branch, which keeps a
        /// cycle in the template hierarchy from creating objects without end.
        /// </remarks>
        /// <param name="parent">The object the created objects are placed below.</param>
        /// <param name="templateId">The template whose children are instantiated.</param>
        /// <param name="request">The request, for resolving the acting identity.</param>
        /// <param name="visited">The templates already instantiated along this branch.</param>
        private static void CreateChildren(Model.Entities.Object parent, Guid templateId, IRequest request, ISet<Guid> visited)
        {
            var parentClass = CoreHub.ClassManager.GetClass(parent.ClassId);
            var currentUser = CoreHub.SessionManager.GetCurrentIdentityId(request);

            foreach (var childTemplate in CoreHub.TemplateManager.GetChildTemplates(templateId))
            {
                if (!visited.Add(childTemplate.Id))
                {
                    continue;
                }

                if (parentClass?.AllowedChildren is { Count: > 0 }
                    && parentClass.AllowedChildren.All(c => c.Id != childTemplate.ClassId))
                {
                    continue;
                }

                var id = Guid.NewGuid();
                var child = new Model.Entities.Object(id)
                {
                    Summary = childTemplate.Name,
                    Description = childTemplate.Description,
                    Icon = childTemplate.Icon ?? CoreHub.GenerateIcon(id),
                    State = WorkspaceState.Active,
                    ClassId = childTemplate.ClassId,
                    WorkspaceId = parent.WorkspaceId,
                    ParentId = parent.Id,
                    CreatorId = currentUser,
                    UpdaterId = currentUser
                };

                EnsureKey(child);

                CoreHub.ObjectManager.Add(child);

                ApplyPresets(child, childTemplate.Id, null);

                CreateChildren(child, childTemplate.Id, request, visited);
            }
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
        /// <param name="object">The object whose field values are written.</param>
        /// <param name="payload">The payload carrying the values.</param>
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

                var raw = Normalize(SerializePayloadValue(kv.Value), field.FieldType);
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
        /// Brings a serialized payload into the canonical storage form of its field type.
        /// </summary>
        /// <remarks>
        /// Only tags need it. A tag list is stored comma-separated, which is what the
        /// object detail page writes and reads, but the tag input control of the table
        /// cells submits its tags semicolon-separated. Rewriting the separator here keeps
        /// one shape in the value row no matter which surface wrote it.
        /// </remarks>
        /// <param name="raw">The serialized payload.</param>
        /// <param name="fieldType">The type of the field being written.</param>
        /// <returns>The payload in storage form.</returns>
        private static string Normalize(string raw, FieldType fieldType)
        {
            if (fieldType != FieldType.Tag || string.IsNullOrEmpty(raw))
            {
                return raw;
            }

            return string.Join(",", raw
                .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
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
