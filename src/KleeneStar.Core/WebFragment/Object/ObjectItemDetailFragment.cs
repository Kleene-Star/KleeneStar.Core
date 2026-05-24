using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Represents a control panel that displays detailed information about a specific
    /// object within the user interface.
    /// </summary>
    /// <remarks>
    /// The control resolves the object addressed by the current request, looks up the
    /// <see cref="FormType.View"/> form configured on its class, and renders that
    /// structure as a read-only view that mirrors the form's tabs, layout groups, and
    /// field references. Unlike <see cref="ObjectEditFormFragment"/> the
    /// view is not a <c>&lt;form&gt;</c> with validation and a submit panel; instead
    /// each value is wrapped in a <see cref="ControlSmartEdit"/> that persists changes
    /// inline via the object REST API as soon as the user finishes editing.
    /// </remarks>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Cache]
    public sealed class ObjectItemDetailFragment : FragmentControlPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public ObjectItemDetailFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
        }

        /// <summary>
        /// Converts the control to an HTML representation. The displayed structure is
        /// derived from the active <see cref="FormType.View"/> form of the resolved
        /// object's class. When no such form is configured, the summary and description
        /// of the object are shown as a minimal default.
        /// </summary>
        /// <param name="renderContext">The context in which the control is rendered.</param>
        /// <param name="visualTree">The visual tree representing the control's structure.</param>
        /// <returns>An HTML node representing the rendered control.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var keyParam = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            var @object = CoreHub.ObjectManager.GetObjectByKey(keyParam);
            var role = Role?.Invoke(renderContext);

            var html = new HtmlElementTextContentDiv()
            {
                Id = Id,
                Class = Css.Concatenate("wx-kleenestar-object-detail", GetClasses()),
                Style = GetStyles(),
                Role = role,
                //DataTheme = Theme.ToValue()
            };

            if (@object is null)
            {
                return html;
            }

            var objectUri = ResolveObjectRestUri(@object, renderContext);

            html.AddUserAttribute("data-object-id", @object.Id.ToString());
            html.AddUserAttribute("data-object-key", @object.Key);
            html.AddUserAttribute("data-rest-url", objectUri?.ToString());

            // Always render the description as inline-editable system
            // attributes. They are rendered before any class-specific structure so the
            // header information stays where the user expects it.
            html.Add(BuildSmartEdit(@object, objectUri, nameof(Model.Entities.Object.Description), @object.Description, "kleenestar.core:object.description.label", multiline: true)
                .Render(renderContext, visualTree));

            var form = ResolveStandardForm(@object.ClassId, FormType.View);

            if (form is null || form.Tabs is null || form.Tabs.Count == 0)
            {
                return html;
            }

            var fields = CoreHub.FieldManager
                .GetFields(new ClassIdParameter(@object.ClassId))
                .Where(f => !f.Deprecated && f.State == FieldState.Active)
                .ToDictionary(f => f.Id);

            var values = LoadValuesForObject(@object.Id);

            var orderedTabs = form.Tabs.OrderBy(t => t.Position).ToList();

            if (orderedTabs.Count == 1)
            {
                var grid = BuildFieldGrid(orderedTabs[0].Elements, fields, values, @object, objectUri);
                if (grid is not null)
                {
                    html.Add(grid.Render(renderContext, visualTree));
                }

                return html;
            }

            var tabControl = new ControlTab("tabs-" + @object.Id.ToString("N"))
            {
                Layout = _ => TypeLayoutTab.Underline
            };

            foreach (var t in orderedTabs)
            {
                var view = new ControlTabView("tab-" + t.Id.ToString("N"))
                {
                    Title = _ => t.Name
                };

                var tabGrid = BuildFieldGrid(t.Elements, fields, values, @object, objectUri);
                if (tabGrid is not null)
                {
                    view.Add(tabGrid);
                }

                tabControl.Add(view);
            }

            html.Add(tabControl.Render(renderContext, visualTree));

            return html;
        }

        /// <summary>
        /// Flattens the supplied form elements (descending recursively into groups) into
        /// a single two-column data grid: column one shows the field name with the field
        /// description as a native HTML <c>title</c> tooltip; column two shows the
        /// inline-editable smart-edit bound to the value. Field references whose name
        /// aliases a system attribute already rendered outside the form (currently only
        /// <see cref="Model.Entities.Object.Description"/>) are skipped to avoid the
        /// duplicate render path. Group labels are not surfaced in this view — the data
        /// grid intentionally collapses the form's visual structure into a flat
        /// name/value table.
        /// </summary>
        /// <returns>The data grid control, or <c>null</c> when no visible field rows
        /// remain after filtering.</returns>
        private static IControl BuildFieldGrid
        (
            IEnumerable<FormElement> elements,
            IDictionary<Guid, Model.Entities.Field> fields,
            IDictionary<Guid, Value> values,
            Model.Entities.Object @object,
            IUri objectUri
        )
        {
            var rows = new List<IControlTableRow>();

            foreach (var fieldRef in FlattenFieldRefs(elements))
            {
                if (!fields.TryGetValue(fieldRef.FieldId, out var field))
                {
                    continue;
                }

                if (IsSystemAttributeAlias(field.Name))
                {
                    continue;
                }

                values.TryGetValue(field.Id, out var value);
                rows.Add(BuildFieldRow(@object, objectUri, field, value));
            }

            if (rows.Count == 0)
            {
                return null;
            }

            var nameColumn = new ControlTableColumn("col-name") { Title = _ => "Name" };
            var valueColumn = new ControlTableColumn("col-value") { Title = _ => "Value" };

            return new ControlTable("field-grid-" + @object.Id.ToString("N"), [nameColumn, valueColumn], [.. rows])
            {
                SuppressHeaders = _ => true,
                Striped = _ => TypeStripedTable.Row
            };
        }

        /// <summary>
        /// Recursively yields every <see cref="FormFieldRefElement"/> contained in the
        /// supplied element tree, in document order honouring <see cref="FormElement.Position"/>.
        /// Group containers are descended into but not emitted themselves.
        /// </summary>
        private static IEnumerable<FormFieldRefElement> FlattenFieldRefs(IEnumerable<FormElement> elements)
        {
            foreach (var element in elements.OrderBy(e => e.Position))
            {
                if (element is FormFieldRefElement fieldRef)
                {
                    yield return fieldRef;
                }
                else if (element is FormGroupElement group)
                {
                    foreach (var inner in FlattenFieldRefs(group.Children))
                    {
                        yield return inner;
                    }
                }
            }
        }

        /// <summary>
        /// Loads the persisted field values of the supplied object via
        /// <see cref="CoreHub.ValueManager"/> and returns them keyed by
        /// <see cref="Value.FieldId"/>. The smart-edit controls use this map to
        /// initialize their inputs so the inline editor shows the current value instead
        /// of being empty on first render.
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The value map; empty when the object has no stored values yet.</returns>
        private static IDictionary<Guid, Value> LoadValuesForObject(Guid objectId)
        {
            return CoreHub.ValueManager
                .GetValues(objectId)
                .GroupBy(v => v.FieldId)
                .ToDictionary(g => g.Key, g => g.First());
        }

        /// <summary>
        /// Returns <c>true</c> when the supplied field name aliases a system attribute
        /// of <see cref="Model.Entities.Object"/> that is rendered outside the form
        /// structure (currently only <see cref="Model.Entities.Object.Description"/>).
        /// </summary>
        /// <param name="fieldName">The field name as configured on the class.</param>
        /// <returns><c>true</c> when the field duplicates a system attribute.</returns>
        private static bool IsSystemAttributeAlias(string fieldName)
        {
            return string.Equals(fieldName, nameof(Model.Entities.Object.Description), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Builds a data-grid row with the field name in the name cell and the
        /// inline-editable smart-edit in the value cell. The name cell carries the field
        /// description as a native HTML <c>title</c> attribute so hovering it shows the
        /// description as a browser tooltip; required fields get a trailing asterisk.
        /// </summary>
        /// <remarks>
        /// When a <paramref name="value"/> exists for the field, it is converted to the
        /// matching <see cref="ControlFormInputValue{T}"/> type and pushed into the input
        /// via <see cref="ControlSmartEdit.Initialize(System.Action{System.Object})"/>;
        /// without this step the input renders empty regardless of the persisted value.
        /// </remarks>
        private static ControlTableRow BuildFieldRow(Model.Entities.Object @object, IUri objectUri, Model.Entities.Field field, Value value)
        {
            var input = CreateInputForField(field);

            var smartEdit = new ControlSmartEdit("field-" + field.Id.ToString("N"))
            {
                ObjectId = _ => @object.Id.ToString(),
                ObjectName = _ => field.Name,
                Uri = _ => objectUri,
                Method = _ => RequestMethod.PUT
            };

            smartEdit.Add(input);

            var initialValue = BuildInputValue(field, value?.Data);
            if (initialValue is not null)
            {
                smartEdit.Initialize(args => args.SetValue(input, initialValue));
            }

            var label = new ControlHtml("field-label-" + field.Id.ToString("N"))
            {
                Html = _ =>
                {
                    var span = new HtmlElementTextSemanticsSpan
                    {
                        Class = "wx-kleenestar-field-label"
                    };

                    if (!string.IsNullOrWhiteSpace(field.Description))
                    {
                        span.AddUserAttribute("title", field.Description);
                    }

                    span.Add(new HtmlText(field.Name + (field.Required ? " *" : "") + ":"));
                    return span.ToString();
                }
            };

            var nameCell = new ControlTableCellPanel("name-" + field.Id.ToString("N"))
            {
                Class = _ => "wx-kleenestar-field-name"
            };
            nameCell.Add(label);

            var valueCell = new ControlTableCellPanel("value-" + field.Id.ToString("N"))
            {
                Class = _ => "wx-kleenestar-field-value"
            };
            valueCell.Add(smartEdit);

            return new ControlTableRow("field-row-" + field.Id.ToString("N"), [nameCell, valueCell]);
        }

        /// <summary>
        /// Converts the persisted <see cref="Value.Data"/> string of a field into the
        /// strongly-typed input value that matches the input control produced by
        /// <see cref="CreateInputForField(Model.Entities.Field)"/>. Boolean and date
        /// fields parse the raw payload, tag fields split on commas, everything else
        /// falls through to a plain string value.
        /// </summary>
        /// <param name="field">The field whose input is being initialised.</param>
        /// <param name="data">The persisted value payload; <c>null</c> or empty for a
        /// field that has not been set yet.</param>
        /// <returns>The input-value wrapper, or <c>null</c> when no value should be pushed
        /// into the input (currently only for <see cref="FieldType.Attachment"/>).</returns>
        private static IControlFormInputValue BuildInputValue(Model.Entities.Field field, string data)
        {
            switch (field.FieldType)
            {
                case FieldType.Boolean:
                    return new ControlFormInputValueBool(
                        bool.TryParse(data, out var b) && b);

                case FieldType.Date:
                    if (string.IsNullOrEmpty(data))
                    {
                        return new ControlFormInputValueDate((DateTime?)null);
                    }
                    return DateTime.TryParse(data, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt)
                        ? new ControlFormInputValueDate(dt)
                        : new ControlFormInputValueDate((DateTime?)null);

                case FieldType.Tag:
                    var items = string.IsNullOrEmpty(data)
                        ? []
                        : data.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    return new ControlFormInputValueStringList(items);

                case FieldType.Attachment:
                    // attachments are referenced through their own upload pipeline, not
                    // through the inline smart-edit value initialisation.
                    return null;

                case FieldType.Number:
                case FieldType.Reference:
                case FieldType.Selection:
                case FieldType.Workflow:
                case FieldType.User:
                case FieldType.Text:
                default:
                    return new ControlFormInputValueString(data ?? string.Empty);
            }
        }

        /// <summary>
        /// Builds a smart-edit control bound to a system attribute of the object (such
        /// as <see cref="Object.Summary"/> or <see cref="Object.Description"/>) that is
        /// not represented as a configurable field.
        /// </summary>
        private static ControlSmartEdit BuildSmartEdit
        (
            Model.Entities.Object @object,
            IUri objectUri,
            string name,
            string value,
            string label,
            bool required = false,
            bool multiline = false
        )
        {
            var input = new ControlFormItemInputText()
            {
                Name = _ => name,
                Label = _ => label,
                Required = _ => required,
                Format = _ => multiline ? TypeEditTextFormat.Wysiwyg : TypeEditTextFormat.Default
            };

            var smartEdit = new ControlSmartEdit("attr-" + name.ToLowerInvariant())
            {
                ObjectId = _ => @object.Id.ToString(),
                ObjectName = _ => name,
                Uri = _ => objectUri,
                Method = _ => RequestMethod.PUT
            };

            smartEdit.Add(input);

            smartEdit.Initialize(args => args.SetValue(input, new ControlFormInputValueString(value)));

            return smartEdit;
        }

        /// <summary>
        /// Resolves the active standard form of the requested type for the given class.
        /// </summary>
        private static Model.Entities.Form ResolveStandardForm(Guid classId, FormType type)
        {
            var form = CoreHub.FormManager
                .GetForms(new ClassIdParameter(classId))
                .FirstOrDefault(f => f.FormType == type && f.State == FormState.Active);

            return form is null ? null : CoreHub.FormManager.GetFormWithStructure(form.Id);
        }

        /// <summary>
        /// Returns the REST endpoint that owns the object's persistence, augmented with
        /// the object's id so smart-edit PUTs target the right record.
        /// </summary>
        private static IUri ResolveObjectRestUri(Model.Entities.Object @object, IRenderControlContext renderContext)
        {
            var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects.Index>();
            if (uri is null)
            {
                return null;
            }

            var withQuery = uri.Add(new UriQuery("id", @object.Id.ToString()));

            return renderContext?.Request is null
                ? withQuery
                : withQuery.BindParameters(renderContext.Request);
        }


        /// <summary>
        /// Creates a typed input control for the given field, mirroring the mapping used
        /// by the edit form so the inline editor presents the same widget the user knows
        /// from the modal.
        /// </summary>
        /// <remarks>
        /// The label is always the field name; the <c>Help</c> property of each input is
        /// driven by <see cref="Field.Description"/> so the description renders as the
        /// tooltip on the help icon next to the label. <see cref="Field.HelpText"/> is
        /// surfaced through the input's <c>Description</c> property (where supported) so
        /// the longer inline help text stays visible beneath the value.
        /// </remarks>
        private static IControlFormItemInput CreateInputForField(Model.Entities.Field field)
        {
            switch (field.FieldType)
            {
                case FieldType.Boolean:
                    return new ControlFormItemInputCheck()
                    {
                        Name = _ => field.Name,
                        Label = _ => field.Name,
                        Description = _ => field.HelpText,
                        Help = _ => field.Description,
                        Required = _ => field.Required
                    };

                case FieldType.Date:
                    return new ControlFormItemInputDate()
                    {
                        Name = _ => field.Name,
                        Label = _ => field.Name,
                        Placeholder = _ => field.Placeholder,
                        Description = _ => field.HelpText,
                        Help = _ => field.Description,
                        Required = _ => field.Required
                    };

                case FieldType.Selection:
                    var combo = new ControlFormItemInputCombo()
                    {
                        Name = _ => field.Name,
                        Label = _ => field.Name,
                        Placeholder = _ => field.Placeholder,
                        Help = _ => field.Description,
                        Required = _ => field.Required
                    };
                    foreach (var option in field.Options ?? [])
                    {
                        combo.Add(new ControlFormItemInputComboItem()
                        {
                            Text = _ => option,
                            Value = _ => option
                        });
                    }
                    return combo;

                case FieldType.Tag:
                    return new ControlFormItemInputTag()
                    {
                        Name = _ => field.Name,
                        Label = _ => field.Name,
                        Placeholder = _ => field.Placeholder,
                        Help = _ => field.Description,
                        Required = _ => field.Required
                    };

                case FieldType.Attachment:
                    return new ControlFormItemInputFile()
                    {
                        Name = _ => field.Name,
                        Label = _ => field.Name,
                        Placeholder = _ => field.Placeholder,
                        Help = _ => field.Description,
                        Required = _ => field.Required
                    };

                case FieldType.Number:
                case FieldType.Reference:
                case FieldType.Workflow:
                case FieldType.User:
                case FieldType.Text:
                default:
                    return new ControlFormItemInputText()
                    {
                        Name = _ => field.Name,
                        Label = _ => field.Name,
                        Placeholder = _ => field.Placeholder,
                        Description = _ => field.HelpText,
                        Help = _ => field.Description,
                        Required = _ => field.Required
                    };
            }
        }
    }
}
