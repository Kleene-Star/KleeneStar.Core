using KleeneStar.Model.Entity;
using System;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace KleeneStar.Core.WWW.Api._1_.Workspaces
{
    /// <summary>
    /// Provides CRUD operations for workspace items via a REST API.
    /// </summary>
    public sealed class Index : RestApiCrud<Workspace>
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
        protected override IEnumerable<Workspace> Retrieve()
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
        protected override IRestApiCrudResultRetrieve<Workspace> RetrieveForCreate(IRequest request)
        {
            return new RestApiCrudResultRetrieve<Workspace>()
            {
                Title = I18N.Translate(request, "kleenestar.core:workspace.add.title")
            };
        }

        /// <summary>
        /// Retrieves a result object containing default values and metadata for 
        /// cloning a item.
        /// </summary>
        /// <param name="id">
        /// The identifier of the item to retrieve. The comparison is case-insensitive.
        /// </param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A result instance representing the data and metadata required
        /// to initialize a new item for creation.
        /// </returns>
        protected override IRestApiCrudResultRetrieve<Workspace> RetrieveForClone(string id, IRequest request)
        {
            var data = CoreHub.WorkspaceManager.GetWorkspace(Guid.Parse(id));
            var newItem = new Model.Entity.Workspace()
            {
                Key = data.Key + "-copy",
                Name = data.Name + " (Copy)",
                Description = data.Description,
                Categories = data.Categories,
                Icon = data.Icon,
                State = TypeWorkspaceState.Active
            };

            return new RestApiCrudResultRetrieve<Workspace>()
            {
                Title = I18N.Translate(request, "kleenestar.core:workspace.clone.title"),
                Data = newItem
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
        protected override IRestApiCrudResultRetrieve<Workspace> RetrieveForUpdate(string id, IRequest request)
        {
            return new RestApiCrudResultRetrieve<Workspace>()
            {
                Title = I18N.Translate(request, "kleenestar.core:workspace.edit.title"),
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
        protected override IRestApiCrudResultRetrieveDelete<Workspace> RetrieveForDelete(string id, IRequest request)
        {
            var data = CoreHub.WorkspaceManager.GetWorkspace(Guid.Parse(id));
            return new RestApiCrudResultRetrieveDelete<Workspace>()
            {
                Data = data,
                Title = I18N.Translate(request, "kleenestar.core:workspace.delete.title"),
                ConfirmItem = data?.Key
            };
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
        protected override IRestApiValidationResult Validate(Workspace existingItem, RestApiCrudFormData payload, IRequest request)
        {
            return base.Validate(existingItem, payload, request);
        }

        /// <summary>
        /// Persists the newly created resource.
        /// Override this method in derived classes to implement the actual
        /// persistence logic and return a result describing the creation.
        /// </summary>
        /// <param name="fieldMap">
        /// The dynamic payload containing the fields required to create the resource.
        /// </param>
        /// <param name="request">
        /// The HTTP request providing additional context for the creation process.
        /// </param>
        /// <param name="newItem">
        /// When the method returns, contains the newly created index item,
        /// or the default value if creation was not successful.
        /// </param>
        /// <returns>
        /// A result object containing information about the create operation,
        /// including the created resource.
        /// </returns>
        protected override IRestApiCrudResultCreate Create(RestApiCrudFormData fieldMap, IRequest request, out Workspace newItem)
        {
            var id = Guid.NewGuid();
            newItem = new Model.Entity.Workspace(id)
            {
                Icon = CoreHub.GenerateIcon(id),
                State = TypeWorkspaceState.Active
            };

            fieldMap.BindTo(newItem);

            CoreHub.WorkspaceManager.AddWorkspace(newItem);

            return new RestApiCrudResultCreate();
        }

        /// <summary>
        /// Creates a new workspace instance by cloning data from the specified form fields and 
        /// adds it to the workspace manager.
        /// </summary>
        /// <param name="existingItem">
        /// The existing workspace item to use as a reference for the clone operation. This parameter 
        /// is not modified.
        /// </param>
        /// <param name="fieldMap">
        /// The form data containing field values to bind to the new workspace instance. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The current request context for the operation. Provides additional information or 
        /// services required during cloning.
        /// </param>
        /// <param name="newItem">
        /// When this method returns, contains the newly created workspace instance populated 
        /// with the provided form data.
        /// </param>
        /// <returns>
        /// A result object indicating the outcome of the create operation.
        /// </returns>
        protected override IRestApiCrudResultCreate Clone(Workspace existingItem, RestApiCrudFormData fieldMap, IRequest request, out Workspace newItem)
        {
            var id = Guid.NewGuid();
            newItem = new Model.Entity.Workspace(id)
            {
                Icon = CoreHub.GenerateIcon(id),
                State = TypeWorkspaceState.Active
            };

            fieldMap.BindTo(newItem);

            CoreHub.WorkspaceManager.AddWorkspace(newItem);

            return new RestApiCrudResultCreate();
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
        protected override IRestApiCrudResultUpdate Update(Workspace existingItem, RestApiCrudFormData payload, IRequest request)
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
        protected override IRestApiCrudResultDelete Delete(Workspace existingItem, IRequest request)
        {
            CoreHub.WorkspaceManager.RemoveWorkspace(existingItem.Id);

            return base.Delete(existingItem, request);
        }
    }
}
