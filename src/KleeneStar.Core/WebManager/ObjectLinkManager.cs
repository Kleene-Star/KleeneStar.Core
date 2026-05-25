using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Manages typed directional links between objects.
    /// </summary>
    public sealed class ObjectLinkManager : IObjectLinkManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// Raised when a new link has been added.
        /// </summary>
        public event EventHandler<ObjectLink> LinkAdded;

        /// <summary>
        /// Raised when an existing link has been removed.
        /// </summary>
        public event EventHandler<ObjectLink> LinkRemoved;

        /// <summary>
        /// Initializes a new instance of the manager. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The HTTP server context.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via reflection.")]
        private ObjectLinkManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns every link in which the supplied object participates, regardless of
        /// whether it is the source or the target side.
        /// </summary>
        /// <param name="objectId">The id of the object.</param>
        public IEnumerable<ObjectLink> GetLinks(Guid objectId)
        {
            var query = new Query<ObjectLink>()
                .Where(x => x.SourceObjectId == objectId || x.TargetObjectId == objectId);

            return ModelHub.GetObjectLinks(query);
        }

        /// <summary>
        /// Adds the supplied link.
        /// </summary>
        public IObjectLinkManager Add(ObjectLink link)
        {
            ArgumentNullException.ThrowIfNull(link);

            ModelHub.Add(link);

            LinkAdded?.Invoke(this, link);

            return this;
        }

        /// <summary>
        /// Removes the supplied link.
        /// </summary>
        public IObjectLinkManager Remove(ObjectLink link)
        {
            ArgumentNullException.ThrowIfNull(link);

            ModelHub.Remove(link);

            LinkRemoved?.Invoke(this, link);

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
