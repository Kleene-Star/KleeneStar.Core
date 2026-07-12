using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing typed directional links between objects.
    /// </summary>
    public interface IObjectLinkManager : IComponentManager
    {
        /// <summary>
        /// Raised when a new link has been added.
        /// </summary>
        event EventHandler<ObjectLink> LinkAdded;

        /// <summary>
        /// Raised when an existing link has been removed.
        /// </summary>
        event EventHandler<ObjectLink> LinkRemoved;

        /// <summary>
        /// Returns every link in which the supplied object participates, regardless of
        /// whether it is the source or the target side.
        /// </summary>
        /// <param name="objectId">The id of the object.</param>
        IEnumerable<ObjectLink> GetLinks(Guid objectId);

        /// <summary>
        /// Adds the supplied link.
        /// </summary>
        /// <param name="link">The link to add. Cannot be null.</param>
        IObjectLinkManager Add(ObjectLink link);

        /// <summary>
        /// Removes the supplied link.
        /// </summary>
        /// <param name="link">The link to remove. Cannot be null.</param>
        IObjectLinkManager Remove(ObjectLink link);
    }
}
