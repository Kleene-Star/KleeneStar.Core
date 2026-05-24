using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Manages the lifecycle of <see cref="Value"/> entities — the per-object per-field
    /// payloads that back the typed inputs in the object detail and edit views. The
    /// manager intentionally returns the persisted <see cref="Value.Data"/> string as
    /// is; type-specific marshalling (parsing dates, splitting tag lists, etc.) is the
    /// responsibility of the consumer because it depends on the
    /// <see cref="Field.FieldType"/> the value is bound to.
    /// </summary>
    public sealed class ValueManager : IValueManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// Raised after a value has been added via <see cref="Add(Value)"/>.
        /// </summary>
        public event EventHandler<Value> ValueAdded;

        /// <summary>
        /// Raised after a value's payload has been updated via <see cref="Update(Value)"/>.
        /// </summary>
        public event EventHandler<Value> ValueUpdated;

        /// <summary>
        /// Raised after a value has been removed via <see cref="Remove(Guid)"/>.
        /// </summary>
        public event EventHandler<Value> ValueRemoved;

        /// <summary>
        /// Initializes a new instance of the manager. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The HTTP server context.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via reflection.")]
        private ValueManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns the value identified by the supplied id.
        /// </summary>
        /// <param name="valueId">The value id.</param>
        /// <returns>The value, or <c>null</c> when no entry matches.</returns>
        public Value GetValue(Guid valueId)
        {
            var query = new Query<Value>()
                .Where(x => x.Id == valueId)
                .WithPaging(0, 1);

            return ModelHub.GetValues(query).FirstOrDefault();
        }

        /// <summary>
        /// Returns the value attached to the supplied (object, field) pair.
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <param name="fieldId">The field id.</param>
        /// <returns>The value, or <c>null</c> when no entry matches.</returns>
        public Value GetValue(Guid objectId, Guid fieldId)
        {
            var query = new Query<Value>()
                .Where(x => x.ObjectId == objectId && x.FieldId == fieldId)
                .WithPaging(0, 1);

            return ModelHub.GetValues(query).FirstOrDefault();
        }

        /// <summary>
        /// Returns every value attached to the object with the supplied id.
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The values attached to the object. The collection may be empty.</returns>
        public IEnumerable<Value> GetValues(Guid objectId)
        {
            var query = new Query<Value>()
                .WhereEquals(x => x.ObjectId, objectId);

            return ModelHub.GetValues(query).ToList();
        }

        /// <summary>
        /// Returns the values that satisfy the supplied query.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching values.</returns>
        public IEnumerable<Value> GetValues(IQuery<Value> query)
        {
            return ModelHub.GetValues(query);
        }

        /// <summary>
        /// Returns the values that satisfy the supplied query, executed inside the
        /// supplied <see cref="IQueryContext"/> (expected to be a
        /// <see cref="KleeneStarDbContext"/>).
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The matching values.</returns>
        public IEnumerable<Value> GetValues(IQuery<Value> query, IQueryContext context)
        {
            return ModelHub.GetValues(query, context as KleeneStarDbContext);
        }

        /// <summary>
        /// Adds the supplied value to the database, raises <see cref="ValueAdded"/>,
        /// and emits a UI notification. Returns the manager instance to allow chaining.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>The current manager instance.</returns>
        public IValueManager Add(Value value)
        {
            ArgumentNullException.ThrowIfNull(value);

            ModelHub.Add(value);
            ValueAdded?.Invoke(this, value);
            TryAddNotification("Create");

            return this;
        }

        /// <summary>
        /// Persists the supplied value's payload change. Raises <see cref="ValueUpdated"/>.
        /// </summary>
        /// <param name="value">The value to update.</param>
        /// <returns>The current manager instance.</returns>
        public IValueManager Update(Value value)
        {
            ArgumentNullException.ThrowIfNull(value);

            ModelHub.Update(value);
            ValueUpdated?.Invoke(this, value);
            TryAddNotification("Update");

            return this;
        }

        /// <summary>
        /// Removes the value identified by the supplied id from the data store. Raises
        /// <see cref="ValueRemoved"/>. No-op when the value does not exist.
        /// </summary>
        /// <param name="valueId">The value id.</param>
        /// <returns>The current manager instance.</returns>
        public IValueManager Remove(Guid valueId)
        {
            var existing = GetValue(valueId);

            if (existing is not null)
            {
                ModelHub.Remove(existing);
                ValueRemoved?.Invoke(this, existing);
            }

            return this;
        }

        /// <summary>
        /// Releases unmanaged resources reserved during use.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Emits a UI notification via <see cref="CoreHub.AddNotification"/>, swallowing
        /// any exception so that tests with a partially wired host don't crash.
        /// </summary>
        /// <param name="header">The notification header.</param>
        private static void TryAddNotification(string header)
        {
            try
            {
                CoreHub.AddNotification(header, "success", 5000);
            }
            catch
            {
                // notification is best-effort; ignore failures from incomplete host state
            }
        }
    }
}
