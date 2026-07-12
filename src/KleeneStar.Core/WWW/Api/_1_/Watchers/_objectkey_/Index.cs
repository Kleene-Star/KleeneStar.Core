using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Watchers._objectkey_
{
    /// <summary>
    /// REST endpoint backing the <c>ControlRestObserver</c> hosted by
    /// <see cref="WebFragment.Object.ObjectPropertyPeopleCardFragment"/> on an object
    /// detail page. The URL is <c>/api/1/watchers/{objectkey}</c>; the
    /// <c>{objectkey}</c> URL segment is declared via
    /// <see cref="ObjectKeySegmentAttribute"/> so callers can bind the segment by
    /// passing the current request's <see cref="ObjectKeyParameter"/> through
    /// <see cref="WebExpress.WebCore.WebUri.IUriExtensions.BindParameters"/>.
    /// </summary>
    /// <remarks>
    /// Persistence is delegated to <see cref="CoreHub.WatcherManager"/>, which keeps
    /// one <see cref="ObjectWatcher"/> row per (object, identity) pair. The
    /// <see cref="RestApiObserverItem"/> DTO consumed by the client-side observer
    /// control does not match an entity 1:1: <see cref="RestApiObserverItem.Initials"/>
    /// and <see cref="RestApiObserverItem.Color"/> are derived deterministically from
    /// the identity name and id so the avatar bubble keeps a stable look without
    /// needing dedicated stored columns.
    /// <para>
    /// <see cref="IncludeSubPathsAttribute"/> is REQUIRED so that the
    /// <c>DELETE {base}/{userId}</c> sub-route is dispatched to this endpoint's
    /// <see cref="RemoveObserver"/> override — without it the sub-path 404s and the
    /// "click avatar to remove" affordance silently degrades.
    /// </para>
    /// </remarks>
    [Title("kleenestar.core:object.watcher.api.title")]
    [ObjectKeySegment]
    [IncludeSubPaths(true)]
    [Cache]
    public sealed class Index : RestApiObserver<Model.Entities.Object>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Returns the watchers currently attached to the object addressed by the URL
        /// <c>{objectkey}</c> segment.
        /// </summary>
        /// <param name="query">The query criteria supplied by the control.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The materialized list of observer items.</returns>
        protected override IEnumerable<RestApiObserverItem> RetrieveObservers(IQuery<Model.Entities.Object> query, IQueryContext context, IRequest request)
        {
            var objectId = ResolveObjectId(request);
            if (objectId == Guid.Empty)
            {
                return [];
            }

            return CoreHub.WatcherManager
                .GetWatchers(objectId)
                .Where(w => w.Identity is not null)
                .Select(w => ToObserverItem(w.Identity))
                .ToList();
        }

        /// <summary>
        /// Adds the identity identified by <paramref name="userId"/> as a watcher of
        /// the object addressed by the request. Returns <see langword="null"/> when the
        /// supplied id is malformed, the identity does not exist, or no object can be
        /// resolved from the request; the base class translates that into a
        /// <see cref="ResponseNotFound"/>.
        /// </summary>
        /// <param name="userId">The identity id supplied by the client.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The newly added observer record, or <see langword="null"/>.</returns>
        protected override RestApiObserverItem AddObserver(string userId, IQueryContext context, IRequest request)
        {
            var objectId = ResolveObjectId(request);
            if (objectId == Guid.Empty || !Guid.TryParse(userId, out var identityId))
            {
                return null;
            }

            var watcher = CoreHub.WatcherManager.Add(objectId, identityId);
            if (watcher is null)
            {
                return null;
            }

            using var db = ModelHub.CreateDbContext();
            var identity = db.Identities.AsNoTracking().FirstOrDefault(i => i.Id == identityId);
            return identity is null ? null : ToObserverItem(identity);
        }

        /// <summary>
        /// Removes the identity identified by <paramref name="userId"/> from the
        /// watcher list of the object addressed by the request. Returns
        /// <see langword="false"/> when the supplied id is malformed, no object can be
        /// resolved, or the identity was not watching the object to begin with.
        /// </summary>
        /// <param name="userId">The identity id supplied by the client.</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns><see langword="true"/> when the watcher existed and was removed.</returns>
        protected override bool RemoveObserver(string userId, IQueryContext context, IRequest request)
        {
            var objectId = ResolveObjectId(request);
            if (objectId == Guid.Empty || !Guid.TryParse(userId, out var identityId))
            {
                return false;
            }

            return CoreHub.WatcherManager.Remove(objectId, identityId);
        }

        /// <summary>
        /// Resolves the object id from the URL <c>{objectkey}</c> path segment by
        /// looking up the object by its <see cref="Model.Entities.Object.Key"/>.
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
        /// Projects an <see cref="Identity"/> row onto the
        /// <see cref="RestApiObserverItem"/> DTO consumed by the client-side observer
        /// control. The team slot is left empty because the identity model does not
        /// surface a single primary team; initials and avatar background colour are
        /// derived deterministically from the display name and id so the avatar bubble
        /// keeps a stable look without needing a stored colour column.
        /// </summary>
        /// <param name="identity">The identity row.</param>
        /// <returns>The observer DTO.</returns>
        internal static RestApiObserverItem ToObserverItem(Identity identity)
        {
            return new RestApiObserverItem
            {
                Id = identity.Id.ToString(),
                Name = identity.Name ?? string.Empty,
                Team = string.Empty,
                Initials = BuildInitials(identity.Name),
                Color = BuildColor(identity.Id)
            };
        }

        /// <summary>
        /// Derives a 1-2 character initials string from the supplied display name. Uses
        /// the first letter of the first and last whitespace-separated token; falls back
        /// to the first two characters of the name when only a single token is present.
        /// </summary>
        /// <param name="name">The identity display name.</param>
        /// <returns>The initials.</returns>
        private static string BuildInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "?";
            }

            var tokens = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 1)
            {
                return tokens[0].Length >= 2
                    ? tokens[0][..2].ToUpperInvariant()
                    : tokens[0].ToUpperInvariant();
            }

            return string.Concat(tokens[0][..1], tokens[^1][..1]).ToUpperInvariant();
        }

        /// <summary>
        /// Derives a stable CSS hex colour for the avatar background from the identity
        /// id. The hash output is masked to a 24-bit value so the result is always a
        /// valid six-digit hex triplet.
        /// </summary>
        /// <param name="id">The identity id.</param>
        /// <returns>A CSS hex colour string of the form <c>#RRGGBB</c>.</returns>
        private static string BuildColor(Guid id)
        {
            var hash = id.GetHashCode() & 0x00FFFFFF;
            return "#" + hash.ToString("x6");
        }
    }
}
