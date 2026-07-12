using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Comments._objectkey_
{
    /// <summary>
    /// REST endpoint backing the <c>ControlDataComment</c> and
    /// <c>ControlDataCommentComposer</c> controls on an object detail page. The URL is
    /// <c>/api/1/comments/{objectkey}</c>; the <c>{objectkey}</c> URL segment is
    /// declared via <see cref="ObjectKeySegmentAttribute"/> so callers can bind the
    /// segment by passing the current request's <see cref="ObjectKeyParameter"/>
    /// through <see cref="WebExpress.WebCore.WebUri.IUriExtensions.BindParameters"/>.
    /// </summary>
    /// <remarks>
    /// All persistence is delegated to <see cref="CoreHub.CommentManager"/>. Deletion
    /// uses the soft-delete path so reply threads remain navigable. Pin / Like / Reaction
    /// are persisted via the dedicated <see cref="CommentLike"/> / <see cref="CommentReaction"/>
    /// entities and the <see cref="Comment.IsPinned"/> column.
    /// <para>
    /// <see cref="IncludeSubPathsAttribute"/> is REQUIRED so that the comment control's
    /// sub-routes (<c>/likes</c>, <c>/pin</c>, <c>/reactions</c>, <c>/replies</c>) are
    /// dispatched to this endpoint's <c>ToggleLike</c> / <c>TogglePin</c> /
    /// <c>ToggleReaction</c> / <c>AppendReply</c> overrides — without it the sub-paths
    /// 404 and the control silently degrades.
    /// </para>
    /// </remarks>
    [Title("kleenestar.core:comment.api.title")]
    [ObjectKeySegment]
    [IncludeSubPaths(true)]
    [Cache]
    public sealed class Index : RestApiComment<Comment>
    {
        /// <summary>
        /// Fallback identity used as the author when the request session does not
        /// carry an authenticated user (e.g. during seeding or anonymous test traffic).
        /// Matches the admin identity seeded by <see cref="KleeneStarDbSeeder"/>.
        /// </summary>
        private static readonly Guid FallbackAuthorId = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD");

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Returns the comments attached to the object addressed by the URL
        /// <c>{objectkey}</c> segment. Top-level comments are returned in chronological
        /// order; their replies are nested in <see cref="RestApiCommentItem.Replies"/>.
        /// Soft-deleted comments are returned with a redacted body so that the UI can
        /// still render the placeholder ("[deleted]") while the thread structure stays
        /// intact.
        /// </summary>
        /// <param name="query">The query criteria supplied by the control.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The materialized list of comment items.</returns>
        protected override IEnumerable<RestApiCommentItem> RetrieveComments(IQuery<Comment> query, IQueryContext context, IRequest request)
        {
            var objectId = ResolveObjectId(request);
            if (objectId == Guid.Empty)
            {
                return [];
            }

            using var db = ModelHub.CreateDbContext();
            var all = db.Comments
                .Include(c => c.Author)
                .Include(c => c.Likes).ThenInclude(l => l.Author)
                .Include(c => c.Reactions).ThenInclude(r => r.Author)
                .AsNoTracking()
                .Where(c => c.ObjectId == objectId)
                .ToList();

            var byParent = all.Where(c => c.ParentCommentId.HasValue)
                              .GroupBy(c => c.ParentCommentId.Value)
                              .ToDictionary(g => g.Key, g => g.OrderBy(c => c.Created).ToList());

            return all
                .Where(c => !c.ParentCommentId.HasValue)
                .OrderByDescending(c => c.IsPinned)
                .ThenBy(c => c.Created)
                .Select(c => ToItem(c, byParent))
                .ToList();
        }

        /// <summary>
        /// Creates a new top-level comment on the addressed object. The author is the
        /// session's authenticated identity, or <see cref="FallbackAuthorId"/> when no
        /// authenticated identity is present.
        /// </summary>
        /// <param name="payload">The submitted body / category / labels.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The created comment, mapped to the REST DTO.</returns>
        protected override RestApiCommentItem CreateComment(RestApiCommentPayload payload, IQueryContext context, IRequest request)
        {
            ArgumentNullException.ThrowIfNull(payload);

            var objectId = ResolveObjectId(request);
            if (objectId == Guid.Empty || string.IsNullOrWhiteSpace(payload.Body))
            {
                return null;
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                ObjectId = objectId,
                AuthorId = ResolveAuthorId(request),
                Content = payload.Body.Trim(),
                State = CommentState.Active,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            CoreHub.CommentManager.Add(comment);

            return ToItem(LoadWithAuthor(comment.Id), parentReplies: null);
        }

        /// <summary>
        /// Updates the body of the comment identified by <paramref name="commentId"/>.
        /// The state is automatically bumped to <see cref="CommentState.Edited"/> by
        /// the <see cref="CommentManager.Update"/> path.
        /// </summary>
        /// <param name="commentId">The id of the comment to update.</param>
        /// <param name="payload">The new body content.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The updated comment mapped to the REST DTO, or <c>null</c> when the
        /// comment does not exist or the body is empty.</returns>
        protected override RestApiCommentItem UpdateComment(string commentId, RestApiCommentPayload payload, IQueryContext context, IRequest request)
        {
            if (!Guid.TryParse(commentId, out var id) || payload is null || string.IsNullOrWhiteSpace(payload.Body))
            {
                return null;
            }

            var existing = CoreHub.CommentManager.GetComment(id);
            if (existing is null)
            {
                return null;
            }

            existing.Content = payload.Body.Trim();
            existing.State = CommentState.Edited;
            existing.Updated = DateTime.UtcNow;

            CoreHub.CommentManager.Update(existing);

            return ToItem(LoadWithAuthor(id), parentReplies: null);
        }

        /// <summary>
        /// Soft-deletes the comment identified by <paramref name="commentId"/> via
        /// <see cref="CommentManager.SoftDelete"/>. Returns <c>false</c> when the
        /// supplied id is malformed or unknown.
        /// </summary>
        /// <param name="commentId">The id of the comment to delete.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns><c>true</c> on success; <c>false</c> otherwise.</returns>
        protected override bool DeleteComment(string commentId, IQueryContext context, IRequest request)
        {
            if (!Guid.TryParse(commentId, out var id))
            {
                return false;
            }

            var existing = CoreHub.CommentManager.GetComment(id);
            if (existing is null)
            {
                return false;
            }

            CoreHub.CommentManager.SoftDelete(id);
            return true;
        }

        /// <summary>
        /// Appends a reply to the parent comment identified by <paramref name="commentId"/>.
        /// </summary>
        /// <param name="commentId">The id of the parent comment.</param>
        /// <param name="reply">The reply body.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The created reply mapped to the REST DTO, or <c>null</c> when the
        /// parent does not exist or the reply body is empty.</returns>
        protected override RestApiCommentReply AppendReply(string commentId, string reply, IQueryContext context, IRequest request)
        {
            if (!Guid.TryParse(commentId, out var parentId) || string.IsNullOrWhiteSpace(reply))
            {
                return null;
            }

            var parent = CoreHub.CommentManager.GetComment(parentId);
            if (parent is null)
            {
                return null;
            }

            var replyComment = new Comment
            {
                Id = Guid.NewGuid(),
                ObjectId = parent.ObjectId,
                AuthorId = ResolveAuthorId(request),
                Content = reply.Trim(),
                State = CommentState.Active,
                ParentCommentId = parentId,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            CoreHub.CommentManager.Add(replyComment);

            var loaded = LoadWithAuthor(replyComment.Id);
            return new RestApiCommentReply
            {
                Id = loaded.Id.ToString(),
                Author = loaded.Author?.Name ?? "",
                Body = loaded.Content,
                When = FormatTimestamp(loaded.Created)
            };
        }

        /// <summary>
        /// Toggles a like on the comment for the identity resolved by name from the
        /// supplied <paramref name="user"/> argument (the control sends back whatever
        /// the host returned from <see cref="ResolveCurrentUser"/>). Returns the list
        /// of identity names that currently like the comment.
        /// </summary>
        /// <param name="commentId">The id of the comment.</param>
        /// <param name="user">The display name of the user toggling the like.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The current likers.</returns>
        protected override IEnumerable<string> ToggleLike(string commentId, string user, IQueryContext context, IRequest request)
        {
            if (!Guid.TryParse(commentId, out var id))
            {
                return [];
            }

            var authorId = ResolveAuthorIdByName(user) ?? ResolveAuthorId(request);
            return CoreHub.CommentManager.ToggleLike(id, authorId);
        }

        /// <summary>
        /// Toggles the <see cref="Comment.IsPinned"/> flag for the comment with the
        /// supplied id. Returns the new pin state, or <c>null</c> when no comment
        /// matches.
        /// </summary>
        /// <param name="commentId">The id of the comment.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The new pin state, or <c>null</c> when no comment matches.</returns>
        protected override bool? TogglePin(string commentId, IQueryContext context, IRequest request)
        {
            if (!Guid.TryParse(commentId, out var id))
            {
                return null;
            }

            return CoreHub.CommentManager.TogglePin(id);
        }

        /// <summary>
        /// Toggles an emoji reaction on the comment for the identity resolved by name
        /// from the supplied <paramref name="user"/> argument.
        /// </summary>
        /// <param name="commentId">The id of the comment.</param>
        /// <param name="user">The display name of the user toggling the reaction.</param>
        /// <param name="reaction">The emoji to toggle.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The reaction map (emoji → identity names) after the toggle.</returns>
        protected override IDictionary<string, IEnumerable<string>> ToggleReaction(string commentId, string user, string reaction, IQueryContext context, IRequest request)
        {
            if (!Guid.TryParse(commentId, out var id))
            {
                return new Dictionary<string, IEnumerable<string>>();
            }

            var authorId = ResolveAuthorIdByName(user) ?? ResolveAuthorId(request);
            return CoreHub.CommentManager.ToggleReaction(id, authorId, reaction);
        }

        /// <summary>
        /// Looks up an identity by its display name and returns the id, or <c>null</c>
        /// when no matching identity exists. The control sends back the name string from
        /// <see cref="ResolveCurrentUser"/>; we re-resolve it server-side rather than
        /// trusting the client-supplied value verbatim.
        /// </summary>
        /// <param name="name">The identity display name.</param>
        /// <returns>The identity id or <c>null</c>.</returns>
        private static Guid? ResolveAuthorIdByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            using var db = ModelHub.CreateDbContext();
            return db.Identities.AsNoTracking().FirstOrDefault(i => i.Name == name)?.Id;
        }

        /// <summary>
        /// Resolves the object id from the URL <c>{objectkey}</c> path segment by
        /// looking up the object by its <see cref="Object.Key"/>.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The object id, or <see cref="Guid.Empty"/> when the key is missing
        /// or no matching object exists.</returns>
        private static Guid ResolveObjectId(IRequest request)
        {
            var keyParam = request?.GetParameter<ObjectKeyParameter>();
            if (string.IsNullOrEmpty(keyParam?.Value))
            {
                return Guid.Empty;
            }

            using var db = ModelHub.CreateDbContext();
            var obj = db.Objects.AsNoTracking().FirstOrDefault(o => o.Key == keyParam.Value);
            return obj?.Id ?? Guid.Empty;
        }

        /// <summary>
        /// Resolves the author id for a new comment / reply. Looks up the identity by
        /// the name returned from <see cref="ResolveCurrentUser"/>; falls back to the
        /// seeded admin identity when no match exists.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The author identity id.</returns>
        private static Guid ResolveAuthorId(IRequest request)
        {
            var name = ResolveCurrentUserName(request);
            if (string.IsNullOrEmpty(name))
            {
                return FallbackAuthorId;
            }

            using var db = ModelHub.CreateDbContext();
            var identity = db.Identities.AsNoTracking().FirstOrDefault(i => i.Name == name);
            return identity?.Id ?? FallbackAuthorId;
        }

        /// <summary>
        /// Returns the display name of the active user. The control passes this string
        /// back through the like / pin / reaction endpoints; the endpoint then resolves
        /// the identity by name. Falls back to the seeded admin identity name when the
        /// request does not carry an authenticated session.
        /// </summary>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The current user's display name.</returns>
        protected override string ResolveCurrentUser(IQueryContext context, IRequest request)
        {
            return ResolveCurrentUserName(request);
        }

        private static string ResolveCurrentUserName(IRequest request)
        {
            // TODO: read the authenticated identity from request.Session once the
            // WebExpress identity flow exposes it on the request. Until then, every
            // anonymous request is attributed to the seeded admin identity so the
            // like / pin / reaction toggles still resolve to a valid author.
            using var db = ModelHub.CreateDbContext();
            var admin = db.Identities.AsNoTracking().FirstOrDefault(i => i.Id == FallbackAuthorId);
            return admin?.Name ?? "admin";
        }

        /// <summary>
        /// Loads the comment by id together with its author, likes, and reactions for
        /// response mapping.
        /// </summary>
        /// <param name="id">The comment id.</param>
        /// <returns>The comment with its author, likes, and reactions hydrated.</returns>
        private static Comment LoadWithAuthor(Guid id)
        {
            using var db = ModelHub.CreateDbContext();
            return db.Comments
                .Include(c => c.Author)
                .Include(c => c.Likes).ThenInclude(l => l.Author)
                .Include(c => c.Reactions).ThenInclude(r => r.Author)
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Maps a <see cref="Comment"/> to its <see cref="RestApiCommentItem"/>
        /// representation, attaching the nested reply collection when one is supplied.
        /// </summary>
        /// <param name="comment">The comment entity.</param>
        /// <param name="parentReplies">A dictionary of parent-id → replies, used when
        /// rendering a top-level comment with its nested replies. Pass <c>null</c> when
        /// mapping a freshly created comment that has no replies yet.</param>
        /// <returns>The REST DTO.</returns>
        private static RestApiCommentItem ToItem(Comment comment, IReadOnlyDictionary<Guid, List<Comment>> parentReplies)
        {
            if (comment is null)
            {
                return null;
            }

            var isDeleted = comment.State == CommentState.Deleted;

            var item = new RestApiCommentItem
            {
                Id = comment.Id.ToString(),
                Author = comment.Author?.Name ?? "",
                Body = isDeleted ? "" : comment.Content,
                When = FormatTimestamp(comment.Created),
                Category = comment.State.ToString(),
                Pinned = comment.IsPinned,
                Likes = comment.Likes?
                    .OrderBy(l => l.Created)
                    .Select(l => l.Author?.Name ?? "")
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList() ?? [],
                Reactions = comment.Reactions is { Count: > 0 }
                    ? comment.Reactions
                        .OrderBy(r => r.Created)
                        .GroupBy(r => r.Emoji)
                        .ToDictionary(
                            g => g.Key,
                            g => (IEnumerable<string>)g.Select(r => r.Author?.Name ?? "")
                                                       .Where(n => !string.IsNullOrEmpty(n))
                                                       .ToList())
                    : new Dictionary<string, IEnumerable<string>>(),
                Edited = comment.State == CommentState.Edited
                    ? new RestApiCommentEditInfo
                    {
                        By = comment.Author?.Name ?? "",
                        When = FormatTimestamp(comment.Updated)
                    }
                    : null
            };

            if (parentReplies is not null && parentReplies.TryGetValue(comment.Id, out var replies))
            {
                item.Replies = replies.Select(r => new RestApiCommentReply
                {
                    Id = r.Id.ToString(),
                    Author = r.Author?.Name ?? "",
                    Body = r.State == CommentState.Deleted ? "" : r.Content,
                    When = FormatTimestamp(r.Created)
                }).ToList();
            }

            return item;
        }

        /// <summary>
        /// Formats a timestamp as an ISO-8601 round-trip string (the format the
        /// client-side control parses).
        /// </summary>
        /// <param name="dt">The timestamp.</param>
        /// <returns>The ISO-8601 string.</returns>
        private static string FormatTimestamp(DateTime dt)
        {
            return dt.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}
