using KleeneStar.Model.Entity;
using System;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace KleeneStar.Core.WWW.Api._1.Workspaces
{
    /// <summary>
    /// Provides CRUD operations for workspace items via a REST API.
    /// </summary>
    public sealed class Index : RestApiCrud<IWorkspace>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Retrieves a collection of index items of type TIndexItem.
        /// </summary>
        /// <returns>
        /// An enumerable collection of TIndexItem objects. The collection is empty if 
        /// no items are available.
        /// </returns>
        public override IEnumerable<IWorkspace> Retrieve()
        {
            return CoreHub.WorkspaceManager.Workspaces;
        }

        /// <summary>
        /// Retrieves the data required to create a new workspace entity.
        /// </summary>
        /// <param name="request">
        /// The request context containing parameters and metadata for the retrieval operation.
        /// </param>
        /// <returns>
        /// An object containing the information necessary to initialize a new workspace for creation.
        /// </returns>
        public override IRestApiCrudResultRetrieve<IWorkspace> RetrieveForCreate(IRequest request)
        {
            return new RestApiCrudResultRetrieve<IWorkspace>()
            {
                Title = I18N.Translate(request, "kleenestar.core:workspace.add.header")
            };
        }

        /// <summary>
        /// Retrieves a workspace identified by the specified key for update operations.
        /// </summary>
        /// <param name="id">
        /// The unique identifier that identifies the workspace to retrieve. Cannot be null or empty.
        /// </param>
        /// <param name="request">
        /// The request context containing additional information for the retrieval operation.
        /// </param>
        /// <returns>
        /// An object containing the workspace associated with the specified key.
        /// </returns>
        public override IRestApiCrudResultRetrieve<IWorkspace> RetrieveForUpdate(string id, IRequest request)
        {
            return new RestApiCrudResultRetrieve<IWorkspace>()
            {
                Title = I18N.Translate(request, "kleenestar.core:workspace.edit.header"),
                Data = CoreHub.WorkspaceManager.GetWorkspace(Guid.Parse(id))
            };
        }

        /// <summary>
        /// Retrieves the workspace entity identified by the specified ID in preparation for deletion.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the workspace to retrieve for deletion. Cannot 
        /// be null or empty.
        /// </param>
        /// <param name="request">
        /// The request context containing additional information for 
        /// the retrieval operation.
        /// </param>
        /// <returns>
        /// An object containing the workspace entity and related information required 
        /// for the delete operation.
        /// </returns>
        public override IRestApiCrudResultRetrieveDelete<IWorkspace> RetrieveForDelete(string id, IRequest request)
        {
            var data = CoreHub.WorkspaceManager.GetWorkspace(Guid.Parse(id));
            return new RestApiCrudResultRetrieveDelete<IWorkspace>()
            {
                Data = data,
                Title = I18N.Translate(request, "kleenestar.core:workspace.delete.header"),
                ConfirmItem = data?.Key
            };
        }

        public override IResponse Create(IRequest request)
        {
            return base.Create(request);
        }

        /// <summary>
        /// Validate the data for create or update operations. When creating, existingItem will 
        /// be null and proposedItem contains the values to create. When updating, existingItem 
        /// is the currently persisted entity and proposedItem contains the incoming values to 
        /// validate.
        /// </summary>
        /// <param name="existingItem">
        /// The currently persisted item (null for create).
        /// </param>
        /// <param name="payload">
        /// The dynamic payload containing updated fields.
        /// </param>
        /// <param name="request">
        /// The HTTP request providing additional context.
        /// </param>
        /// <returns>
        /// An IRestApiValidationResult indicating validation success or errors.
        /// </returns>
        public override IRestApiValidationResult Validate(IWorkspace existingItem, RestApiCrudFormData payload, IRequest request)
        {
            return base.Validate(existingItem, payload, request);
        }

        /// <summary>
        /// Updates the data record.
        /// </summary>
        /// <param name="existingItem">
        /// The currently persisted item.
        /// </param>
        /// <param name="payload">
        /// The dynamic payload containing updated fields.
        /// </param>
        /// <param name="request">
        /// The HTTP request providing additional context.
        /// </param>
        public override IRestApiCrudResultUpdate Update(IWorkspace existingItem, RestApiCrudFormData payload, IRequest request)
        {
            return base.Update(existingItem, payload, request);
        }

        /// <summary>
        /// Deletes the specified resource.
        /// </summary>
        /// <param name="existingItem">
        /// The currently persisted item that is to be deleted.
        /// </param>
        /// <param name="request">
        /// The HTTP request providing additional context for the delete operation.
        /// </param>
        /// <returns>
        /// A result object containing information about the delete operation.
        /// </returns>
        public override IRestApiCrudResultDelete Delete(IWorkspace existingItem, IRequest request)
        {
            CoreHub.WorkspaceManager.RemoveWorkspace(existingItem.Id);

            return base.Delete(existingItem, request);
        }
    }
}
