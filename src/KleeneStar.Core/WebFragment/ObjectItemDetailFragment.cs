using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
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

namespace KleeneStar.Core.WebFragment
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

            var html = new HtmlElementTextContentDiv()
            {
                Id = Id,
                Class = Css.Concatenate("wx-kleenestar-object-detail", GetClasses()),
                Style = GetStyles(),
                Role = Role,
                DataTheme = Theme.ToValue()
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

            var orderedTabs = form.Tabs.OrderBy(t => t.Position).ToList();

            if (orderedTabs.Count == 1)
            {
                foreach (var element in orderedTabs[0].Elements.OrderBy(e => e.Position))
                {
                    var node = RenderElement(element, fields, @object, objectUri, renderContext, visualTree);
                    if (node is not null)
                    {
                        html.Add(node);
                    }
                }

                return html;
            }

            var tabControl = new ControlTab("tabs-" + @object.Id.ToString("N"))
            {
                Layout = TypeLayoutTab.Underline
            };

            foreach (var t in orderedTabs)
            {
                var view = new ControlTabView("tab-" + t.Id.ToString("N"))
                {
                    Title = t.Name
                };

                foreach (var element in t.Elements.OrderBy(e => e.Position))
                {
                    var node = BuildElementControl(element, fields, @object, objectUri);
                    if (node is not null)
                    {
                        view.Add(node);
                    }
                }

                tabControl.Add(view);
            }

            html.Add(tabControl.Render(renderContext, visualTree));

            return html;
        }

        /// <summary>
        /// Renders a single top-level element directly into the view. Group elements are
        /// emitted as a panel that recurses into their children; field references are
        /// rendered as inline-editable smart-edit controls.
        /// </summary>
        private static IHtmlNode RenderElement
        (
            FormElement element,
            IDictionary<Guid, Field> fields,
            Model.Entities.Object @object,
            IUri objectUri,
            IRenderControlContext renderContext,
            IVisualTreeControl visualTree
        )
        {
            var control = BuildElementControl(element, fields, @object, objectUri);

            return control?.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Builds a panel control for a top-level element. Group elements recurse into
        /// their child elements; field references resolve to inline-editable smart-edit
        /// controls.
        /// </summary>
        private static IControl BuildElementControl
        (
            FormElement element,
            IDictionary<Guid, Field> fields,
            Model.Entities.Object @object,
            IUri objectUri
        )
        {
            if (element is FormFieldRefElement fieldRef)
            {
                return fields.TryGetValue(fieldRef.FieldId, out var field)
                    ? BuildFieldSmartEdit(@object, objectUri, field)
                    : null;
            }

            if (element is FormGroupElement group)
            {
                var panel = new ControlPanel("group-" + group.Id.ToString("N"))
                {
                    Direction = MapGroupDirection(group.Layout)
                };

                if (!string.IsNullOrWhiteSpace(group.Label))
                {
                    panel.Add(new ControlText() { Text = group.Label });
                }

                foreach (var child in group.Children.OrderBy(c => c.Position))
                {
                    var childControl = BuildElementControl(child, fields, @object, objectUri);
                    if (childControl is not null)
                    {
                        panel.Add(childControl);
                    }
                }

                return panel;
            }

            return null;
        }

        /// <summary>
        /// Builds a smart-edit control bound to a single class field. Each rendered field
        /// becomes its own inline editor that PUTs back to the object endpoint when the
        /// user commits the change.
        /// </summary>
        private static ControlSmartEdit BuildFieldSmartEdit(Model.Entities.Object @object, IUri objectUri, Field field)
        {
            var input = CreateInputForField(field);

            var smartEdit = new ControlSmartEdit("field-" + field.Id.ToString("N"))
            {
                ObjectId = @object.Id.ToString(),
                ObjectName = field.Name,
                Uri = objectUri,
                Method = RequestMethod.PUT
            };

            smartEdit.Add(input);

            return smartEdit;
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
                Name = name,
                Label = label,
                Required = required,
                Format = multiline ? TypeEditTextFormat.Wysiwyg : TypeEditTextFormat.Default
            };

            var smartEdit = new ControlSmartEdit("attr-" + name.ToLowerInvariant())
            {
                ObjectId = @object.Id.ToString(),
                ObjectName = name,
                Uri = objectUri,
                Method = RequestMethod.PUT
            };

            smartEdit.Add(input);

            smartEdit.Initialize(args => args.SetValue(input, new ControlFormInputValueString(value)));

            return smartEdit;
        }

        /// <summary>
        /// Resolves the active standard form of the requested type for the given class.
        /// </summary>
        private static Form ResolveStandardForm(Guid classId, FormType type)
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
            var uri = CoreHub.GetUri<WWW.Api._1_.Objects.Index>();
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
        /// Maps a form group layout to the direction setting of a panel that hosts the
        /// group's read-only children. Mixed and column layouts are flattened to vertical
        /// because the read-only view does not need the form's label/help-below
        /// arrangements.
        /// </summary>
        private static TypeDirection MapGroupDirection(FormGroupLayout layout)
        {
            return layout switch
            {
                FormGroupLayout.Horizontal => TypeDirection.Horizontal,
                FormGroupLayout.ColumnHorizontal => TypeDirection.Horizontal,
                _ => TypeDirection.Vertical,
            };
        }

        /// <summary>
        /// Creates a typed input control for the given field, mirroring the mapping used
        /// by the edit form so the inline editor presents the same widget the user knows
        /// from the modal.
        /// </summary>
        private static IControlFormItemInput CreateInputForField(Field field)
        {
            switch (field.FieldType)
            {
                case FieldType.Boolean:
                    return new ControlFormItemInputCheck()
                    {
                        Name = field.Name,
                        Label = field.Name,
                        Help = field.HelpText,
                        Required = field.Required
                    };

                case FieldType.Date:
                    return new ControlFormItemInputDate()
                    {
                        Name = field.Name,
                        Label = field.Name,
                        Placeholder = field.Placeholder,
                        Help = field.HelpText,
                        Required = field.Required
                    };

                case FieldType.Selection:
                    var combo = new ControlFormItemInputCombo()
                    {
                        Name = field.Name,
                        Label = field.Name,
                        Placeholder = field.Placeholder,
                        Help = field.HelpText,
                        Required = field.Required
                    };
                    foreach (var option in field.Options ?? [])
                    {
                        combo.Add(new ControlFormItemInputComboItem()
                        {
                            Text = option,
                            Value = option
                        });
                    }
                    return combo;

                case FieldType.Tag:
                    return new ControlFormItemInputTag()
                    {
                        Name = field.Name,
                        Label = field.Name,
                        Placeholder = field.Placeholder,
                        Help = field.HelpText,
                        Required = field.Required
                    };

                case FieldType.Attachment:
                    return new ControlFormItemInputFile()
                    {
                        Name = field.Name,
                        Label = field.Name,
                        Placeholder = field.Placeholder,
                        Help = field.HelpText,
                        Required = field.Required
                    };

                case FieldType.Number:
                case FieldType.Reference:
                case FieldType.Workflow:
                case FieldType.User:
                case FieldType.Text:
                default:
                    return new ControlFormItemInputText()
                    {
                        Name = field.Name,
                        Label = field.Name,
                        Placeholder = field.Placeholder,
                        Help = field.HelpText,
                        Required = field.Required
                    };
            }
        }
    }
}
