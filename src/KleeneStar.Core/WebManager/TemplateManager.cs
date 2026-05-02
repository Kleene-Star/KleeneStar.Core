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
    /// Defines the contract for managing templates, including adding, retrieving, and removing, as well as
    /// handling template-related events.
    /// </summary>
    public sealed class TemplateManager : ITemplateManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// An event that fires when a template is added.
        /// </summary>
        public event EventHandler<Template> TemplateAdded;

        /// <summary>
        /// An event that fires when a template is updated.
        /// </summary>
        public event EventHandler<Template> TemplateUpdated;

        /// <summary>
        /// An event that fires when a template is removed.
        /// </summary>
        public event EventHandler<Template> TemplateRemoved;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private TemplateManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns a template based on its id.
        /// </summary>
        /// <param name="templateId">The id of the template.</param>
        /// <returns>The template.</returns>
        public Template GetTemplate(Guid templateId)
        {
            var query = new Query<Template>()
                .Where(x => x.Id == templateId)
                .WithPaging(0, 1);

            return ModelHub.GetTemplates(query)
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a collection of templates that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned templates. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of templates that match the given predicate. If no template 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Template> GetTemplates(IQuery<Template> query)
        {
            return ModelHub.GetTemplates(query);
        }

        /// <summary>
        /// Retrieves a collection of templates that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned templates. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of templates that match the given predicate. If no template 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Template> GetTemplates(IQuery<Template> query, IQueryContext context)
        {
            if (context is KleeneStarDbContext db)
            {
                return ModelHub.GetTemplates(query, db);
            }

            return [];
        }

        /// <summary>
        /// Adds a new template to the collection.
        /// </summary>
        /// <param name="templateEntry">
        /// The template to add. Cannot be null.
        /// </param>
        /// <returns>The manager instance to allow method chaining.</returns>
        public ITemplateManager AddTemplate(Template templateEntry)
        {
            ModelHub.Add(templateEntry);

            TemplateAdded?.Invoke(this, templateEntry);

            return this;
        }

        /// <summary>
        /// Updates the properties of an existing template.
        /// </summary>
        /// <param name="templateEntry">
        /// The template with updated properties. Cannot be null.
        /// </param>
        /// <returns>The manager instance to allow method chaining.</returns>
        public ITemplateManager UpdateTemplate(Template templateEntry)
        {
            ModelHub.Update(templateEntry);

            TemplateUpdated?.Invoke(this, templateEntry);

            return this;
        }

        /// <summary>
        /// Removes the specified template from the collection.
        /// </summary>
        /// <param name="templateEntry">
        /// The template to remove. Cannot be null.
        /// </param>
        /// <returns>The manager instance to allow method chaining.</returns>
        public ITemplateManager RemoveTemplate(Template templateEntry)
        {
            ModelHub.Remove(templateEntry);

            TemplateRemoved?.Invoke(this, templateEntry);

            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Add disposal logic if necessary
        }
    }
}