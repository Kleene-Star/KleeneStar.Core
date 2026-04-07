using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing fields, including adding, retrieving, and removing, as well as
    /// handling field-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing fields and events for tracking changes 
    /// to the field collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public sealed class FormManager : IFormManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// An event that fires when an form is added.
        /// </summary>
        public event EventHandler<Form> FormAdded;

        /// <summary>
        /// An event that fires when an form is udpated.
        /// </summary>
        public event EventHandler<Form> FormUpdated;

        /// <summary>
        /// An event that fires when an form is removed.
        /// </summary>
        public event EventHandler<Form> FormRemoved;

        /// <summary>
        /// Returns the collection of workspace keys that are reserved and cannot be used for custom workspaces.
        /// </summary>
        /// <remarks>
        /// The reserved keys typically represent system-defined workspaces and are not available
        /// for user-defined or custom workspace creation.
        /// </remarks>
        public static IEnumerable<string> ReservedFormNames =>
        [
            "default", "admin", "system", "assets", "api", "workspace",
            "workspaces", "icons", "setting"
        ];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private FormManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns a form based on its id.
        /// </summary>
        /// <param name="formId">The id of the form.</param>
        /// <returns>The form.</returns>
        public Form GetForm(Guid formId)
        {
            var query = new Query<Form>()
                .Where(x => x.Id == formId)
                .WithPaging(0, 1);

            return ModelHub.GetForms(query)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns a form based on its id.
        /// </summary>
        /// <param name="formId">The id of the form.</param>
        /// <returns>The form.</returns>
        public Form GetForm(FormIdParameter formId)
        {
            var guid = Guid.TryParse(formId.Value, out Guid id) ? id : Guid.Empty;

            return GetForm(guid);
        }

        /// <summary>
        /// Retrieves a collection of forms that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="classId">The id of the class.</param>
        /// <returns>
        /// An enumerable collection of forms that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Form> GetForms(ClassIdParameter classId)
        {
            var guid = Guid.TryParse(classId.Value, out Guid id) ? id : Guid.Empty;
            var query = new Query<Form>()
                .WhereEquals(x => x.ClassId, guid)
                .WithPaging(0, 1);

            return ModelHub.GetForms(query);
        }

        /// <summary>
        /// Retrieves a collection of forms that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned forms. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of forms that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Form> GetForms(IQuery<Form> query)
        {
            return ModelHub.GetForms(query);
        }

        /// <summary>
        /// Retrieves a collection of forms that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned forms. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of forms that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Form> GetForms(IQuery<Form> query, IQueryContext context)
        {
            return ModelHub.GetForms(query, context as KleeneStarDbContext);
        }

        /// <summary>
        /// Adds a form to the manager.
        /// </summary>
        /// <param name="formEntity">The form to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IFormManager Add(Form formEntity)
        {
            ArgumentNullException.ThrowIfNull(formEntity);

            ModelHub.Add(formEntity);

            FormAdded?.Invoke(this, formEntity);

            // create notification
            CoreHub.AddNotification("Create", "success", 5000);

            return this;
        }

        /// <summary>
        /// Update a form to the manager.
        /// </summary>
        /// <param name="formEntity">The form to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IFormManager Update(Form formEntity)
        {
            ArgumentNullException.ThrowIfNull(formEntity);

            ModelHub.Update(formEntity);

            FormUpdated?.Invoke(this, formEntity);

            // create notification
            CoreHub.AddNotification("Clone", "success", 5000);

            return this;
        }

        /// <summary>
        /// Removes the specified form from the manager.
        /// </summary>
        /// <remarks>This method removes the specified form from the manager. If the form does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="formId">The form id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IFormManager Remove(Guid formId)
        {
            var formEntry = GetForm(formId);

            if (formEntry is not null)
            {
                ModelHub.Remove(formEntry);
                FormRemoved?.Invoke(this, formEntry);
            }

            return this;
        }

        /// <summary>
        /// Release of unmanaged resources reserved during use.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
