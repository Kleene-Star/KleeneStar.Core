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
    /// Default implementation of <see cref="IObjectViewManager"/>. Discovered and constructed
    /// by the WebExpress component manager via reflection.
    /// </summary>
    public sealed class ObjectViewManager : IObjectViewManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <inheritdoc />
        public event EventHandler<ObjectView> ObjectViewAdded;

        /// <inheritdoc />
        public event EventHandler<ObjectView> ObjectViewUpdated;

        /// <inheritdoc />
        public event EventHandler<ObjectView> ObjectViewRemoved;

        /// <summary>
        /// Initializes a new instance via reflection.
        /// </summary>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private ObjectViewManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <inheritdoc />
        public ObjectView GetObjectView(Guid viewId)
        {
            var query = new Query<ObjectView>()
                .Where(x => x.Id == viewId)
                .WithPaging(0, 1);

            return ModelHub.GetObjectViews(query)
                .FirstOrDefault();
        }

        /// <inheritdoc />
        public IEnumerable<ObjectView> GetObjectViews(IQuery<ObjectView> query)
        {
            return ModelHub.GetObjectViews(query);
        }

        /// <inheritdoc />
        public IEnumerable<ObjectView> GetObjectViews(IQuery<ObjectView> query, IQueryContext context)
        {
            if (context is KleeneStarDbContext db)
            {
                return ModelHub.GetObjectViews(query, db);
            }

            return [];
        }

        /// <inheritdoc />
        public IEnumerable<ObjectView> GetViewsForWorkspace(Guid workspaceId)
        {
            var query = new Query<ObjectView>()
                .WhereEquals(x => x.WorkspaceId, workspaceId)
                .OrderByAsc(x => x.Order);

            return ModelHub.GetObjectViews(query);
        }

        /// <inheritdoc />
        public IObjectViewManager AddObjectView(ObjectView viewEntry)
        {
            ModelHub.Add(viewEntry);
            ObjectViewAdded?.Invoke(this, viewEntry);
            return this;
        }

        /// <inheritdoc />
        public IObjectViewManager UpdateObjectView(ObjectView viewEntry)
        {
            ModelHub.Update(viewEntry);
            ObjectViewUpdated?.Invoke(this, viewEntry);
            return this;
        }

        /// <inheritdoc />
        public IObjectViewManager RemoveObjectView(ObjectView viewEntry)
        {
            ModelHub.Remove(viewEntry);
            ObjectViewRemoved?.Invoke(this, viewEntry);
            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }
    }
}
