using KleeneStar.Model;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Branding
{
    /// <summary>
    /// Serves the identity of the installation - the title and the icon the application is
    /// presented under - to the settings form and takes its updates.
    /// </summary>
    /// <remarks>
    /// The identity is a singleton, so only the retrieve and update halves of the CRUD contract
    /// are meaningful here. Creating or deleting is left to the inherited implementations, which
    /// the settings form never calls because it addresses the fixed record directly.
    /// </remarks>
    [Cache]
    public sealed class Index : RestApiCrud<Model.Entities.Branding>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Creates a new instance of an object that implements the IQueryContext interface.
        /// </summary>
        /// <returns>An IQueryContext instance that can be used to execute queries.</returns>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Retrieves the branding records that match the specified query criteria.
        /// </summary>
        /// <param name="query">The query parameters. Cannot be null.</param>
        /// <param name="context">The context in which the query is executed. Cannot be null.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The filtered set of records; may be empty.</returns>
        protected override IEnumerable<Model.Entities.Branding> Retrieve(IQuery<Model.Entities.Branding> query, IQueryContext context, IRequest request)
        {
            return CoreHub.BrandingManager.GetBrandings(query, context);
        }

        /// <summary>
        /// Retrieves the branding in preparation for an update.
        /// </summary>
        /// <param name="query">The query parameters. Cannot be null.</param>
        /// <param name="request">The request context.</param>
        /// <returns>An object containing the branding.</returns>
        protected override IRestApiCrudResultRetrieve RetrieveForUpdate(IQuery<Model.Entities.Branding> query, IRequest request)
        {
            // the record is the singleton the manager already holds, so the query is not applied
            // here and a fresh installation yields the empty default rather than nothing
            return RetrieveForUpdate(request, CoreHub.BrandingManager.GetBranding());
        }

        /// <summary>
        /// Persists the edited branding and applies it to the running application.
        /// </summary>
        /// <param name="existingItem">The currently persisted branding.</param>
        /// <param name="payload">The dynamic payload containing the edited branding.</param>
        /// <param name="request">The HTTP request providing additional context.</param>
        /// <returns>A result object containing information about the update operation.</returns>
        protected override IRestApiCrudResultUpdate Update(Model.Entities.Branding existingItem, RestApiCrudFormData payload, IRequest request)
        {
            // the icon is taken out of the payload and stored separately: the icon control submits
            // it inline as a data url, which the binder would hand to RestValueConverterImageIcon
            // and end up as the URI "http:///". See StoreIcon.
            var submitted = payload.Remove(nameof(Model.Entities.Branding.Icon).ToLowerInvariant(), out var icon);

            var res = base.Update(existingItem, payload, request);

            if (submitted)
            {
                StoreIcon(existingItem, icon as string);
            }

            CoreHub.BrandingManager.Update(existingItem);

            return res;
        }

        /// <summary>
        /// Applies the icon submitted by the settings form to the branding.
        /// </summary>
        /// <remarks>
        /// An empty value is how the form reports that the icon was removed. The application then
        /// falls back to the icon it declared through its <c>[Icon]</c> attribute, which is what
        /// the field's help text promises.
        /// </remarks>
        /// <param name="branding">The branding being saved.</param>
        /// <param name="payload">The submitted value, or <see langword="null"/> / empty when the
        /// icon was removed.</param>
        private static void StoreIcon(Model.Entities.Branding branding, string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                CoreHub.RemoveStoredIcons(Model.Entities.Branding.SingletonId);
                branding.Icon = null;

                return;
            }

            var stored = CoreHub.StoreIcon(Model.Entities.Branding.SingletonId, payload);

            // a payload that carries no usable image leaves the current icon alone rather than
            // clearing it - the administrator asked to change the icon, not to lose it
            if (stored is not null)
            {
                branding.Icon = stored;
            }
        }
    }
}
