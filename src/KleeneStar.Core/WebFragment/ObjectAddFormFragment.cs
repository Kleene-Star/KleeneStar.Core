using KleeneStar.Model.Entities;
using System.Linq;
using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a multi-step wizard fragment for creating a new object.
    /// </summary>
    /// <remarks>
    /// The wizard guides the user through three steps:
    /// <list type="number">
    ///   <item>Select Workspace — a cascading control whose top level lists workspace categories and whose
    ///   children list the workspaces of each category.</item>
    ///   <item>Select Template — a tile control that displays all class templates available for the workspace
    ///   selected in step 1. Each tile carries the workspace id so the client can filter the visible templates
    ///   based on the prior selection.</item>
    ///   <item>New — the dynamic property form for the new object. Hidden inputs preserve the workspace and
    ///   class chosen in the previous steps so they are submitted together with the property values.</item>
    /// </list>
    /// </remarks>
    [Title("kleenestar.core:object.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Objects.Add>]
    [Cache]
    public sealed class ObjectAddFormFragment : FragmentControlRestWizard
    {
        /// <summary>
        /// Gets the cascading input control for selecting a workspace.
        /// The top-level entries represent workspace categories, the second-level entries represent
        /// the workspaces inside the selected category.
        /// </summary>
        public ControlFormItemInputCascading WorkspaceSelection { get; } = new()
        {
            Name = _ => nameof(Object.WorkspaceId),
            Label = _ => "kleenestar.core:object.workspace.label",
            Help = _ => "kleenestar.core:object.workspace.help",
            Placeholder = _ => "kleenestar.core:object.workspace.placeholder",
            Required = _ => true
        };

        /// <summary>
        /// Gets the tile input control for selecting an object template (class).
        /// </summary>
        public ControlFormItemInputTile TemplateSelection { get; } = new()
        {
            Name = _ => nameof(Object.ClassId),
            Label = _ => "kleenestar.core:object.template.label",
            Help = _ => "kleenestar.core:object.template.help",
            LargeIcon = _ => true,
            Required = _ => true
        };

        /// <summary>
        /// Gets the input text control for specifying the summary of the object.
        /// </summary>
        public ControlRestFormItemInputUnique Summary { get; } = new()
        {
            Name = _ => nameof(Object.Summary),
            Label = _ => "kleenestar.core:object.summary.label",
            Placeholder = "kleenestar.core:object.summary.placeholder",
            Help = _ => "kleenestar.core:object.summary.help",
            Required = _ => true,
        };

        /// <summary>
        /// Gets the input text control for specifying the description of the object.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new()
        {
            Name = _ => nameof(Object.Description),
            Label = _ => "kleenestar.core:object.description.label",
            Placeholder = _ => "kleenestar.core:object.description.placeholder",
            Format = _ => TypeEditTextFormat.Wysiwyg,
            Required = _ => false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            var step1 = new ControlRestWizardPage("step-workspace");
            step1.Add(WorkspaceSelection);

            var step2 = new ControlRestWizardPage("step-template");
            step2.Add(TemplateSelection);

            var step3 = new ControlRestWizardPage("step-properties");
            step3.Add(Summary);
            step3.Add(Description);

            Add(step1, step2, step3);

            Mode = TypeRestFormMode.Add;
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects.Index>();
        }

        /// <summary>
        /// Renders the control as an HTML node.
        /// </summary>
        /// <param name="renderContext">
        /// The context in which the control is rendered.
        /// </param>
        /// <param name="visualTree">
        /// The visual tree representing the control's structure.
        /// </param>
        /// <returns>
        /// An HTML node representing the rendered control.
        /// </returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            PopulateWorkspaceSelection();
            PopulateTemplateSelection();

            return base.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Populates the workspace cascading control with the categories and workspaces currently registered.
        /// Workspaces are nested below their respective category. Workspaces without any category are added
        /// at the top level so they remain selectable.
        /// </summary>
        private void PopulateWorkspaceSelection()
        {
            // remove previously rendered options to avoid duplicates on subsequent renders
            foreach (var existing in WorkspaceSelection.Options.ToList())
            {
                WorkspaceSelection.Remove(existing);
            }

            var categories = CoreHub.WorkspaceManager
                .GetCategories(new Query<Category>())
                .OrderBy(x => x.Name)
                .ToList();

            var workspaces = CoreHub.WorkspaceManager
                .GetWorkspaces(new Query<Workspace>())
                .OrderBy(x => x.Name)
                .ToList();

            foreach (var category in categories)
            {
                var categoryNode = new ControlFormItemInputCascadingItem(category.Id.ToString())
                {
                    Text = _ => category.Name
                };

                foreach (var workspace in workspaces.Where(x => x.Categories?.Any(c => c.Id == category.Id) == true))
                {
                    categoryNode.Add(CreateWorkspaceItem(workspace));
                }

                WorkspaceSelection.Add(categoryNode);
            }

            foreach (var workspace in workspaces.Where(x => x.Categories == null || x.Categories.Count == 0))
            {
                WorkspaceSelection.Add(CreateWorkspaceItem(workspace));
            }
        }

        /// <summary>
        /// Populates the template tile control with all non-abstract classes. Each tile carries the workspace
        /// id of its class via its identifier (<c>{workspaceId}:{classId}</c>) so a client-side filter driven
        /// by the cascading control can hide the templates that do not belong to the workspace selected in
        /// the previous wizard step.
        /// </summary>
        private void PopulateTemplateSelection()
        {
            foreach (var existing in TemplateSelection.Items.ToList())
            {
                TemplateSelection.Remove(existing);
            }

            var classes = CoreHub.ClassManager
                .GetClasses(new Query<Class>())
                .Where(x => !x.IsAbstract)
                .OrderBy(x => x.Name);

            foreach (var @class in classes)
            {
                var card = new ControlTileCard($"{@class.WorkspaceId}:{@class.Id}")
                {
                    Header = _ => @class.Name,
                    Icon = _ => @class.Icon
                };

                if (!string.IsNullOrWhiteSpace(@class.Description))
                {
                    card.Add(new ControlText { Text = @class.Description });
                }

                TemplateSelection.Add(card);
            }
        }

        /// <summary>
        /// Creates a cascading item describing the specified workspace.
        /// </summary>
        /// <param name="workspace">The workspace to expose as a selectable option.</param>
        /// <returns>The cascading item carrying the workspace id, name and icon.</returns>
        private static ControlFormItemInputCascadingItem CreateWorkspaceItem(Workspace workspace)
        {
            return new ControlFormItemInputCascadingItem(workspace.Id.ToString())
            {
                Text = _ => workspace.Name,
                Icon = _ => workspace.Icon
            };
        }
    }
}
