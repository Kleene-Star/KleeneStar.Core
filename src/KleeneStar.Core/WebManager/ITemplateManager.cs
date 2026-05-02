using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing templates, including adding, retrieving, and removing, as well as
    /// handling template-related events.
    /// </summary>
    public interface ITemplateManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when a template is added.
        /// </summary>
        event EventHandler<Template> TemplateAdded;

        /// <summary>
        /// An event that fires when a template is updated.
        /// </summary>
        event EventHandler<Template> TemplateUpdated;

        /// <summary>
        /// An event that fires when a template is removed.
        /// </summary>
        event EventHandler<Template> TemplateRemoved;

        /// <summary>
        /// Returns a template based on its id.
        /// </summary>
        /// <param name="templateId">The id of the template.</param>
        /// <returns>The template.</returns>
        Template GetTemplate(Guid templateId);

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
        IEnumerable<Template> GetTemplates(IQuery<Template> query);

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
        IEnumerable<Template> GetTemplates(IQuery<Template> query, IQueryContext context);

        /// <summary>
        /// Adds a new template to the collection.
        /// </summary>
        /// <param name="templateEntry">
        /// The template to add. Cannot be null.
        /// </param>
        /// <returns>The manager instance to allow method chaining.</returns>
        ITemplateManager AddTemplate(Template templateEntry);

        /// <summary>
        /// Updates the properties of an existing template.
        /// </summary>
        /// <param name="templateEntry">
        /// The template with updated properties. Cannot be null.
        /// </param>
        /// <returns>The manager instance to allow method chaining.</returns>
        ITemplateManager UpdateTemplate(Template templateEntry);

        /// <summary>
        /// Removes the specified template from the collection.
        /// </summary>
        /// <param name="templateEntry">
        /// The template to remove. Cannot be null.
        /// </param>
        /// <returns>The manager instance to allow method chaining.</returns>
        ITemplateManager RemoveTemplate(Template templateEntry);
    }
}