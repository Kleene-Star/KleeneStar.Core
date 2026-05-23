using KleeneStar.Core.WebParameter;
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
    /// Manages the lifecycle of <see cref="SlaPolicy"/> entities and their dependent targets,
    /// scope rules, and escalation levels.
    /// </summary>
    public sealed class SlaManager : ISlaManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <inheritdoc/>
        public event EventHandler<SlaPolicy> SlaAdded;

        /// <inheritdoc/>
        public event EventHandler<SlaPolicy> SlaUpdated;

        /// <inheritdoc/>
        public event EventHandler<SlaPolicy> SlaRemoved;

        /// <summary>
        /// Returns the path-segment names reserved by the SLA router and unavailable as
        /// custom policy ids.
        /// </summary>
        public static IEnumerable<string> ReservedSlaNames =>
        [
            "default", "admin", "system", "assets", "api", "add", "edit",
            "delete", "clone", "settings", "icons"
        ];

        /// <summary>
        /// Initializes a new instance of the manager. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The HTTP server context.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via reflection.")]
        private SlaManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <inheritdoc/>
        public SlaPolicy GetSla(Guid slaId)
        {
            var query = new Query<SlaPolicy>()
                .Where(x => x.Id == slaId)
                .WithPaging(0, 1);

            return ModelHub.GetSlaPolicies(query).FirstOrDefault();
        }

        /// <inheritdoc/>
        public SlaPolicy GetSla(SlaIdParameter slaId)
        {
            ArgumentNullException.ThrowIfNull(slaId);

            var guid = Guid.TryParse(slaId.Value, out var id) ? id : Guid.Empty;

            return GetSla(guid);
        }

        /// <inheritdoc/>
        public IEnumerable<SlaPolicy> GetSlas(ClassIdParameter classId)
        {
            ArgumentNullException.ThrowIfNull(classId);

            var guid = Guid.TryParse(classId.Value, out var id) ? id : Guid.Empty;

            return GetSlas(guid);
        }

        /// <inheritdoc/>
        public IEnumerable<SlaPolicy> GetSlas(Guid classId)
        {
            var query = new Query<SlaPolicy>()
                .WhereEquals(x => x.ClassId, classId);

            return ModelHub.GetSlaPolicies(query);
        }

        /// <inheritdoc/>
        public IEnumerable<SlaPolicy> GetSlas(IQuery<SlaPolicy> query)
        {
            return ModelHub.GetSlaPolicies(query);
        }

        /// <inheritdoc/>
        public IEnumerable<SlaPolicy> GetSlas(IQuery<SlaPolicy> query, IQueryContext context)
        {
            return ModelHub.GetSlaPolicies(query, context as KleeneStarDbContext);
        }

        /// <inheritdoc/>
        public ISlaManager Add(SlaPolicy policy)
        {
            ArgumentNullException.ThrowIfNull(policy);

            ModelHub.Add(policy);

            SlaAdded?.Invoke(this, policy);

            TryAddNotification("Create");

            return this;
        }

        /// <inheritdoc/>
        public ISlaManager Update(SlaPolicy policy)
        {
            ArgumentNullException.ThrowIfNull(policy);

            ModelHub.Update(policy);

            SlaUpdated?.Invoke(this, policy);

            TryAddNotification("Update");

            return this;
        }

        // Wraps CoreHub.AddNotification so that callers running outside a fully wired
        // WebExpress host (in particular unit tests using the in-memory fixture) do not
        // crash on the unavailable global ComponentHub.
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

        /// <inheritdoc/>
        public ISlaManager Remove(Guid slaId)
        {
            var existing = GetSla(slaId);

            if (existing is not null)
            {
                ModelHub.Remove(existing);
                SlaRemoved?.Invoke(this, existing);
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
    }
}
