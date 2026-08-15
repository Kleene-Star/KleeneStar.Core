using KleeneStar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Profile
{
    /// <summary>
    /// Serves the profile settings of the calling identity to the forms on the profile pages
    /// and takes their updates.
    /// </summary>
    /// <remarks>
    /// The endpoint is deliberately narrower than <see cref="Identities.Index"/>: every read
    /// and every write is confined to the identity the request is served for, so a caller who
    /// passes somebody else's id gets nothing rather than a foreign account to edit. Creating
    /// and deleting are not part of the contract — an account is created and removed through
    /// the identity administration, not through its own profile.
    /// </remarks>
    [Cache]
    public sealed class Index : RestApiCrud<Model.Entities.Identity>
    {
        /// <summary>
        /// The identity fields the profile forms own, keyed the way the payload names them
        /// (lower case). Anything outside this set is ignored on update.
        /// </summary>
        private static readonly HashSet<string> EditableFields = new(StringComparer.OrdinalIgnoreCase)
        {
            // profile page
            nameof(Model.Entities.Identity.Name),
            nameof(Model.Entities.Identity.Avatar),
            nameof(Model.Entities.Identity.Bio),
            nameof(Model.Entities.Identity.PhoneCountry),
            nameof(Model.Entities.Identity.Phone),
            nameof(Model.Entities.Identity.Website),
            nameof(Model.Entities.Identity.Location),
            nameof(Model.Entities.Identity.Position),

            // account page
            nameof(Model.Entities.Identity.Language),
            nameof(Model.Entities.Identity.TimeZone),
            nameof(Model.Entities.Identity.DateFormat),
            nameof(Model.Entities.Identity.WeekStart),

            // tenant & role page — the role itself is set by the workspace admins
            nameof(Model.Entities.Identity.Department),
            nameof(Model.Entities.Identity.CostCenter),
            nameof(Model.Entities.Identity.DeputyId)
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Creates a new instance of an object that implements the IQueryContext interface.
        /// </summary>
        /// <returns>An IQueryContext instance.</returns>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Retrieves the calling identity, ignoring any other identity the query may name.
        /// </summary>
        /// <param name="query">
        /// The query criteria. Not applied — the caller does not get to choose whose profile is
        /// returned.
        /// </param>
        /// <param name="context">The context in which the query is executed.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>
        /// A collection holding the calling identity, or an empty one when no identity can be
        /// resolved for the request.
        /// </returns>
        protected override IEnumerable<Model.Entities.Identity> Retrieve
        (
            IQuery<Model.Entities.Identity> query,
            IQueryContext context,
            IRequest request
        )
        {
            var identityId = CoreHub.SessionManager.GetCurrentIdentityId(request);

            if (identityId == Guid.Empty)
            {
                return [];
            }

            var own = new Query<Model.Entities.Identity>()
                .WhereEquals(x => x.Id, identityId)
                .WithPaging(0, 1);

            return CoreHub.IdentityManager
                .GetIdentities(own, context)
                .Select(Sanitize);
        }

        /// <summary>
        /// Blanks the credential fields of an identity before it leaves the server.
        /// </summary>
        /// <remarks>
        /// The REST serializer reflects over every public property and does not honour
        /// <see cref="System.Text.Json.Serialization.JsonIgnoreAttribute"/>, so a property that
        /// is invisible to a plain serialization still reaches the client here. The password
        /// hash is nothing the profile form has any use for, so it is removed from the copy
        /// that is serialized.
        /// </remarks>
        /// <param name="identity">The identity about to be serialized. May be null.</param>
        /// <returns>The same instance with its credential fields cleared.</returns>
        private static Model.Entities.Identity Sanitize(Model.Entities.Identity identity)
        {
            if (identity is not null)
            {
                identity.PasswordHash = null;
            }

            return identity;
        }

        /// <summary>
        /// Persists the edited profile settings of the calling identity.
        /// </summary>
        /// <remarks>
        /// Only the fields the profile forms own are taken from the payload. Everything else an
        /// identity carries — its state, its password, the tenant it belongs to, the role the
        /// workspace admins assigned — is administered elsewhere and must not become writable
        /// just because it sits on the same record as the user's own settings.
        /// </remarks>
        /// <param name="existingItem">The currently persisted identity.</param>
        /// <param name="payload">The dynamic payload containing the edited settings.</param>
        /// <param name="request">The HTTP request providing additional context.</param>
        /// <returns>A result object containing information about the update operation.</returns>
        protected override IRestApiCrudResultUpdate Update
        (
            Model.Entities.Identity existingItem,
            RestApiCrudFormData payload,
            IRequest request
        )
        {
            var editable = new RestApiCrudFormData();

            foreach (var entry in payload.Where(x => EditableFields.Contains(x.Key)))
            {
                editable[entry.Key] = entry.Value;
            }

            // the picture is taken out of the payload and stored separately: the avatar control
            // submits it inline as a data url, which the binder would hand to
            // RestValueConverterImageIcon and end up as the URI "http:///". See StoreAvatar.
            editable.Remove(nameof(Model.Entities.Identity.Avatar).ToLowerInvariant(), out var avatar);

            // the item handed in came through Sanitize, which blanked the password hash on its
            // way to the client. Saving that copy would write the blank back, so the edits are
            // applied to a freshly read record instead.
            var persisted = CoreHub.IdentityManager.GetIdentity(existingItem.Id) ?? existingItem;

            var res = base.Update(persisted, editable, request);

            if (payload.ContainsKey(nameof(Model.Entities.Identity.Avatar).ToLowerInvariant()))
            {
                StoreAvatar(persisted, avatar as string);
            }

            // an identity must not stand in for itself — the deputy would be the very account
            // that is absent
            if (persisted.DeputyId == persisted.Id)
            {
                persisted.DeputyId = null;
            }

            CoreHub.IdentityManager.Update(persisted);

            return res;
        }

        /// <summary>
        /// Applies the picture submitted by the profile form to the identity.
        /// </summary>
        /// <remarks>
        /// The avatar control has no upload endpoint of its own; it posts the picture inline as
        /// <c>file:&lt;name&gt;;data:&lt;mime&gt;;base64,&lt;payload&gt;</c>. The property behind
        /// it is an <see cref="WebExpress.WebUI.WebIcon.ImageIcon"/>, which holds a URI, and the
        /// converter in between passes the whole string to <c>ImageIcon.FromString</c> — the
        /// data url is parsed as a URI and collapses to <c>http:///</c>, which is why an
        /// uploaded picture used to vanish while the request still answered 200. So the payload
        /// is decoded and written to the icons directory here, and the identity is pointed at
        /// the file.
        ///
        /// An empty value is how the form reports that the picture was removed. The identity
        /// then falls back to the generated initials icon, matching what the field's help text
        /// promises.
        /// </remarks>
        /// <param name="identity">The identity being saved.</param>
        /// <param name="payload">
        /// The submitted value, or <see langword="null"/> / empty when the picture was removed.
        /// </param>
        private static void StoreAvatar(Model.Entities.Identity identity, string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                CoreHub.RemoveStoredIcons(identity.Id);
                identity.Avatar = CoreHub.GenerateIcon(identity.Id);

                return;
            }

            var stored = CoreHub.StoreIcon(identity.Id, payload);

            // a payload that carries no usable image leaves the current picture alone rather
            // than clearing it — the user asked to change the avatar, not to lose it
            if (stored is not null)
            {
                identity.Avatar = stored;
            }
        }

        /// <summary>
        /// Deletes nothing. An account is removed through the identity administration, not
        /// through its own profile settings.
        /// </summary>
        /// <param name="existingItem">The identity the request addressed.</param>
        /// <param name="request">The HTTP request providing additional context.</param>
        /// <returns>A result object reporting that nothing was deleted.</returns>
        protected override IRestApiCrudResultDelete Delete(Model.Entities.Identity existingItem, IRequest request)
        {
            return new RestApiCrudResultDelete();
        }
    }
}
