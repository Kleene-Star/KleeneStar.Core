using KleeneStar.Core.WebParameter;
using System;
using System.Reflection;
using KleeneStar.Model.Entities;
using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a edit form fragment for a workspace.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Edit>]
    [Cache]
    public sealed class WorkspaceEditFormFragment : FragmentControlRestFormEdit
    {
        /// <summary>
        /// Gets the input text control for specifying the name of the workspace.
        /// </summary>
        public ControlRestFormItemInputUnique WorkspaceName { get; } = new()
        {
            Name = nameof(Workspace.Name),
            Label = "kleenestar.core:workspace.name.label",
            Placeholder = "kleenestar.core:workspace.name.placeholder",
            Help = "kleenestar.core:workspace.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces.UniqueName>()
        };

        /// <summary>
        /// Gets the input text control for specifying the key of the workspace.
        /// </summary>
        public ControlRestFormItemInputUnique Key { get; } = new()
        {
            Name = nameof(Workspace.Key),
            Label = "kleenestar.core:workspace.key.label",
            Placeholder = "kleenestar.core:workspace.key.placeholder",
            Help = "kleenestar.core:workspace.key.help",
            Required = true,
            MaxLength = 10,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces.UniqueKey>()
        };

        /// <summary>
        /// Gets the input tag definition for the workspace category field.
        /// </summary>
        public ControlFormItemInputTag Category { get; } = new()
        {
            Name = nameof(Workspace.Categories),
            Label = "kleenestar.core:workspace.category.label",
            Placeholder = "kleenestar.core:workspace.category.placeholder",
            Help = "kleenestar.core:workspace.category.help"
        };

        /// <summary>
        /// Gets the input text control for specifying the description of the workspace.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Workspace.Description),
            Label = "kleenestar.core:workspace.description.label",
            Placeholder = "kleenestar.core:workspace.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Gets the input selection control for the state.
        /// </summary>
        public ControlRestFormItemInputSelection WorkspaceState { get; } = new()
        {
            Name = nameof(Model.Entities.Workspace.State),
            Label = "kleenestar.core:workspace.state.label",
            Placeholder = "kleenestar.core:workspace.state.placeholder",
            Help = "kleenestar.core:workspace.state.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces.State>()
        };

        /// <summary>
        /// Gets the input selection control for the inherited workspace.
        /// </summary>
        public ControlRestFormItemInputSelection InheritedSelection { get; } = new()
        {
            Name = "InheritedId",
            Label = "kleenestar.core:workspace.inherited.label",
            Placeholder = "kleenestar.core:workspace.inherited.placeholder",
            Help = "kleenestar.core:workspace.inherited.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces._workspacekey_.Inherited>()
        };

        /// <summary>
        /// Gets the input selection control for the access modifier.
        /// </summary>
        public ControlRestFormItemInputSelection AccessModifierSelection { get; } = new()
        {
            Name = "AccessModifier",
            Label = "kleenestar.core:workspace.accessmodifier.label",
            Placeholder = "kleenestar.core:workspace.accessmodifier.placeholder",
            Help = "kleenestar.core:workspace.accessmodifier.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces.AccessModifier>()
        };

        /// <summary>
        /// Gets the checkbox control for the sealed flag.
        /// </summary>
        public ControlFormItemInputCheck WorkspaceSealed { get; } = new()
        {
            Name = "Sealed",
            Label = "kleenestar.core:workspace.sealed.label",
            Help = "kleenestar.core:workspace.sealed.help"
        };

        /// <summary>
        /// Gets the tenant management input.
        /// </summary>
        public ControlFormItemInputTag Tenant { get; } = new()
        {
            Name = "Tenant",
            Label = "kleenestar.core:workspace.tenant.label",
            Placeholder = "kleenestar.core:workspace.tenant.placeholder",
            Help = "kleenestar.core:workspace.tenant.help"
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public WorkspaceEditFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Key);
            Add(WorkspaceName);
            Add(Category);
            Add(Description);
            Add(InheritedSelection);
            Add(AccessModifierSelection);
            Add(WorkspaceSealed);
            Add(Tenant);
            Add(WorkspaceState);

            Mode = TypeRestFormMode.Edit;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workspaces.Index>();
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
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            var key = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(key?.Value);
            var id = workspace?.Id.ToString();

            SetControlDisabled(InheritedSelection, workspace?.Sealed == true);
            SetControlDisabled(AccessModifierSelection, workspace?.Sealed == true);

            return base.Render(renderContext, visualTree, Items, id, Uri);
        }

        private static void SetControlDisabled(object control, bool disabled)
        {
            var property = control?.GetType().GetProperty("Disabled", BindingFlags.Instance | BindingFlags.Public);
            property?.SetValue(control, disabled);
        }
    }
}
