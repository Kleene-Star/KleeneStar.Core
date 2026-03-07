using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WWW.Classes._workspacekey_._classid_;
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
    /// Represents a clone form fragment for a class.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<Clone>]
    [Cache]
    public sealed class ClassCloneFormFragment : FragmentControlRestFormClone
    {
        /// <summary>
        /// Returns the input text control for specifying the name of the class.
        /// </summary>
        public ControlRestFormItemInputUnique ClassName { get; } = new()
        {
            Name = nameof(Class.Name),
            Label = "kleenestar.core:class.name.label",
            Placeholder = "kleenestar.core:class.name.placeholder",
            Help = "kleenestar.core:class.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<WWW.Api._1_.Workspaces.UniqueName>()
        };

        /// <summary>
        /// Returns the input text control for specifying the description of the workspace.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Workspace.Description),
            Label = "kleenestar.core:class.description.label",
            Placeholder = "kleenestar.core:class.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ClassCloneFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(ClassName);
            Add(Description);

            Mode = TypeRestFormMode.Clone;
            Uri = CoreHub.GetUri<WWW.Api._1_.Classes._workspacekey_.Index>();
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
            var param = renderContext.Request.GetParameter<ClassIdParameter>();

            return base.Render(renderContext, visualTree, Items, param?.Value, Uri);
        }
    }
}
