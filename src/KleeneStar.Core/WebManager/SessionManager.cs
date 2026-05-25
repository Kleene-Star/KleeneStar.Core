using KleeneStar.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Manages per-identity session/preference entries stored in the
    /// <c>UserSession</c> table. The manager is intentionally schema-less:
    /// callers pick a <c>scope</c> plus a <c>key</c> and supply an opaque
    /// payload that the producer/consumer pair knows how to interpret. The
    /// first concrete use case is REST API table layouts, for which a few
    /// typed convenience methods are provided.
    /// </summary>
    public sealed class SessionManager : ISessionManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// Identity used to own session entries when the request does not yet
        /// carry an authenticated user (mirrors the comment endpoint's fallback
        /// to the seeded admin identity).
        /// </summary>
        private static readonly Guid FallbackOwnerId = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD");

        /// <summary>
        /// Shared JSON serializer options for round-tripping the opaque payloads
        /// stored under (owner, scope, key). Null fields are omitted on write and
        /// property-name matching is case-insensitive on read.
        /// </summary>
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Initializes a new instance of the class. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The host context.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private SessionManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Resolves the identity that owns the current request. WebExpress
        /// does not yet expose the authenticated identity on <see cref="IRequest"/>;
        /// until it does, every request is attributed to the seeded admin
        /// identity so that user preferences still round-trip to a valid row.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <returns>The current identity id.</returns>
        public Guid GetCurrentIdentityId(IRequest request)
        {
            // TODO: read the authenticated identity from request.Session once the
            // WebExpress identity flow exposes it on the request. Until then,
            // anonymous requests are attributed to the seeded admin identity.
            return FallbackOwnerId;
        }

        /// <summary>
        /// Returns the value stored under (owner, scope, key), or
        /// <see langword="null"/> if no entry exists.
        /// </summary>
        /// <param name="ownerId">The identity that owns the entry.</param>
        /// <param name="scope">The scope namespace.</param>
        /// <param name="key">The key inside the scope.</param>
        /// <returns>The stored value, or <see langword="null"/>.</returns>
        public string GetValue(Guid ownerId, string scope, string key)
        {
            return ModelHub.GetUserSessionValue(ownerId, scope, key);
        }

        /// <summary>
        /// Inserts or updates the value stored under (owner, scope, key).
        /// Passing <see langword="null"/> as <paramref name="value"/> deletes the entry.
        /// </summary>
        /// <param name="ownerId">The identity that owns the entry.</param>
        /// <param name="scope">The scope namespace.</param>
        /// <param name="key">The key inside the scope.</param>
        /// <param name="value">The new value, or <see langword="null"/> to delete.</param>
        public void SetValue(Guid ownerId, string scope, string key, string value)
        {
            ModelHub.SetUserSessionValue(ownerId, scope, key, value);
        }

        /// <summary>
        /// Convenience wrapper that resolves the current identity from the
        /// request and reads the value stored under (current owner, scope, key).
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <param name="scope">The scope namespace.</param>
        /// <param name="key">The key inside the scope.</param>
        /// <returns>The stored value, or <see langword="null"/>.</returns>
        public string GetValue(IRequest request, string scope, string key)
        {
            var owner = GetCurrentIdentityId(request);
            return owner == Guid.Empty ? null : GetValue(owner, scope, key);
        }

        /// <summary>
        /// Convenience wrapper that resolves the current identity from the
        /// request and writes the value stored under (current owner, scope, key).
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <param name="scope">The scope namespace.</param>
        /// <param name="key">The key inside the scope.</param>
        /// <param name="value">The new value, or <see langword="null"/> to delete.</param>
        public void SetValue(IRequest request, string scope, string key, string value)
        {
            var owner = GetCurrentIdentityId(request);
            if (owner == Guid.Empty)
            {
                return;
            }

            SetValue(owner, scope, key, value);
        }

        /// <summary>
        /// Loads the persisted column layout for the REST API table identified
        /// by <paramref name="tableKey"/> belonging to the current request's
        /// identity, or <see langword="null"/> if nothing has been stored yet.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <param name="tableKey">
        /// A stable identifier for the table, typically <c>typeof(MyTable).FullName</c>.
        /// </param>
        /// <returns>
        /// The previously stored column layout (id, visibility, width — in order),
        /// or <see langword="null"/> when the user has never customized the table.
        /// </returns>
        public IReadOnlyList<RestApiTableColumnUpdate> GetTableLayout(IRequest request, string tableKey)
        {
            if (string.IsNullOrWhiteSpace(tableKey))
            {
                return null;
            }

            var json = GetValue(request, ISessionManager.TableLayoutScope, tableKey);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                var stored = JsonSerializer.Deserialize<List<RestApiTableColumnUpdate>>(json, _jsonOptions);
                return stored?.Where(c => !string.IsNullOrWhiteSpace(c?.Id)).ToList();
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// Stores the column layout for the REST API table identified by
        /// <paramref name="tableKey"/> against the current request's identity.
        /// Only id / visibility / width are persisted — labels, icons, and
        /// templates remain owned by the REST API table itself.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <param name="tableKey">
        /// A stable identifier for the table, typically <c>typeof(MyTable).FullName</c>.
        /// </param>
        /// <param name="columns">
        /// The columns in the order chosen by the user; visibility and width
        /// are taken from each column.
        /// </param>
        public void SetTableLayout(IRequest request, string tableKey, IEnumerable<RestApiTableColumn> columns)
        {
            if (string.IsNullOrWhiteSpace(tableKey))
            {
                return;
            }

            if (columns is null)
            {
                SetValue(request, ISessionManager.TableLayoutScope, tableKey, null);
                return;
            }

            var snapshot = columns
                .Where(c => !string.IsNullOrWhiteSpace(c?.Id))
                .Select(c => new RestApiTableColumnUpdate
                {
                    Id = c.Id,
                    Visible = c.Visible,
                    Width = c.Width
                })
                .ToList();

            var json = JsonSerializer.Serialize(snapshot, _jsonOptions);
            SetValue(request, ISessionManager.TableLayoutScope, tableKey, json);
        }

        /// <summary>
        /// Applies the stored layout for <paramref name="tableKey"/> on top of the
        /// table's default column list. Columns not mentioned in the stored
        /// layout are appended at the tail with their default visibility.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <param name="tableKey">A stable identifier for the table.</param>
        /// <param name="defaultColumns">The default columns defined by the table.</param>
        /// <returns>The columns reordered/resized for the current user.</returns>
        public IEnumerable<RestApiTableColumn> ApplyStoredTableLayout
        (
            IRequest request,
            string tableKey,
            IEnumerable<RestApiTableColumn> defaultColumns
        )
        {
            if (defaultColumns is null)
            {
                yield break;
            }

            var defaults = defaultColumns.ToList();
            var stored = GetTableLayout(request, tableKey);

            if (stored is null || stored.Count == 0)
            {
                foreach (var column in defaults)
                {
                    yield return column;
                }

                yield break;
            }

            var lookup = defaults.ToDictionary(c => c.Id, StringComparer.OrdinalIgnoreCase);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // emit columns in the stored order, copying visibility/width
            foreach (var update in stored)
            {
                if (string.IsNullOrWhiteSpace(update.Id) ||
                    !lookup.TryGetValue(update.Id, out var template) ||
                    !seen.Add(template.Id))
                {
                    continue;
                }

                yield return new RestApiTableColumn
                {
                    Id = template.Id,
                    Name = template.Name,
                    Label = template.Label,
                    Icon = template.Icon,
                    Template = template.Template,
                    Visible = update.Visible ?? template.Visible,
                    Width = update.Width ?? template.Width
                };
            }

            // append any column the stored layout does not know about (e.g. a
            // column added in a newer build) at the tail with its default state
            foreach (var column in defaults)
            {
                if (seen.Contains(column.Id))
                {
                    continue;
                }

                yield return column;
            }
        }

        /// <summary>
        /// Releases resources held by this manager.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
