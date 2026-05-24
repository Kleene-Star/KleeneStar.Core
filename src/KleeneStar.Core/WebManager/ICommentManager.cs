using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing <see cref="Comment"/> entities attached to
    /// <see cref="Object"/>s.
    /// </summary>
    public interface ICommentManager : IComponentManager
    {
        /// <summary>
        /// Raised when a new comment has been added.
        /// </summary>
        event EventHandler<Comment> CommentAdded;

        /// <summary>
        /// Raised when a comment's content or state has been updated.
        /// </summary>
        event EventHandler<Comment> CommentUpdated;

        /// <summary>
        /// Raised when a comment has been removed (hard delete).
        /// </summary>
        event EventHandler<Comment> CommentRemoved;

        /// <summary>
        /// Returns the comment identified by the supplied id.
        /// </summary>
        /// <param name="commentId">The comment id.</param>
        /// <returns>The comment, or <c>null</c> when no entry matches.</returns>
        Comment GetComment(Guid commentId);

        /// <summary>
        /// Returns the comment identified by the supplied URL-bound id parameter.
        /// </summary>
        /// <param name="commentId">The id parameter parsed from the URL path.</param>
        /// <returns>The comment, or <c>null</c> when no entry matches.</returns>
        Comment GetComment(CommentIdParameter commentId);

        /// <summary>
        /// Returns every comment attached to the supplied object (parameter form), in
        /// chronological order (oldest first).
        /// </summary>
        /// <param name="objectKey">The object-key parameter parsed from the URL path.</param>
        /// <returns>The comments attached to the object. The collection may be empty.</returns>
        IEnumerable<Comment> GetComments(ObjectKeyParameter objectKey);

        /// <summary>
        /// Returns every comment attached to the object with the supplied id, in
        /// chronological order (oldest first).
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The comments attached to the object. The collection may be empty.</returns>
        IEnumerable<Comment> GetComments(Guid objectId);

        /// <summary>
        /// Returns the comments that satisfy the supplied query.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching comments.</returns>
        IEnumerable<Comment> GetComments(IQuery<Comment> query);

        /// <summary>
        /// Returns the comments that satisfy the supplied query, executed inside the
        /// supplied <see cref="IQueryContext"/>.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The matching comments.</returns>
        IEnumerable<Comment> GetComments(IQuery<Comment> query, IQueryContext context);

        /// <summary>
        /// Adds a comment to the manager.
        /// </summary>
        /// <param name="comment">The comment to add.</param>
        /// <returns>The current instance to allow chaining.</returns>
        ICommentManager Add(Comment comment);

        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="comment">The comment to update.</param>
        /// <returns>The current instance to allow chaining.</returns>
        ICommentManager Update(Comment comment);

        /// <summary>
        /// Soft-deletes the comment identified by the supplied id (sets
        /// <see cref="Comment.State"/> to <see cref="CommentState.Deleted"/> and
        /// populates <see cref="Comment.DeletedAt"/>). Use this in preference to the
        /// hard <see cref="Remove(Guid)"/> path so that any replies remain navigable.
        /// </summary>
        /// <param name="commentId">The id of the comment to soft-delete.</param>
        /// <returns>The current instance to allow chaining.</returns>
        ICommentManager SoftDelete(Guid commentId);

        /// <summary>
        /// Hard-removes the comment identified by the supplied id from the data store.
        /// Use <see cref="SoftDelete(Guid)"/> instead when the comment has replies.
        /// </summary>
        /// <param name="commentId">The id of the comment to remove.</param>
        /// <returns>The current instance to allow chaining.</returns>
        ICommentManager Remove(Guid commentId);

        /// <summary>
        /// Toggles the <see cref="Comment.IsPinned"/> flag of the comment with the
        /// supplied id.
        /// </summary>
        /// <param name="commentId">The id of the comment to pin / unpin.</param>
        /// <returns>The new pin state (<c>true</c> = pinned), or <c>null</c> when no
        /// comment matches.</returns>
        bool? TogglePin(Guid commentId);

        /// <summary>
        /// Toggles a like on the comment for the supplied identity. Returns the new set
        /// of identity names that have liked the comment, in chronological order of
        /// like-creation.
        /// </summary>
        /// <param name="commentId">The comment id.</param>
        /// <param name="authorId">The identity authoring the like toggle.</param>
        /// <returns>The names of every identity that currently likes the comment, or
        /// an empty enumeration when the comment does not exist.</returns>
        IEnumerable<string> ToggleLike(Guid commentId, Guid authorId);

        /// <summary>
        /// Toggles an emoji reaction on the comment for the supplied identity. Returns
        /// the full reaction map (emoji → identity names) after the toggle.
        /// </summary>
        /// <param name="commentId">The comment id.</param>
        /// <param name="authorId">The identity authoring the reaction toggle.</param>
        /// <param name="emoji">The emoji to toggle.</param>
        /// <returns>The current reaction map. Empty when the comment does not exist.</returns>
        IDictionary<string, IEnumerable<string>> ToggleReaction(Guid commentId, Guid authorId, string emoji);
    }
}
