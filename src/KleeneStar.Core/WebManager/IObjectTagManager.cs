using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing tags (labels) attached to objects.
    /// </summary>
    public interface IObjectTagManager : IComponentManager
    {
        /// <summary>
        /// Raised when a tag has been attached to an object.
        /// </summary>
        event EventHandler<ObjectTag> TagAdded;

        /// <summary>
        /// Raised when a tag has been detached from an object.
        /// </summary>
        event EventHandler<ObjectTag> TagRemoved;

        /// <summary>
        /// Returns every tag attached to the supplied object (parameter form), in
        /// chronological order (oldest first).
        /// </summary>
        /// <param name="objectKey">The object-key parameter parsed from the URL path.</param>
        /// <returns>The tags attached to the object. The collection may be empty.</returns>
        IEnumerable<ObjectTag> GetTags(ObjectKeyParameter objectKey);

        /// <summary>
        /// Returns every tag attached to the object with the supplied id, in chronological
        /// order (oldest first).
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The tags attached to the object. The collection may be empty.</returns>
        IEnumerable<ObjectTag> GetTags(Guid objectId);

        /// <summary>
        /// Returns the tags that satisfy the supplied query.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching tags.</returns>
        IEnumerable<ObjectTag> GetTags(IQuery<ObjectTag> query);

        /// <summary>
        /// Returns the tags that satisfy the supplied query, executed inside the supplied
        /// <see cref="IQueryContext"/>.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The matching tags.</returns>
        IEnumerable<ObjectTag> GetTags(IQuery<ObjectTag> query, IQueryContext context);

        /// <summary>
        /// Attaches a tag with the supplied name and optional color to the object. When a tag
        /// of that name already exists on the object, the existing row is returned and no
        /// change is made.
        /// </summary>
        /// <param name="objectId">The id of the object being tagged.</param>
        /// <param name="name">The tag display text.</param>
        /// <param name="color">The optional CSS color of the tag badge, or <c>null</c>.</param>
        /// <returns>The persisted tag, or <see langword="null"/> when the object does not
        /// exist or the name is empty.</returns>
        ObjectTag Add(Guid objectId, string name, string color);

        /// <summary>
        /// Detaches the tag with the supplied id from its object.
        /// </summary>
        /// <param name="tagId">The id of the tag to remove.</param>
        /// <returns><see langword="true"/> when a row existed and was removed.</returns>
        bool Remove(Guid tagId);
    }
}
