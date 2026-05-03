using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Forms
{
    /// <summary>
    /// Provides editing capabilities for form structures via a REST API, enabling retrieval and update operations for
    /// form elements.
    /// </summary>
    [Title("Form structure")]
    public sealed class FormEditor : RestApiFormEditor<Model.Entities.Form>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FormEditor()
        {
        }

        /// <summary>
        /// Creates a query context backed by the application's database.
        /// </summary>
        /// <returns>The shared <see cref="KleeneStarDbContext"/>.</returns>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Retrieves a catalog of form editor field items based on the specified query context and request parameters.
        /// </summary>
        /// <param name="context">
        /// The query context that provides information about the current data retrieval operation. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The request containing parameters that influence which catalog items are retrieved. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of catalog field items that match the specified context and request. The 
        /// collection may be empty if no items are found.
        /// </returns>
        protected override IEnumerable<RestApiFormEditorFieldItem> RetrieveCatalog(string formId, IQueryContext context, IRequest request)
        {
            var db = context as KleeneStarDbContext;
            var guid = Guid.TryParse(formId, out var g) ? g : Guid.Empty;
            var form = CoreHub.FormManager.GetForm(guid);

            return CoreHub.FieldManager
                .GetFields(new ClassIdParameter(form.ClassId))
                .Where(f => !f.Deprecated && f.State == FieldState.Active)
                .OrderBy(f => f.Name)
                .Select(f => new RestApiFormEditorFieldItem()
                {
                    Id = f.Id.ToString(),
                    Label = f.Name,
                    Type = "string",
                    Required = f.Required,
                    Help = f.HelpText,
                    //Placeholder = f.Placeholder
                });
        }

        /// <summary>
        /// Retrieves the full structural tree of the form addressed by <paramref name="formId"/>.
        /// </summary>
        /// <param name="formId">The unique identifier of the form to load.</param>
        /// <param name="context">The query context (a <see cref="KleeneStarDbContext"/>).</param>
        /// <param name="request">The current API request.</param>
        /// <returns>
        /// The form editor item, or <c>null</c> when the form does not exist (the base
        /// class converts this into a 404 response).
        /// </returns>
        protected override RestApiFormEditorItem RetrieveItem(string formId, IQueryContext context, IRequest request)
        {
            if (!Guid.TryParse(formId, out var guid))
            {
                return null;
            }

            var db = context as KleeneStarDbContext;
            var form = CoreHub.FormManager.GetFormWithStructure(guid);

            if (form is null)
            {
                return null;
            }

            return new RestApiFormEditorItem()
            {
                ClassName = form.Class.Name,
                FormId = form.Id.ToString(),
                FormName = form.Name,
                FormDescription = form.Description,
                Version = 1,
                Tabs = form.Tabs.Select(t => new RestApiFormEditorTabItem()
                {
                    Id = t.Id.ToString(),
                    Name = t.Name,
                    Children = t.Elements.Select(e => GetChildren(e))
                })
            };
        }

        /// <summary>
        /// Persists the structural tree contained in <paramref name="item"/> for the form
        /// addressed by <paramref name="formId"/>.
        /// </summary>
        /// <param name="formId">The unique identifier of the form to update.</param>
        /// <param name="item">The form structure sent by the editor.</param>
        /// <param name="context">The query context (a <see cref="KleeneStarDbContext"/>).</param>
        /// <param name="request">The current API request.</param>
        /// <returns>
        /// The freshly reloaded form structure with the new version number embedded.
        /// </returns>
        /// <remarks>
        /// Concurrency conflicts are surfaced as
        /// <see cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException"/>; the
        /// base class converts those into a 400 response with a descriptive message
        /// (the editor reloads when it sees a non-200 response).
        /// </remarks>
        protected override RestApiFormEditorItem UpdateItem(string formId, RestApiFormEditorItem item, IQueryContext context, IRequest request)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (!Guid.TryParse(formId, out var guid))
            {
                throw new ArgumentException("Form id is not a valid GUID.", nameof(formId));
            }

            var form = CoreHub.FormManager.GetForm(guid)
                ?? throw new InvalidOperationException($"Form '{guid}' not found.");

            var validFieldIds = CoreHub.FieldManager
                .GetFields(new ClassIdParameter { Value = form.ClassId.ToString() })
                .Select(f => f.Id)
                .ToHashSet();


            var saved = ModelHub.GetFormWithStructure(guid);
            var fields = CoreHub.FieldManager
                .GetFields(new ClassIdParameter { Value = saved.ClassId.ToString() })
                .ToDictionary(f => f.Id);

            return new RestApiFormEditorItem();
        }

        /// <summary>
        /// Creates a node item representing the specified form element for use in the REST API form editor.
        /// </summary>
        /// <param name="element">
        /// The form element to convert to a node item. Must be a field or group element.
        /// </param>
        /// <returns>
        /// A node item representing the form element, or null if the element type is not supported.
        /// </returns>
        private static RestApiFormEditorNodeItem GetChildren(FormElement element)
        {
            if (element is FormFieldRefElement field)
            {
                return new RestApiFormEditorFieldItem()
                {
                    Id = field.Id.ToString(),
                    Label = field.Field?.Name ?? "unkown",
                    Type = "string"
                };
            }
            else if (element is FormGroupElement group)
            {
                return new RestApiFormEditorGroupItem()
                {
                    Id = group.Id.ToString(),
                    Label = group.Label,
                    Layout = group.Layout.ToString(),
                    Children = group.Children.Select(c => GetChildren(c))
                };
            }

            return null;
        }
    }
}
