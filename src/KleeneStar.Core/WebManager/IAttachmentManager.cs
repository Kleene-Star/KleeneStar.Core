using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing files attached to objects.
    /// </summary>
    public interface IAttachmentManager : IComponentManager
    {
        /// <summary>
        /// Raised when an attachment has been added to an object.
        /// </summary>
        event EventHandler<Attachment> AttachmentAdded;

        /// <summary>
        /// Raised when an attachment has been removed from an object.
        /// </summary>
        event EventHandler<Attachment> AttachmentRemoved;

        /// <summary>
        /// Returns every visible attachment of the object addressed by the supplied
        /// URL-bound object-key parameter, in chronological order (oldest first).
        /// </summary>
        /// <param name="objectKey">The object-key parameter parsed from the URL path.</param>
        /// <returns>The attachments of the object. The collection may be empty.</returns>
        IEnumerable<Attachment> GetAttachments(ObjectKeyParameter objectKey);

        /// <summary>
        /// Returns every visible attachment of the object with the supplied id, in
        /// chronological order (oldest first). The binary payload is not loaded.
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The attachments of the object. The collection may be empty.</returns>
        IEnumerable<Attachment> GetAttachments(Guid objectId);

        /// <summary>
        /// Returns the single attachment with the supplied id including its binary
        /// <see cref="Attachment.Content"/>, or <see langword="null"/> when not found.
        /// Used by the download endpoint.
        /// </summary>
        /// <param name="attachmentId">The attachment id.</param>
        /// <returns>The attachment with its payload, or <see langword="null"/>.</returns>
        Attachment GetAttachment(Guid attachmentId);

        /// <summary>
        /// Returns the attachments that satisfy the supplied query.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching attachments.</returns>
        IEnumerable<Attachment> GetAttachments(IQuery<Attachment> query);

        /// <summary>
        /// Returns the attachments that satisfy the supplied query, executed inside the
        /// supplied <see cref="IQueryContext"/>.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The matching attachments.</returns>
        IEnumerable<Attachment> GetAttachments(IQuery<Attachment> query, IQueryContext context);

        /// <summary>
        /// Attaches a file to the object. Returns <see langword="null"/> when the object
        /// does not exist or the file name is empty.
        /// </summary>
        /// <param name="objectId">The id of the object the file is attached to.</param>
        /// <param name="fileName">The original file name including its extension.</param>
        /// <param name="contentType">The MIME content type of the file.</param>
        /// <param name="content">The binary payload of the file. The size is derived from it.</param>
        /// <param name="description">An optional human-readable description, or <c>null</c>.</param>
        /// <param name="uploaderId">The id of the uploading identity, or <c>null</c>.</param>
        /// <returns>The persisted attachment, or <see langword="null"/>.</returns>
        Attachment Add(Guid objectId, string fileName, string contentType, byte[] content, string description, Guid? uploaderId);

        /// <summary>
        /// Removes the attachment with the supplied id from its object.
        /// </summary>
        /// <param name="attachmentId">The id of the attachment to remove.</param>
        /// <returns><see langword="true"/> when a row existed and was removed.</returns>
        bool Remove(Guid attachmentId);
    }
}
